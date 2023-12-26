using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryProject.Models.DTOs;

namespace InventoryProject.Contracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(SendEmailDTO sendEmailDTO);
        public string LoadEmailTemplate(string templateFilePath);
        public string ReplacePlaceholders(string template, Dictionary<string, string> replacements);
    }
}