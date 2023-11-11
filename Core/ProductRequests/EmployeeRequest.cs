namespace backend.Core.ProductRequests
{
    public class EmployeeRequest
    {

        public string Name { get; set; }

        public long ID { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}
