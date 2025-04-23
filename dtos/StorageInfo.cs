namespace LabMaterials.dtos
{
    public class StorageInfo
    {
        public int StorageId {  get; set; }
        public string ItemName {  get; set; }
        public string StoreNumber { get; set; } 
        public string StoreName { get; set; }
        public string ShelfNumber { get; set; }
        public string AvailableQuantity { get; set; } 
        public DateTime? ExpiryDate { get; set; } 
    }
}
