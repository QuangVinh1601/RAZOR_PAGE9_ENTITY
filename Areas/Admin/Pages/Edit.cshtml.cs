﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RAZOR_PAGE9_ENTITY.Areas.Admin.Pages
{
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager) : base(roleManager)
        {
        }
        public class InputModel
        {
            [StringLength(256, MinimumLength =6, ErrorMessage ="{0} có độ dài từ {2} đến {1}")]
            [Required(ErrorMessage ="{0} bắt buộc phải nhập")]
            [Display(Name = "Tên role cần cập nhật" )]
            public string Name { get; set; }
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public IdentityRole Role { get; set; }

        public async Task<IActionResult> OnGet(string roleid)
        {
            if(roleid == null)
            {
                return NotFound("Không tìm thấy role phù hợp");
            }
            Role =   await _roleManager.FindByIdAsync(roleid);
            if(Role != null)
            {
                Input = new InputModel
                {
                    Name = Role.Name,
                };
                return Page();
            }
            return NotFound("Không tìm thấy role phù hợp");
            
        }
        public async Task<IActionResult> OnPost(string roleid)
        {
            if(roleid == null)
            {
                return NotFound("Không tìm thấy Role ");
            }
            Role =  await _roleManager.FindByIdAsync(roleid);
            if(Role == null)
            {
                return NotFound("Không tìm thấy Role phù hợp");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            Role.Name = Input.Name;
            var result = await _roleManager.UpdateAsync(Role);
            if( result.Succeeded)
            {
                StatusMessage = $"Đã thay đổi role: {Role.Name}";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(error => ModelState.AddModelError(string.Empty, error.Description));
            }
            return Page();



        }
    }
    }

