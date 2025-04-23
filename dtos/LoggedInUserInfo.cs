using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LabMaterials.dtos
{
    public class LoggedInUserInfo
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; } 
        public bool IsActive { get; set; }
        public bool Locked { get; set; }
        public List<string> Privileges { get; set; }
    }
}
