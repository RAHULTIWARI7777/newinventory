using backend.Core.Enums;

namespace backend.Core.Dtos.HardwareInfo
{
    public class HardwareInfoGetDto
    {
        public HardwareType Type { get; set; }
        public long ID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
