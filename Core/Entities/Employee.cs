namespace backend.Core.Entities
{
    public class Employee : BaseEntity
    {
        public string Name { get; set; }

        //Relations
        public ICollection<EmployeeHardwareInfo> EmployeeHardwareInfos { get; set; }
    }
}
