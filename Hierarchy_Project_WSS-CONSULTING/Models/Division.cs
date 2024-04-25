using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Hierarchy_Project_WSS_CONSULTING.Models
{
    public class Division
    {
        [XmlIgnore]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; private set; }

        // Для упрощения используем имя
        public string Name { get; set; }

        [XmlIgnore]
        public HierarchyId PathFromPatriarch { get; set; }

        [NotMapped]
        public string PathFromPatriarchString
        {
            get { return PathFromPatriarch.ToString(); }
            set { PathFromPatriarch = HierarchyId.Parse(value); }
        }

        public Division() { }
        public Division(HierarchyId pathFromPatriarch, string name)
        {
            Name = name;
            PathFromPatriarch = pathFromPatriarch;
        }

    }
}