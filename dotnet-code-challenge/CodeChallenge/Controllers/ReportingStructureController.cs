using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/reporting")]
    public class ReportingStructureController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public ReportingStructureController(ILogger<CompensationController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpGet("{id}", Name = "getReportingStructure")]
        public IActionResult GetReportingStructure(String id)
        {
            _logger.LogDebug($"Received reporting structure get request for '{id}'.");
            var employee = _employeeService.GetById(id);

            if (employee == null)
                return NotFound();

            var reportingNum = GetReportingAmount(employee);

            var reportingStructure = new ReportingStructure
            {
                Employee = employee,
                NumberOfReports = reportingNum
            };

            return Ok(reportingStructure);
        }


        private int GetReportingAmount(Employee employee)
        {
            var result = employee.DirectReports.Count();
            foreach(var x in  employee.DirectReports)
            {
                var temp = _employeeService.GetById(x.EmployeeId);
                if (temp.DirectReports.Any())
                {
                    result += GetReportingAmount(temp);
                }
            }

            return result;
        }
    }
}
