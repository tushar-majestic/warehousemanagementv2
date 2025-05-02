namespace LabMaterials.dtos
{
    public class SupplierInfo
    {
        public int SupplierId {  get; set; }
        public string SupplierName { get; set; } 
        public string ConatctNumber { get; set; } 
        public string SupplierType { get; set; }
        public string CoordinatorName { get; internal set; }
    }
}
