using backend.Core.Entities;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using backend.Core.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace backend.Core.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        public DbSet<Employee> Employees { get; set; }
        public DbSet<HardwareInfo> HardwareInfos { get; set; }
        public DbSet<EmployeeHardwareInfo> EmployeeHardwareInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define the one-to-many relationship between Employee and EmployeeHardwareInfo
            modelBuilder.Entity<Employee>()
                .HasMany(e => e.EmployeeHardwareInfos)
                .WithOne(ehi => ehi.Employee)
                .HasForeignKey(ehi => ehi.EmployeeId);

            // Define the one-to-many relationship between HardwareInfo and EmployeeHardwareInfo
            modelBuilder.Entity<HardwareInfo>()
                .HasMany(hi => hi.EmployeeHardwareInfos)
                .WithOne(ehi => ehi.HardwareInfo)
                .HasForeignKey(ehi => ehi.HardwareInfoId);


            modelBuilder.Entity<HardwareInfo>()
                .Property(hardwareInfo => hardwareInfo.Type)
                .HasConversion<string>();

            var typeConverter = new ValueConverter<HardwareType, string>(
                v => v.ToString(),
                v => (HardwareType)Enum.Parse(typeof(HardwareType), v)
                );

            modelBuilder.Entity<HardwareInfo>()
             .Property(hardwareInfo => hardwareInfo.Type)
             .HasConversion(typeConverter);

        }


    }



    }

