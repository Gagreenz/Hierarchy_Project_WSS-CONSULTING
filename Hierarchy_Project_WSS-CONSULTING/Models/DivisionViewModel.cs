using Microsoft.EntityFrameworkCore;

namespace Hierarchy_Project_WSS_CONSULTING.Models
{
    public class DivisionViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public HierarchyId PathFromPatriarch { get; set; }
    }
}
