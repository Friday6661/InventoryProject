using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryProject.Models.DTOs
{
    public class ResetPasswordDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string newPassword { get; set; }
    }
}