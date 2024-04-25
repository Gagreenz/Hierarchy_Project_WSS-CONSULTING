using System.ComponentModel.DataAnnotations;

namespace Hierarchy_Project_WSS_CONSULTING.Models.Dtos
{
    public class MoveDivisionDto
    {
        [Required]
        public string OldParentId { get; set; }
        [Required]
        public string NewParentId { get; set; }
    }
}