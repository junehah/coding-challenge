using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/compensation")]
    public class CompensationController :ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ICompensationService _compensationService;
        private readonly IEmployeeService _employeeService;

        public CompensationController(ILogger<CompensationController> logger,
            ICompensationService compensationService,
            IEmployeeService employeeService)
        {
            _logger = logger;
            _compensationService = compensationService;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateCompensation([FromBody] Compensation comp)
        {
            _logger.LogDebug($"Received compensation create request for '{comp.Id}'");

            var employee = _employeeService.GetById(comp.Id);
            if (employee == null)
                throw new Exception("Employee not found");

            var compensation = new Compensation
            {
                Id = comp.Id,
                Employee = employee,
                Salary = comp.Salary,
                EffectiveDate = comp.EffectiveDate
            };

            _compensationService.Create(compensation);

            return CreatedAtRoute("getCompensationById", new { Id = comp.Id }, compensation);
        }

        [HttpGet("{id}", Name = "getCompensationByEmployeeId")]
        public IActionResult GetCompensationByEmployeeId(String id)
        {
            _logger.LogDebug($"Received compensation get request for '{id}'");

            var compensation = _compensationService.GetById(id);

            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }
    }
}
