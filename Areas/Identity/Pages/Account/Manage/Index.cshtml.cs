﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LexiconLMS.Core.Models;
using LexiconLMS.Core.Services;
using LexiconLMS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LexiconLMS.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<SystemUser> _userManager;
        private readonly SignInManager<SystemUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IDocumentService _documentService;

        public IndexModel(
            UserManager<SystemUser> userManager,
            SignInManager<SystemUser> signInManager,
            ApplicationDbContext context,
            IDocumentService documentService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _documentService = documentService;
        }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Id { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Role { get; set; }
            [EmailAddress]
            public string Email { get; set; }
            [StringLength(30)]
            [Required]
            public string Name { get; set; }


            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            public List<SelectListItem> RoleItemList { get; set; }
            public IFormFile FormFile { get; set; }
        }

        private async Task LoadAsync(SystemUser user)
        {
            string name = user.Name, userName = user.UserName, phoneNumber = user.PhoneNumber;
            Id = user.Id;
            var email = await _userManager.GetPhoneNumberAsync(user);

            List<SelectListItem> selectListItems = new List<SelectListItem>();

            var roles = await _context.Roles.ToListAsync();
            var currentRoleId = await _context.UserRoles.Where(r => r.UserId.Equals(user.Id)).Select(r => r.RoleId).FirstOrDefaultAsync();

            foreach (var role in roles)
            {

                if (currentRoleId == null)
                {
                    throw new Exception("Failed to find the current user's role.");
                }
                var selectItem = new SelectListItem
                {
                    Text = role.Name,
                    Selected = currentRoleId.Equals(role.Id)
                };
                selectListItems.Add(selectItem);
            }
            Username = userName;
            
            Input = new InputModel
            {
                Name = user.Name,
                PhoneNumber = phoneNumber,
                RoleItemList = selectListItems
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var selectedRole = Input.RoleItemList.Where(r => r.Selected).Select(r => r.Text).FirstOrDefault();
            var currentRoleId = await _context.UserRoles.Where(r => r.UserId == user.Id).Select(r => r.UserId).FirstOrDefaultAsync();
            var currentRole = await _context.Roles.Where(r => r.Id == currentRoleId).FirstOrDefaultAsync();
            var name = user.Name;
            if (Input.Name != name)
            {
                user.Name = Input.Name;
                var setNameResult = await _userManager.UpdateAsync(user);
                if (!setNameResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred changing name for user with ID '{userId}'.");
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (!currentRole.Name.Equals(selectedRole))
            {
                var roleAddResult = await _userManager.AddToRoleAsync(user, selectedRole);
                if (!roleAddResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred adding user with user ID '{userId}' to role '{currentRole.Name}'.");
                }
                var roleRemoveResult = await _userManager.RemoveFromRoleAsync(user, currentRole.Name);
                if (!roleRemoveResult.Succeeded)
                {
                    var userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException($"Unexpected error occurred removing user with user ID '{userId}' from role '{currentRole.Name}'.");
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        //public async Task<IActionResult> DocumentUpload()
        //{
        //    StatusMessage = "Document successfully uploaded.";
        //    return RedirectToPage(nameof(OnPostAsync));
        //}
    }
}
