namespace TmsApi.DTOs;
using System.ComponentModel.DataAnnotations;
public class UpdateStudentDto
{
    public string Name { get; set; } = "";
    public decimal GPA { get; set; }
}



public class CreateStudentDto
{
    [Required]
    [MaxLength(10)]

    public string RegistrationNumber { get; set; } = "";

    public string Name { get; set; } = "";

    public decimal GPA { get; set; }

    public bool IsActive { get; set; }
}