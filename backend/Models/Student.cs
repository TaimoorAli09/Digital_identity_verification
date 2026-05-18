namespace backend.Models;

public class Student
{
    public int Id { get; set; }

    public string StudentId { get; set; }

    public string Name { get; set; }

    public int Age { get; set; }

    public string Program { get; set; }

    public string Cnic { get; set; }

    public string ImageUrl { get; set; }

    public string Payload { get; set; }

    public byte[] Signature { get; set; }

    // ✅ NEW FIELD: store QR image
    public byte[] QrImage { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiryDate { get; set; }
}