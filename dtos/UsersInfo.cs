namespace LabMaterials.dtos
{
    public class UsersInfo
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserGroup { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
