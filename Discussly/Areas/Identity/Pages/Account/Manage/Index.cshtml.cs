// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Discussly.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http; 

namespace Discussly.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<DiscusslyUser> _userManager;
        private readonly SignInManager<DiscusslyUser> _signInManager;

        public IndexModel(
            UserManager<DiscusslyUser> userManager,
            SignInManager<DiscusslyUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ProfilePicFileName { get; set; } // Add this property

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Profile Picture")]
            public IFormFile ProfilePic { get; set; }

            [Display(Name = "Display Name")]
            public string Name { get; set; }
        }

        private async Task LoadAsync(DiscusslyUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;
            ProfilePicFileName = user.ProfilePic;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Name = user.Name 
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

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            // Update Name if changed
            if (Input.Name != user.Name)
            {
                user.Name = Input.Name;
                await _userManager.UpdateAsync(user);
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostProfilePicAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
            string fileExtension = Path.GetExtension(Input.ProfilePic.FileName).ToLower();

            // Validate file type
            if (!allowedExtensions.Contains(fileExtension))
            {
                StatusMessage = "Invalid file type. Please upload an image.";
                return Page();
            }

            if (Input.ProfilePic != null && Input.ProfilePic.Length > 0)
            {
                var uploadsFolder = "./wwwroot/UserProfilePicture/";
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Delete old profile picture if it exists and is not the default
                if (!string.IsNullOrEmpty(user.ProfilePic) && user.ProfilePic != "NoProfilePic.png")
                {
                    var oldFilePath = Path.Combine(uploadsFolder, user.ProfilePic);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Generate unique file name
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Input.ProfilePic.FileName)}";
                if (fileName.Length > 230)
                {
                    fileName = fileName.Substring(0, 230);
                }
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save the new file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ProfilePic.CopyToAsync(fileStream);
                }

                // Update user profile
                user.ProfilePic = fileName;
                await _userManager.UpdateAsync(user);

                StatusMessage = "Profile picture updated.";
            }
            else
            {
                StatusMessage = "No file selected.";
            }

            return RedirectToPage();
        }
    }
}
