using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;
    private readonly HashService _hashService;
    private readonly SignatureService _signatureService;
    private readonly QrService _qrService;

    public StudentController(
        AppDbContext db,
        TokenService tokenService,
        HashService hashService,
        SignatureService signatureService,
        QrService qrService)
    {
        _db = db;
        _tokenService = tokenService;
        _hashService = hashService;
        _signatureService = signatureService;
        _qrService = qrService;
    }

    // ==========================
    // CREATE STUDENT
    // ==========================
    [HttpPost("create")]
    public IActionResult CreateStudent(StudentDto dto)
    {
        // 1. Generate secure token
        var token = _tokenService.GenerateToken();

        // 2. Prepare data string (important for hashing + signing)
        var data = $"{dto.Name}|{dto.Age}|{dto.Program}";

        // 3. Generate hash
        var hash = _hashService.GenerateHash(data);

        // 4. Generate digital signature (RSA)
        var signature = _signatureService.SignData(data);

        // 5. Create student object
        var student = new Student
        {
            Name = dto.Name,
            Age = dto.Age,
            Program = dto.Program,

            Token = token,
            Hash = hash,
            Signature = signature,

            CreatedAt = DateTime.Now,
            ExpiryDate = DateTime.Now.AddYears(1)
        };

        // 6. Save to database
        _db.Students.Add(student);
        _db.SaveChanges();

        // 7. Create verification URL (used in QR)
        var verifyUrl = $"https://localhost:5001/api/student/verify/{token}";

        // 8. Generate QR code
        var qrImage = _qrService.GenerateQr(verifyUrl);

        // 9. Return response
        return Ok(new
        {
            message = "Student created successfully",
            student.Name,
            student.Program,
            student.Token,
            verifyUrl,
            qrBase64 = Convert.ToBase64String(qrImage)
        });
    }

    // ==========================
    // VERIFY STUDENT
    // ==========================
    [HttpGet("verify/{token}")]
    public IActionResult Verify(string token)
    {
        // 1. Find student
        var student = _db.Students.FirstOrDefault(s => s.Token == token);

        if (student == null)
            return BadRequest("INVALID TOKEN");

        // 2. Check expiry
        if (DateTime.Now > student.ExpiryDate)
            return BadRequest("EXPIRED");

        // 3. Recreate original data
        var data = $"{student.Name}|{student.Age}|{student.Program}";

        // 4. Load public key
        var publicKey = System.IO.File.ReadAllText("keys/public.pem");

        using var rsa = System.Security.Cryptography.RSA.Create();
        rsa.ImportFromPem(publicKey.ToCharArray());

        // 5. Verify signature
        var isValid = rsa.VerifyData(
            System.Text.Encoding.UTF8.GetBytes(data),
            student.Signature,
            System.Security.Cryptography.HashAlgorithmName.SHA256,
            System.Security.Cryptography.RSASignaturePadding.Pkcs1
        );

        if (!isValid)
            return BadRequest("TAMPERED DATA");

        // 6. Return valid result
        return Ok(new
        {
            status = "VALID",
            student.Name,
            student.Program
        });
    }
}