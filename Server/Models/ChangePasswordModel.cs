namespace VueStart;

using System.ComponentModel.DataAnnotations;

public class ChangePasswordModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string NewPassword { get; set; }
}
