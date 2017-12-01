using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TutorialUniversity.Data;
using TutorialUniversity.Models;
using TutorialUniversity.Models.ContosoUniversity.Models;

namespace TutorialUniversity.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly SchoolContext _context;

        public DepartmentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var schoolContext = _context.Departments.Include(d => d.Administrator);
            return View(await schoolContext.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Administrator)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            ViewData["InstructorID"] = new SelectList(_context.Instructors, nameof(Instructor.ID), nameof(Instructor.FullName));
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DepartmentID,Name,Budget,StartDate,InstructorID")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InstructorID"] = new SelectList(_context.Instructors, nameof(Instructor.ID), nameof(Instructor.FullName), department.InstructorID);
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var depWithXmin = await _context.Departments.AsNoTracking()
                .Select(d => new
                {
                    Department = d,
                    xmin = EF.Property<uint>(d, "xmin"),
                    ShadowTest = EF.Property<string>(d, "ShadowTest"),
                })
            .Where(d => d.Department.DepartmentID == id)
            .SingleOrDefaultAsync();

            var dep = await _context.Departments.AsNoTracking().Where(d => d.DepartmentID == id).SingleOrDefaultAsync();
            var attachedEntry = _context.Attach(dep);
            var s1 = attachedEntry;
            attachedEntry.Property("xmin").OriginalValue = depWithXmin.xmin;
            //attachedEntry.Property("xmin").CurrentValue = depWithXmin.xmin;
            dep.Budget += 1;
            var s2 = _context.Entry(dep).State;
            await _context.SaveChangesAsync();
            var s3 = _context.Entry(dep).State;


            var departmentWithXmin = await _context.Departments
                .AsNoTracking()
                .Select(d => new
                {
                    Department = d,
                    xmin = EF.Property<uint>(d, "xmin"),
                    ShadowTest = EF.Property<string>(d, "ShadowTest"),
                })
                //.AsNoTracking()
                .SingleOrDefaultAsync(m => m.Department.DepartmentID == id);


            //var department = await _context.Departments.AsNoTracking().SingleOrDefaultAsync(m => m.DepartmentID == id);
            if (departmentWithXmin == null)
            {
                return NotFound();
            }
            ViewData["InstructorID"] = new SelectList(
                _context.Instructors,
                nameof(Instructor.ID),
                nameof(Instructor.FullName),
                departmentWithXmin.Department.InstructorID);
            ViewData["RowVersion"] = departmentWithXmin.xmin;

            var props = _context
                .Entry(departmentWithXmin.Department)
                .Properties
                .Select(p => new { p.Metadata.Name, p.CurrentValue, p.OriginalValue }).ToList();

            var c = _context.Entry(departmentWithXmin.Department).Property("ShadowTest").CurrentValue;

            //departmentWithXmin.Department.Budget += 1;
            //await _context.SaveChangesAsync();

            return View(departmentWithXmin.Department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, uint rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departmentToUpdate = await _context.Departments.Include(i => i.Administrator).SingleOrDefaultAsync(m => m.DepartmentID == id);

            if (departmentToUpdate == null)
            {
                Department deletedDepartment = new Department();
                await TryUpdateModelAsync(deletedDepartment);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department was deleted by another user.");
                ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "FullName", deletedDepartment.InstructorID);
                return View(deletedDepartment);
            }

            _context.Entry(departmentToUpdate).Property("xmin").OriginalValue = rowVersion;

            if (await TryUpdateModelAsync<Department>(
                departmentToUpdate,
                "",
                s => s.Name, s => s.StartDate, s => s.Budget, s => s.InstructorID))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Department)exceptionEntry.Entity;
                    var databaseEntry = await exceptionEntry.GetDatabaseValuesAsync();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The department was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Department)databaseEntry.ToObject();

                        if (databaseValues.Name != clientValues.Name)
                        {
                            ModelState.AddModelError("Name", $"Current value: {databaseValues.Name}");
                        }
                        if (databaseValues.Budget != clientValues.Budget)
                        {
                            ModelState.AddModelError("Budget", $"Current value: {databaseValues.Budget:c}");
                        }
                        if (databaseValues.StartDate != clientValues.StartDate)
                        {
                            ModelState.AddModelError("StartDate", $"Current value: {databaseValues.StartDate:d}");
                        }
                        if (databaseValues.InstructorID != clientValues.InstructorID)
                        {
                            Instructor databaseInstructor = await _context.Instructors.SingleOrDefaultAsync(i => i.ID == databaseValues.InstructorID);
                            ModelState.AddModelError("InstructorID", $"Current value: {databaseInstructor?.FullName}");
                        }

                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you got the original value. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to edit this record, click "
                                + "the Save button again. Otherwise click the Back to List hyperlink.");

                        ViewData["RowVersion"] = exceptionEntry.Property("xmin").CurrentValue;

                        //_context.Entry(departmentToUpdate).Property("xmin").CurrentValue = exceptionEntry.Property("xmin").CurrentValue;
                        //departmentToUpdate.RowVersion = (byte[])databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }
            ViewData["InstructorID"] = new SelectList(_context.Instructors, "ID", "FullName", departmentToUpdate.InstructorID);
            return View(departmentToUpdate);
        }

        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Administrator)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.DepartmentID == id);
            if (department == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Department department)
        {
            try
            {
                if (await _context.Departments.AnyAsync(m => m.DepartmentID == department.DepartmentID))
                {
                    _context.Departments.Remove(department);
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException /* ex */)
            {
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = department.DepartmentID });
            }
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentID == id);
        }
    }
}
