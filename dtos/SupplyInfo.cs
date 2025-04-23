namespace LabMaterials.dtos
{
    public class SupplyInfo
    {
        public int SupplyId {  get; set; }
        public string ItemName {  get; set; }
        public DateTime ReceivedAt {  get; set; }
        public string SupplierName { get; set; }
        public string QuantityReceived { get; set; }
        public string InvoiceNumber { get; set; }
        public string PurchaseOrderNo { get; set; }
        public bool InventoryBalanced { get; set; }
        public string StoreName { get; set; }
        public string RoomName { get; set; }
        public string ShelfNumber { get; set; }
        public DateTime ExpiryDate { get; set; }

        public string ItemCode { get; set; }
        public string ItemType { get; set; }
    }
}
