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

            var reportingNum = 0;

            if (employee.DirectReports is not null)
            {
                reportingNum = employee.DirectReports.Count();
                foreach (var x in employee.DirectReports)
                {
                    //update cuz this is disgusting.  
                    //any child employee does not return their direct reports.  
                    var tempEmployee = _employeeService.GetById(x.EmployeeId);
                    if (tempEmployee.DirectReports.Any())
                    {
                        reportingNum += x.DirectReports.Count();
                    }
                    //may not need this as needed as it might return an employee object for each directReport
                    //if (x.DirectReports is not null)
                    //    reportingNum += x.DirectReports.Count();
                }
            }

            var reportingStructure = new ReportingStructure
            {
                Employee = employee,
                NumberOfReports = reportingNum
            };

            return Ok(reportingStructure);
        }
    }
}
