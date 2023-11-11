namespace backend.Core.Dtos.Employee
{
    public class EmployeeGetDto
    {
        public string Name { get; set; }
        public long ID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
    }
}
