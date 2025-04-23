namespace LabMaterials.dtos
{
    public class DisbursementInfo
    {
        public int DisbursementRequestId {  get; set; }
        public string RequesterName {  get; set; }
        public string RequestingPlace { get; set; } 
        public DateTime ReqReceivedAt { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public string InventoryBalanced { get; set; }
        public string ItemCode { get; set; }
        public string ItemTypeCode { get; set; }
        public int? Quantity { get; set; }
        public string? StoreName { get; set; }
        public string? ItemName { get; set; }
    }
}
