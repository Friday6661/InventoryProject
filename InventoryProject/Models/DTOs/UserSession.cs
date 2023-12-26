using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryProject.Models.DTOs
{
    public record UserSession(string? Id, string? UserName, string? Email, string? Role);
}