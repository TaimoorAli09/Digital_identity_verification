using backend.Data;
using backend.DTOs;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // ✅ EXTENSION INCLUDED

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly SignatureService _signatureService;
    private readonly VerifyService _verifyService;
    private readonly QrService _qrService;
    private readonly TokenService _tokenService;
    private readonly IConfiguration _configuration; // ✅ REFRESH LAYER FIELD


    // constructor updated to support appsettings extraction automatically
    public StudentController(
     AppDbContext db,
     SignatureService signatureService,
     VerifyService verifyService,
     QrService qrService,
     TokenService tokenService,
     IConfiguration configuration) // ✅ SERVICE INJECTED
    {
        _db = db;
        _signatureService = signatureService;
        _verifyService = verifyService;
        _qrService = qrService;
        _tokenService = tokenService;
        _configuration = configuration; // ✅ ASSIGNED
    }

    // =========================================
    // CREATE STUDENT
    // =========================================

    [HttpPost("create")]
    public IActionResult CreateStudent(
    [FromForm] StudentDto dto)
    {
        try
        {
            // =============================
            // IMAGE VALIDATION
            // =============================
            var image = dto.Image;

            if (image == null || image.Length == 0)
                return BadRequest("Image required");

            var extension = Path.GetExtension(image.FileName).ToLower();
            var allowed = new[] { ".jpg", ".jpeg", ".png" };

            if (!allowed.Contains(extension))
                return BadRequest("Only JPG/PNG allowed");

            // =============================
            // CREATE UPLOAD FOLDER
            // =============================
            if (!Directory.Exists("wwwroot/uploads"))
            {
                Directory.CreateDirectory("wwwroot/uploads");
            }

            // =============================
            // SAVE IMAGE
            // =============================
            var fileName = Guid.NewGuid() + extension;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            image.CopyTo(stream);

            // =============================
            // IMAGE URL
            // =============================
            //var imageUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
            var imageUrl = $"/uploads/{fileName}";

            // =============================
            // GENERATE RANDOM TOKEN & SIGN IT
            // =============================
            var token = _tokenService.GenerateToken();
            var signature = _signatureService.SignData(token);

            // =================================================================
            // ✅ REAL-WORLD LINK GENERATION FOR MOBILE PHONES
            // =================================================================

            // 1. Fetch your frontend address route profile path cleanly from settings
            //string frontendBaseUrl = _configuration["AppSettings:FrontendBaseUrl"];

            string frontendBaseUrl = $"{Request.Scheme}://{Request.Host}";
            // 2. Escape variables to protect Base64 symbols (+, /, =) from breaking inside web URLs
            string base64Signature = Convert.ToBase64String(signature);
            string encodedSignature = Uri.EscapeDataString(base64Signature);
            string encodedToken = Uri.EscapeDataString(token);

            // 3. Assemble the direct web link path query string
            //string qrWebLink = $"{frontendBaseUrl}/frontend/scan.html?token={encodedToken}&signature={encodedSignature}";

            string qrWebLink =$"{frontendBaseUrl}/frontend/scan.html?token={encodedToken}&signature={encodedSignature}";
            // =================================================================
            // GENERATE QR
            // =================================================================

            // Pass the clickable web link string right into your existing generator engine
            var qrImage = _qrService.GenerateQr(qrWebLink);

            // =============================
            // SAVE STUDENT
            // =============================
            var student = new Student
            {
                StudentId = dto.StudentId,
                Name = dto.Name,
                Age = dto.Age,
                Program = dto.Program,
                Payload = "",
                Cnic = dto.Cnic,
                ImageUrl = imageUrl,
                Token = token,
                Signature = signature,
                QrImage = qrImage,
                CreatedAt = DateTime.Now,
                ExpiryDate = DateTime.Now.AddYears(1)
            };

            _db.Students.Add(student);
            _db.SaveChanges();

            // =============================
            // RESPONSE
            // =============================
            return Ok(new
            {
                message = "Student created",
                student.Name,
                student.Program,
                student.ImageUrl,
                qrBase64 = Convert.ToBase64String(qrImage)
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // ========== verify student ==========================
    [HttpPost("verify")]
    public IActionResult Verify(
    [FromBody] VerifyDto dto)
    {
        try
        {
            bool valid = _verifyService.VerifySignature(dto.Token, dto.Signature);

            if (!valid)
            {
                return BadRequest(new { status = "FAKE QR" });
            }

            var student = _db.Students.FirstOrDefault(s => s.Token == dto.Token);

            if (student == null)
            {
                return BadRequest("Student not found");
            }

            if (DateTime.Now > student.ExpiryDate)
            {
                return BadRequest("Card expired");
            }

            return Ok(new
            {
                status = "VALID",
                student.Name,
                student.Age,
                student.Program,
                student.Cnic,
                student.ImageUrl
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // =========================================
    // GET ALL STUDENTS
    // =========================================
    [HttpGet("getall")]
    public IActionResult GetAllStudents()
    {
        try
        {
            var students = _db.Students.Select(student => new
            {
                student.Id,
                student.StudentId,
                student.Name,
                student.Age,
                student.Program,
                student.Cnic,
                student.ImageUrl,
                qrBase64 = Convert.ToBase64String(student.QrImage)
            }).ToList();

            return Ok(students);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}