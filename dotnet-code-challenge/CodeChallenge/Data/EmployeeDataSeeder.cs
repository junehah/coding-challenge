﻿using CodeChallenge.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Data
{
    public class EmployeeDataSeeder
    {
        private EmployeeContext _employeeContext;
        private const String EMPLOYEE_SEED_DATA_FILE = "resources/EmployeeSeedData.json";
        private const String COMPENSATION_SEED_DATA_FILE = "resources/CompensationSeedData.json";

        public EmployeeDataSeeder(EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
        }

        public async Task Seed()
        {
            if(!_employeeContext.Employees.Any())
            {
                List<Employee> employees = LoadEmployees();
                _employeeContext.Employees.AddRange(employees);

                await _employeeContext.SaveChangesAsync();
            }

            if (!_employeeContext.Compensations.Any())
            {
                List<Compensation> compensations = LoadCompensations();
                _employeeContext.Compensations.AddRange(compensations);

                await _employeeContext.SaveChangesAsync();
            }
        }

        private List<Employee> LoadEmployees()
        {
            using (FileStream fs = new FileStream(EMPLOYEE_SEED_DATA_FILE, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonReader jr = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();

                List<Employee> employees = serializer.Deserialize<List<Employee>>(jr);
                FixUpReferences(employees);

                return employees;
            }
        }

        private List<Compensation> LoadCompensations()
        {
            using (FileStream fs = new FileStream(COMPENSATION_SEED_DATA_FILE, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonReader jr = new JsonTextReader(sr))
            {
                JsonSerializer serializer = new JsonSerializer();

                List<Compensation> compensations = serializer.Deserialize<List<Compensation>>(jr);

                return compensations;
            }
        }

        //private void FixUpCompensationReferences(List<Compensation> compensations)
        //{
        //    var compensationIdRefMap = from compensation in compensations
        //                               select new { Id = compensation.id, CompensationRef = compensation };

        //    compensations.ForEach(compensation =>
        //    {
        //        var compensationId = compensation.id;
        //    })
        //}

        private void FixUpReferences(List<Employee> employees)
        {
            var employeeIdRefMap = from employee in employees
                                select new { Id = employee.EmployeeId, EmployeeRef = employee };

            employees.ForEach(employee =>
            {
                if (employee.DirectReports != null)
                {
                    var employeeId = employee.EmployeeId;
                    var referencedEmployees = new List<Employee>(employee.DirectReports.Count);
                    employee.DirectReports.ForEach(report =>
                    {
                        var referencedEmployee = employeeIdRefMap.First(e => e.Id == report.EmployeeId).EmployeeRef;
                        referencedEmployees.Add(referencedEmployee);
                    });
                    employee.DirectReports = referencedEmployees;
                }
            });
        }
    }
}
