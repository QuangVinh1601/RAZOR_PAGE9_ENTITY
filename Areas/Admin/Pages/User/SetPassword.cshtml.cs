using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RAZOR_PAGE9_ENTITY.Models;

namespace RAZOR_PAGE9_ENTITY.Areas.Admin.Pages.User
{
    public class SetPasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public SetPasswordModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu mới")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Nhập lại mật khẩu")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }
        public AppUser user { get; set; }

        public async Task<IActionResult> OnGetAsync(string userid)
        {
            if (string.IsNullOrEmpty(userid))
            {
                return NotFound("Không tìm thấy User");
            }

            user = await _userManager.FindByIdAsync(userid);
            if (user == null)
            {
                return NotFound($"Không tìm thấy User với id {userid}");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string userid)
        {
            if(String.IsNullOrEmpty(userid))
            {
                return NotFound("Không tìm thấy User");
            }
            user = await  _userManager.FindByIdAsync(userid);
            if(user == null)
            {
                return NotFound($"Không tìm thấy User với id: {userid}");
            }
            if(!ModelState.IsValid)
            {
                return Page();
            }

            await _userManager.RemovePasswordAsync(user);
            var result =  await _userManager.AddPasswordAsync(user, Input.NewPassword);
            if (result.Succeeded) 
            {
                StatusMessage = "Cập nhập mật khẩu thành công";
                return RedirectToPage("./Index");
            }
            else
            {
                var errors =  result.Errors.ToList();
                foreach ( var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }
    }
}
