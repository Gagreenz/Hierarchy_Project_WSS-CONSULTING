using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Hierarchy_Project_WSS_CONSULTING.Models.Dtos
{
    public class NewDivisionDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ParentId { get; set; }
    }
}