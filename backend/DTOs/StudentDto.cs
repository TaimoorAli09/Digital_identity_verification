using Microsoft.AspNetCore.Http;

namespace backend.DTOs;

public class StudentDto
{
    public string StudentId { get; set; }

    public string Name { get; set; }

    public int Age { get; set; }

    public string Program { get; set; }

    public string Cnic { get; set; }

    public IFormFile Image { get; set; }
}