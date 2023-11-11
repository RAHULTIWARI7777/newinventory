using AutoMapper;
using backend.Core.Context;
using backend.Core.Dtos.Employee;
using backend.Core.Dtos.HardwareInfo;
using backend.Core.Entities;
using backend.Core.ProductRequest;
using Core.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Core.Enums;
using OfficeOpenXml;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HardwareInfoController : ControllerBase
    {
        private ApplicationDbContext _context { get; }
        private IMapper _mapper { get; }

        private readonly IWebHostEnvironment _webHostEnvironment;


        public HardwareInfoController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpPost]
        [Route("Create")]

        public async Task<IActionResult> CreateHardwareInfo([FromBody] HardwareInfoCreateDto dto)
        {
            HardwareInfo newHardwareInfo = _mapper.Map<HardwareInfo>(dto);
            await _context.HardwareInfos.AddAsync(newHardwareInfo);
            await _context.SaveChangesAsync();

            return Ok("HardwareInfo Created Successfully");
        }

        [HttpGet]
        [Route("Get")]

        public async Task<ActionResult<IEnumerable<HardwareInfoGetDto>>> GetHardwareInfos()
        {
            var hardwareInfos = await _context.HardwareInfos.OrderByDescending(q => q.CreatedAt).ToListAsync();
            var convertedhardwareInfos = _mapper.Map<IEnumerable<HardwareInfoGetDto>>(hardwareInfos);

            return Ok(convertedhardwareInfos);
        }


        [HttpPost("upload")]
        [DisableRequestSizeLimit]
        public async Task<ActionResult> Upload(CancellationToken ct)
        {
            if (Request.Form.Files.Count == 0) return NoContent();

            var file = Request.Form.Files[0];
            var filePath = SaveFile(file);

            // load product requests from an Excel file
            var productRequests = ExcelHelper.Import<ProductRequest>(filePath);

            // Save employee view models to the database
            foreach (var productRequest in productRequests)
            {
                var hardwareInfo = new HardwareInfo
                {

                    Type = productRequest.Type,
                    ID = productRequest.ID,

                    CreatedAt = productRequest.CreatedAt = DateTime.Now,
                    UpdatedAt = productRequest.UpdatedAt = DateTime.Now,
                    IsActive = productRequest.IsActive = true,


                };
                await _context.HardwareInfos.AddAsync(hardwareInfo, ct);

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

            var hardwareinfos = _context.HardwareInfos.OrderByDescending(q => q.CreatedAt).ToList();
            var convertedHardwareInfos = _mapper.Map<IEnumerable<HardwareInfoGetDto>>(hardwareinfos);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("HardwareInfos");

                // Add header row dynamically based on properties of HardwareInfoGetDto
                var headers = typeof(HardwareInfoGetDto).GetProperties()
                    .Select((prop, index) => new { Type = prop.Name, Index = index + 1 });

                foreach (var header in headers)
                {
                    worksheet.Cells[1, header.Index].Value = header.Type;
                }

                // Add data rows
                int row = 2;
                foreach (var hardwareInfo in convertedHardwareInfos)
                {
                    for (int i = 1; i <= headers.Count(); i++)
                    {
                        var prop = typeof(HardwareInfoGetDto).GetProperty(headers.ElementAt(i - 1).Type);
                        var value = prop.GetValue(hardwareInfo, null);
                        worksheet.Cells[row, i].Value = value;
                    }
                    row++;
                }

                // Set content type and return the Excel file
                var content = package.GetAsByteArray();
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "HardwareInfos.xlsx");
            }

        }
    }
}
