namespace LabMaterials.dtos
{
    public class UserInfo
    {
        public int UserID {  get; set; }
        public string UserName {  get; set; }
        public string FullName { get; set; } 
        public string Email { get; set; }
        public string IsActive { get; set; }
        public string EnableBtnText { get; set; }
        public string IsLocked { get; set; }
        public string IsADUser { get; set; }
        public string GroupName { get; set; }

    }
}
