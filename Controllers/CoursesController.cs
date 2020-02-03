﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LexiconLMS.Core.Models;
using LexiconLMS.Data;
using LexiconLMS.Core.Repository;
using LexiconLMS.Core.ViewModels;

namespace LexiconLMS.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICourseRepository _courseRepository;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
            _courseRepository = new CourseRepository(_context);
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            return View(await _context.Courses.ToListAsync());
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return View(await _courseRepository.GetCourseViewModel(id));
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,Description")] Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // POST: Modules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateModule([Bind("Id, CourseId, Name, Description, StartDate, EndDate")] Module module)
        {
            if (ModelState.IsValid)
            {
                _context.Add(module);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Edit", new { id = module.CourseId });
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
           

            if (id == null)
            {
                return NotFound();
            }

            var course = await _courseRepository.GetCourseViewModel(id);
            if (course == null)
            {
                return NotFound();
            }
            //var model = new CourseEditViewModel()
            //{
            //    course = course,
            //    SystemUsersList = GetSelectedSystemUser(_context.SystemUsers)
            //};
            return View(course);
            //return View(model);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,Description, SystemUsersList")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }


        public async Task<IActionResult> Filter(string courseName)
        {
            var model = await _context.Courses.ToListAsync();

            model = string.IsNullOrWhiteSpace(courseName) ?
                model :
                model.Where(p => p.Name.ToLower().Contains(courseName.ToLower())).ToList();

            return View(nameof(Index), model);
        }

        public IActionResult UsersForCourseEditPartial(IEnumerable<SystemUser> users)
        {
            var result = new List<SystemUserViewModel>();

            foreach (var user in users)
            {
                var model = new SystemUserViewModel
                {
                    Name = user.Name,
                    Id = user.Id
                };
                result.Add(model);
            }

            return PartialView(result);
        }

        public IEnumerable<SelectListItem> GetSelectedSystemUser(IEnumerable<SystemUser> SystemUsers)
        {
            List<SelectListItem> list = null;
                var query = (from ca in SystemUsers
                             //where ca.AccountID == AccountID
                             orderby ca.Name
                             select new SelectListItem { Text = ca.Name, Value = ca.Id }).Distinct();
                list = query.ToList();
            return list;
        }

        public ActionResult ModalPopUp()
        {
            return View();
        }
    }
}
