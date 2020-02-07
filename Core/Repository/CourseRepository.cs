﻿using LexiconLMS.Core.Models;
using LexiconLMS.Core.Services;
using LexiconLMS.Core.ViewModels;
using LexiconLMS.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LexiconLMS.Core.Repository
{
    public class CourseRepository : ICourseRepository
    {
        private ApplicationDbContext _context { get; }
        private IUserService _userService;

        public CourseRepository(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }
        public void AddModule(Module module)
        {
            throw new NotImplementedException();
        }

        public void RemoveModule(Module module)
        {
            throw new NotImplementedException();
        }

        public async Task<CourseViewModel> GetCourseViewModel(int? id)
        {
            var model = new CourseViewModel
            {
                Course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id)
            };
            if (model.Course == null)
            {
                return NotFound();
            }

            model.Modules = await _context.Modules.Where(m => m.CourseId == id).ToListAsync();            

            List<SystemUserViewModel> userViewModels = _userService.GetSystemUserViewModels(id);

            model.SystemUsers = userViewModels;

            model.Module = new Module
            {
                CourseId = model.Course.Id,
            };

            return model;
        }

        //Used for ViewComponent
        public async Task<IEnumerable<Module>> GetAllCourseModulesAsync(int courseId)
        {
            return await _context.Modules.Where(m => m.CourseId == courseId).ToListAsync();
        }

        public async Task<ModuleViewModel> GetModuleViewModel(int? id)
        {
            var model = new ModuleViewModel
            {
                Module = await _context.Modules.FirstOrDefaultAsync(m => m.Id == id)
            };

            if (model.Module == null)
            {
                return NotFoundModule();
            }

            model.Activities = await _context.Activities.Where(m => m.ModuleId == id).ToListAsync();

            model.Activity = new Activity
            {
                ModuleId = model.Module.Id
            };

            return model;
        }

        //Used for ViewComponent
        public async Task<IEnumerable<Activity>> GetAllModuleActivitiesAsync(int moduleId)
        {
            return await _context.Activities.Where(a => a.ModuleId == moduleId).ToListAsync();
        }

        public async Task<Activity> GetActivity(int? id)
        {
            var activity = await _context.Activities.Include(a => a.Documents).Where(m => m.Id == id).FirstOrDefaultAsync();

            foreach (var documentActivity in activity.Documents)
            {
                documentActivity.Document = await _context.Documents.FirstOrDefaultAsync(d => d.Id == documentActivity.DocumentId);
            }

            /* Alternate solution:
             * var activity = await _context.Activities.FirstOrDefaultAsync(m => m.Id == id);

            activity.Documents = await _context.DocumentsActivities.Include(da => da.Document).Where(da => da.ActivityId == activity.Id).ToListAsync();*/

            return activity;
        }

        private ModuleViewModel NotFoundModule()
        {
            throw new NotImplementedException();
        }
        private CourseViewModel NotFound()
        {
            throw new NotImplementedException();
        }

        
    }
}
