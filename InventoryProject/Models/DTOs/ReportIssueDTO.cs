using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Enum;

namespace InventoryProject.Models.DTOs
{
    public class ReportIssueDTO
    {
        public IssueStatus Status { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}