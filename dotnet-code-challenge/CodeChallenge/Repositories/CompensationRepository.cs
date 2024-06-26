﻿using CodeChallenge.Data;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CodeChallenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRepository(ILogger<ICompensationRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Compensation Add(Compensation compensation)
        {
            if (_employeeContext.Compensations.Any(c => c.Id == compensation.Id))
                throw new Exception("A record with this Id already exists in the Compensation Table");

            _employeeContext.Compensations.Add(compensation);

            return compensation;
        }

        public Compensation GetById(string id)
        {
            var compensation = _employeeContext.Compensations.Where(c=> c.Id == id).SingleOrDefault();
            if (compensation != null)
            {
                compensation.Employee = _employeeContext.Employees.Where(e => e.EmployeeId == id)
                    .Include(x => x.DirectReports).SingleOrDefault();
            }
            return compensation;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
    }
}
