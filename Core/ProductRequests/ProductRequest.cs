using backend.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace backend.Core.ProductRequest
{
    public class ProductRequest
    {
        public HardwareType Type { get; set; }

        [Key]
        public long ID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
