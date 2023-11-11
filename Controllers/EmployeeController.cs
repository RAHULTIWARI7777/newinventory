using AutoMapper;
using backend.Core.Context;
using backend.Core.Dtos.Employee;
using backend.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using backend.Core.ProductRequest;
using Core.Helper;
using Microsoft.AspNetCore.Hosting;
using backend.Core.ProductRequests;
using OfficeOpenXml;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {


        private ApplicationDbContext _context { get; }
        private IMapper _mapper { get; }
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmployeeController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;

        }



        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreateDto dto)
        {
            Employee newEmployee = _mapper.Map<Employee>(dto);
            await _context.Employees.AddAsync(newEmployee);
            await _context.SaveChangesAsync();

            return Ok("Employee Created Successfully");
        }


        [HttpGet]
        [Route("Get")]

        public async Task<ActionResult<IEnumerable<EmployeeGetDto>>> GetEmployees()
        {
            var employees = await _context.Employees.OrderByDescending(q => q.CreatedAt).ToListAsync();
            var convertedEmployees = _mapper.Map<IEnumerable<EmployeeGetDto>>(employees);

            return Ok(convertedEmployees);
        }





        [HttpPost("upload")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult> Upload(CancellationToken ct)
        {
            if (Request.Form.Files.Count == 0) return NoContent();

            var file = Request.Form.Files[0];
            var filePath = SaveFile(file);

            // load product requests from an Excel file
            var productRequests = ExcelHelper.Import<EmployeeRequest>(filePath);

            // Save employee view models to the database
            foreach (var productRequest in productRequests)
            {
                var employee = new Employee
                {

                    Name = productRequest.Name,



                    ID = productRequest.ID,


                    CreatedAt = productRequest.CreatedAt = DateTime.Now,

                    UpdatedAt = productRequest.UpdatedAt = DateTime.Now,
                    IsActive = productRequest.IsActive = true,


                };
                await _context.Employees.AddAsync(employee, ct);

            }

            await _context.SaveChangesAsync(ct);

            return Ok();
        }






        // Save the uploaded file into wwwroot/uploads folder
        private string SaveFile(IFormFile file)
        {
            if (file.Length == 0)
            {
                throw new BadHttpRequestException("File is empty.");
            }
            var extension = Path.GetExtension(file.FileName);

            var webRootPath = _webHostEnvironment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRootPath))
            {
                webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            var folderPath = Path.Combine(webRootPath, "uploads");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = $"{Guid.NewGuid()}.{extension}";
            var filePath = Path.Combine(folderPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(stream);

            return filePath;
        }


        [HttpGet("DownloadExcel")]

        public IActionResult DownloadExcel()
        {
           
            var employees = _context.Employees.OrderByDescending(q => q.CreatedAt).ToList();
            var convertedEmployees = _mapper.Map<IEnumerable<EmployeeGetDto>>(employees);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employees");

                // Add header row dynamically based on properties of EmployeeGetDto
                var headers = typeof(EmployeeGetDto).GetProperties()
                    .Select((prop, index) => new { Name = prop.Name, Index = index + 1 });

                foreach (var header in headers)
                {
                    worksheet.Cells[1, header.Index].Value = header.Name;
                }

                // Add data rows
                int row = 2;
                foreach (var employee in convertedEmployees)
                {
                    for (int i = 1; i <= headers.Count(); i++)
                    {
                        var prop = typeof(EmployeeGetDto).GetProperty(headers.ElementAt(i - 1).Name);
                        var value = prop.GetValue(employee, null);
                        worksheet.Cells[row, i].Value = value;
                    }
                    row++;
                }

                // Set content type and return the Excel file
                var content = package.GetAsByteArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
            }






        }
    }
}

