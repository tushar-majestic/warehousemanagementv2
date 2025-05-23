namespace LabMaterials.dtos
{
    public class ItemInfo
    {
        public int ItemId {  get; set; }
        public string GroupCode {  get; set; }
        public string GroupDesc { get; set; }
        public string ItemTypeCode { get; set; }
        public string TypeName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string StoreName { get; set; }
        public string UnitCode { get; set; }
        public string UnitDesc { get; set; }
        public bool? IsHazardous { get; set; }
        public string? HazardTypeName { get; set; }
        public int AvailableQuantity { get; set; }
        public string BatchNo { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int DamagedQuantity { get; set; }
        public string DamageReason { get; set; }
        public int Count { get; set; }
        public string? Ended { get; set; }
        public string ItemNameAr { get; internal set; }
        public bool? Chemical { get; internal set; }
        public string RiskRating { get; internal set; }
        public string StateofMatter { get; internal set; }
        public int UnitId { get; internal set; }
        public string ItemDescription { get; internal set; }
        public int Id { get; internal set; }
        public string UnitOfmeasure { get; internal set; }
    }
}
