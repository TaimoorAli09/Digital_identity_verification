namespace backend.Models;

public class Student
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Age { get; set; }

    public string Program { get; set; }

    public string Token { get; set; }

    public string Hash { get; set; }

    public byte[] Signature { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiryDate { get; set; }
}