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
    private readonly SignatureService _signatureService;
    private readonly VerifyService _verifyService;
    private readonly QrService _qrService;

    public StudentController(
        AppDbContext db,
        SignatureService signatureService,
        VerifyService verifyService,
        QrService qrService)
    {
        _db = db;
        _signatureService = signatureService;
        _verifyService = verifyService;
        _qrService = qrService;
    }

    // =========================================
    // CREATE STUDENT
    // =========================================

    [HttpPost("create")]
    public IActionResult CreateStudent([FromForm] StudentDto dto)
    {
        try
        {
            // ================= IMAGE VALIDATION =================
            var image = dto.Image;

            if (image == null || image.Length == 0)
                return BadRequest("Image is required");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(image.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Only JPG and PNG allowed");

            if (image.Length > 5 * 1024 * 1024)
                return BadRequest("Max file size is 5MB");

            // ================= SAVE IMAGE =================
            if (!Directory.Exists("uploads"))
                Directory.CreateDirectory("uploads");

            var fileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine("uploads", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyTo(stream);
            }

            var imageUrl =
                $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";

            // ================= PAYLOAD =================
            var payloadObj = new
            {
                studentId = dto.StudentId,
                name = dto.Name,
                age = dto.Age,
                program = dto.Program,
                cnic = dto.Cnic,
                image = imageUrl,
                expiry = DateTime.Now.AddYears(1)
            };

            var payloadJson =
                System.Text.Json.JsonSerializer.Serialize(payloadObj);

            // ================= SIGNATURE =================
            var signature = _signatureService.SignData(payloadJson);

            // ================= QR GENERATION =================
            var qrData = new
            {
                payload = payloadJson,
                signature = Convert.ToBase64String(signature)
            };

            var qrString =
                System.Text.Json.JsonSerializer.Serialize(qrData);

            var qrImage = _qrService.GenerateQr(qrString);

            // ================= SAVE STUDENT =================
            var student = new Student
            {
                StudentId = dto.StudentId,
                Name = dto.Name,
                Age = dto.Age,
                Program = dto.Program,
                Cnic = dto.Cnic,
                ImageUrl = imageUrl,

                Payload = payloadJson,
                Signature = signature,

                // ✅ SAVE QR IN DB
                QrImage = qrImage,

                CreatedAt = DateTime.Now,
                ExpiryDate = DateTime.Now.AddYears(1)
            };

            _db.Students.Add(student);
            _db.SaveChanges();

            // ================= RESPONSE =================
            return Ok(new
            {
                message = "Student created successfully",
                student.Id,
                student.StudentId,
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

                // ✅ LOAD QR FROM DB
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