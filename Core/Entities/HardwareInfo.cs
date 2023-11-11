using backend.Core.Enums;

namespace backend.Core.Entities
{
    public class HardwareInfo : BaseEntity
    {
        public HardwareType Type { get; set; }

        //Relations

        public ICollection<EmployeeHardwareInfo> EmployeeHardwareInfos { get; set; }


    }
}
