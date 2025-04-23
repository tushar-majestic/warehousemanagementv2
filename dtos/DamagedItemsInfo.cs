namespace LabMaterials.dtos
{
    public class DamagedItemsInfo
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public DateTime? DamageDate { get; set; }
        public int? DamageQuantity { get; set; }
        public string DamageReason { get; set; }
    }
}
