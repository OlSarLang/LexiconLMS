﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LexiconLMS.Core.Services;
using LexiconLMS.Core.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LexiconLMS.Controllers
{
    public class DocumentController : Controller
    {
        private IDocumentService _documentService;
        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ActivityDocumentUpload(ActivityDocumentUploadViewModel model)
        {
            if (model.FormFile==null)
            {
                return RedirectToAction("Error", "Home");

            }

            var success = await _documentService.SaveActivityDocumentToFile(model);
            if (success)
                return RedirectToAction(@"Details", "Activity", new { Id = model.ActivityId });
            else
                return RedirectToAction("Error", "Home");
        }



        public async Task<IActionResult> CourseDocumentUpload(CourseDocumentUploadViewModel model)
        {
            if (model.FormFile == null)
            {
                return RedirectToAction("Error", "Home");

            }

            //var success = await _documentService.SaveCourseDocumentToFile(model.FormFile,model.UserId,model.CourseId);
            var success = await _documentService.SaveCourseDocumentToFile(model);
            if (success)
                return RedirectToAction(@"Details", "Courses", new { Id = model.CourseId });
            else
                return RedirectToAction("Error", "Home");
        }




        [HttpGet]
        public IActionResult UploadUserDocument(string userId)
        {
            return View(new UserDocumentUploadViewModel { UserId = userId });
        }


        public async Task<IActionResult> UploadUserDocument(IFormFile formFile, string userId)
        {
            var success = await _documentService.SaveUserDocumentToFile(formFile, userId);
            if (success)
                return RedirectToAction(@"Details", "SystemUser", new { Id = userId });
            else
                return RedirectToAction("Error", "Home");
        }


        [HttpGet]
        public IActionResult UploadCourseDocument(string userId)
        {
            return View(new UserDocumentUploadViewModel { UserId = userId });
        }


        //public async Task<IActionResult> UploadCourseDocument(IFormFile formFile, string userId, int courseId)
        //{
        //    //if (formFile!="")
        //    {
        //        var result = await _documentService.SaveCourseDocumentToFile(CourseDocumentUploadViewModel model);

        //       // var result = await _documentService.SaveCourseDocumentToFile(formFile, userId, courseId);
        //        return RedirectToAction(@"Details", "SystemUser", new { Id = userId });
        //    }

        //}
    }
}