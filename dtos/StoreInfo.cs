namespace LabMaterials.dtos
{
    public class StoreInfo
    {
        public int StoreId {  get; set; }
        public string StoreNumber {  get; set; }
        public string StoreName { get; set; } 
        public string ShelfNumbers { get; set; }
        public string[] Shelves { get; set; }
        public List<RoomInfo> Rooms { get; set; }
    }
}
