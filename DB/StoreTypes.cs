using System.ComponentModel.DataAnnotations;

namespace LabMaterials.DB
{
    public class StoreTypes
    {
        [Key]
        public int StoreTypeId { get; set; }


        public String StoreType { get; set; } = null!;
    }
}
