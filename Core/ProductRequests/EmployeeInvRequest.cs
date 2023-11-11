using backend.Core.Enums;

namespace backend.Core.ProductRequests
{
    public class EmployeeInvRequest
    {
        public string Remarks { get; set; }

        public long ID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public long HardwareInfoId { get; set; }
        public HardwareType HardwareInfoType { get; set; }
    }
}
