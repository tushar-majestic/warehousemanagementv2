namespace LabMaterials.dtos
{
    public class UsersInfo
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserGroup { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string FullName { get; internal set; }
        public string Email { get; internal set; }
        public string EmpAffiliation { get; internal set; }
        public string? JobNumber { get; internal set; }
        public string? Transfer { get; internal set; }
        public string? GroupName { get; internal set; }
        public string IsActive { get; internal set; }
    }
}
