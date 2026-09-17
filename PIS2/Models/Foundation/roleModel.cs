using System.ComponentModel.DataAnnotations;

namespace PIS2.Models.Foundation
{
    public class roleModel
    {
        [Key]
        public int roleID { get; set; }
        public string roleName { get; set; }
        public string roleDescription { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }
        public roleModel() { }
    }

    public class permissionModel
    {
        [Key]
        public int permissionID { get; set; }
        public string permissionName { get; set; }
        public string permissionAction { get; set; }
        public string permissionDescription { get; set; }
        public string modifiedBy { get; set; }
        public DateTime modifiedDate { get; set; }
        public permissionModel() { }   
    }

    public class moduleModel
    {
        [Key]
        public int moduleID { get; set; }
        public string moduleName { get; set; }
        public string moduleDescription { get; set; }
        public string modifiedBy { get; set; } 
        public DateTime modifiedDate { get; set; }
        public moduleModel() { }
    }
}
