using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AP204_Pronia.ViewModels
{
    public class RegisterVM
    {
        [Required,StringLength(maximumLength:15)]
        public string Firstname { get; set; }
        [Required,StringLength(maximumLength:20)]
        public string Lastname { get; set; }
        [Required, StringLength(maximumLength: 15)]
        public string Username { get; set; }
        [Required,DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required,DataType(DataType.Password)]
        public string Password { get; set; }
        [Required,DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

    }
}
