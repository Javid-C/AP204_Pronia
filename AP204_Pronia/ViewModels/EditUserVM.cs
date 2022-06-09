using System.ComponentModel.DataAnnotations;

namespace AP204_Pronia.ViewModels
{
    public class EditUserVM
    {
        [Required, StringLength(maximumLength: 15)]
        public string Firstname { get; set; }
        [Required, StringLength(maximumLength: 20)]
        public string Lastname { get; set; }
        [Required, StringLength(maximumLength: 15)]
        public string Username { get; set; }
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
