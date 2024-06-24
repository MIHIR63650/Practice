using System.ComponentModel.DataAnnotations;

namespace UserApi.Models;

public class UserItem
{
    [Key]
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? TelephoneNumber { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? DOB { get; set; }
    public string? GrossSalaryFY201920 { get; set; }
    public string? GrossSalaryFY202021 { get; set; }
    public string? GrossSalaryFY202122 { get; set; }
    public string? GrossSalaryFY202223 { get; set; }
    public string? GrossSalaryFY202324 { get; set; }
}
