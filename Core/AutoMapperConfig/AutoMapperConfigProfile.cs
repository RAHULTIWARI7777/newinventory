using AutoMapper;
using backend.Core.Dtos.Employee;
using backend.Core.Dtos.EmployeeHardwareInfo;
using backend.Core.Dtos.HardwareInfo;
using backend.Core.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace backend.Core.AutoMapperConfig
{
    public class AutoMapperConfigProfile : Profile
    {
        public AutoMapperConfigProfile()


        {


            //Employee
            CreateMap<EmployeeCreateDto, Employee>();
            CreateMap<Employee, EmployeeGetDto>();



            //HardwareInfo
            CreateMap<HardwareInfoCreateDto, HardwareInfo>();
            CreateMap<HardwareInfo, HardwareInfoGetDto>();



            //EmployeeHardwareInfo
            CreateMap<EmployeeHardwareInfoCreateDto, EmployeeHardwareInfo>();
            CreateMap<EmployeeHardwareInfo, EmployeeHardwareInfoGetDto>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.Name))
                .ForMember(dest => dest.HardwareInfoType, opt => opt.MapFrom(src => src.HardwareInfo.Type));




            /*       // Company
                   CreateMap<CompanyCreateDto, Company>();
                   CreateMap<Company, CompanyGetDto>();

                   // Job
                   CreateMap<JobCreateDto, Job>();
                   CreateMap<Job, JobGetDto>()
                       .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name));

                   // Candidate
                   CreateMap<CandidateCreateDto, Candidate>();
                   CreateMap<Candidate, CandidateGetDto>()
                       .ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job.Title)); */
        }
    }
}
