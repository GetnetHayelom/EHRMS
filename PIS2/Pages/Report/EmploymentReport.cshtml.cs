using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PIS2.Data;
using PIS2.Enums;
using PIS2.Models;
using PIS2.Views;

namespace PIS2.Pages.Report
{
    public class EmploymentReportModel : PageModel
    {
        private readonly PISContext _context;

        public EmploymentReportModel(PISContext context)
        {
            _context = context;
        }

        public List<EmployeeDetailView> employmentModel { get;set; } = default!;
        public List<EmployeeDetailView> groupedView { get; set; } = default!;
        public IList<departmentModel> Departments { get; set; } = default!;
        public IList<employmentTypeModel> EmploymentTypes { get; set; } = default!;
        public IList<jobModel> JobTitles { get; set; } = default!;
        public IList<workSiteModel> WorkLocations { get; set; } = default!;
        public IList<companyModel> Companies { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; } = DateTime.Now;
        public int totalCount { get; set; }
        public int filteredCount { get; set; }
        public int activeCount { get; set; }
        public int maleCount { get; set; }
        public int femaleCount { get; set; }
        public int managementCount { get; set; }

        public async Task OnGetAsync()
        {
            EmploymentTypes = await _context.EmploymentTypes.OrderBy(e => e.employmentTypeName).ToListAsync();
            Departments = await _context.Departments.OrderBy(d => d.departmentName).ToListAsync();
            JobTitles = await _context.Jobs.OrderBy(j => j.jobTitle).ToListAsync();
            WorkLocations = await _context.WorkSites.OrderBy(w => w.workSiteName).ToListAsync();
            Companies = await _context.Companies.OrderBy(c => c.companyName).ToListAsync();
            employmentModel = await _context.EmployeeDetailViews
                .ToListAsync();
            StartDate =employmentModel.IsNullOrEmpty()? DateTime.MinValue : employmentModel.Min(e => e.EmploymentDate);
            totalCount = employmentModel.Count;
            filteredCount = employmentModel.Count;
                activeCount = employmentModel.Where(e => e.EmploymentStatus == mainStatus.Active).Count();
            femaleCount = employmentModel.Where(e => e.PersonGender == Gender.Female).Count();
            maleCount = employmentModel.Where(e => e.PersonGender == Gender.Male).Count();
            managementCount = employmentModel.Where(e => e.EmploymentPosition == EmploymentPositions.Management).Count();

            groupedView = _context.EmployeeDetailViews.Where(e => e.EmploymentStatus == mainStatus.Active).ToList();
        }
     
        public IActionResult OnGetFilter(int? department, int? jobTitle, int? empStatus, int? empType, int? company, int? workLoc, DateTime? dateStart, DateTime? dateEnd,  int? position,int? gender)
        {
            Console.WriteLine("the Date is " + dateStart);
            // Start with the full list of employees
            var employmentModel = _context.EmployeeDetailViews.AsQueryable(); // Using IQueryable to build a dynamic query

            
            // Apply filters based on the provided query parameters

            // Filter by company (if provided)
            if (company.HasValue && company != null)
            {
                employmentModel = employmentModel.Where(e => e.CompanyID == company);
                
            }
            
            // Filter by Department (if provided)
            if (department.HasValue && department !=null)
            { 
                employmentModel = employmentModel.Where(e => e.DepartmentID == department);
                
            }

            // Filter by Status (if provided)
            if (empStatus.HasValue)
            {
                employmentModel = employmentModel.Where(e => e.EmploymentStatus == (mainStatus) empStatus);
               
            }

            // Filter by Gender (if provided)
            if (gender.HasValue)
            {
                employmentModel = employmentModel.Where(e => e.PersonGender == (Gender)gender);

            }
            // Filter by Status (if provided)
            if (position.HasValue)
            {
                employmentModel = employmentModel.Where(e => e.EmploymentPosition == (EmploymentPositions)position);

            }

            // Filter by Job Title (if provided)
            if (jobTitle.HasValue)
            {
               
                employmentModel = employmentModel.Where(e => e.JobID == jobTitle);
            }
            // Filter by type (if provided)
            if (empType.HasValue && empType != null)
            {
               
                employmentModel = employmentModel.Where(e => e.EmploymentTypeID == empType);
            }
            
            
            // Filter by workloc (if provided)
            if (workLoc.HasValue && workLoc != null)
            {              
                employmentModel = employmentModel.Where(e => e.WorkSiteID == workLoc);
            }
            // Filter by Start TIme (if provided)
            if (dateStart.HasValue && dateStart != null)
            {
                employmentModel = employmentModel
                    .Where(e => e.EmploymentDate >= dateStart);
            }
            // Filter by end TIme (if provided)
            if (dateEnd.HasValue && dateEnd != null)
            {
                employmentModel = employmentModel
                    .Where(e => e.EmploymentDate <= dateEnd);
            }

            // Execute the query and get the filtered results
            var filteredEmployees = employmentModel.OrderBy(e => e.GivenID).ToList();
            filteredCount = filteredEmployees.Count;
            activeCount = filteredEmployees.Where(e => e.EmploymentStatus == mainStatus.Active).Count();
            femaleCount = filteredEmployees.Where(e => e.PersonGender == Gender.Female).Count();
            maleCount = filteredEmployees.Where(e => e.PersonGender == Gender.Male).Count();
            managementCount = filteredEmployees.Where(e => e.EmploymentPosition == EmploymentPositions.Management).Count();
            // Generate the table HTML
            var tableHtml = string.Join("",
    filteredEmployees
        .GroupBy(e => e.CompanyName) // Group by company
        .Select(companyGroup =>
        {
            // Company row
            var companyRow = $"<tr class='table-primary fw-bold'>" +
                             $"<td colspan='7'>Company: {companyGroup.Key} — Total Employees: {companyGroup.Count()}</td></tr>";

            // For each department inside this company
            var departmentRows = string.Join("",
                companyGroup
                    .GroupBy(e => e.DepartmentName)
                    .Select(deptGroup =>
                    {
                        // Department row
                        var deptRow = $"<tr class='table-secondary fw-bold'>" +
                                      $"<td colspan='7'>Department: {deptGroup.Key} — Total Employees: {deptGroup.Count()}</td></tr>";

                        // Employee rows
                        var employeeRows = string.Join("",
                            deptGroup.Select(e =>
                            {
                                var url = Url.Page("Details", new { id = e.EmploymentID });

                                return $"<tr onclick=\"location.href='{url}'\" style=\"cursor:pointer;\">" +
                                       $"<td><a href='/Employment/Details?id={e.EmploymentID}' class='text-decoration-none text-dark'>{e.GivenID}</a></td>" +
                                       $"<td><a href='/Employment/Details?id={e.EmploymentID}' class='text-decoration-none text-dark'>{e.FullName}</a></td>" +
                                       $"<td>{e.EmploymentDate.ToShortDateString()}</td>" +
                                       $"<td>{e.EmploymentStatus}</td>" +
                                       $"<td>{e.EmploymentTypeName}</td>" +
                                       $"<td>{e?.JobTitle ?? "N/A"}</td></tr>";
                            })
                        );

                        return deptRow + employeeRows;
                    })
            );

            return companyRow + departmentRows;
        })
);




            filteredCount = employmentModel.Count();
            // Return the generated HTML
            //return Content(tableHtml);
            return new JsonResult(new { tableHtml, filteredCount, activeCount, femaleCount, maleCount, managementCount });
        }
    }
}
