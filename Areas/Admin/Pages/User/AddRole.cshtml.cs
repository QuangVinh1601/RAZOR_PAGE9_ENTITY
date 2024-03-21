using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RAZOR_PAGE9_ENTITY.Models;

namespace RAZOR_PAGE9_ENTITY.Areas.Admin.Pages.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly MyBlogContext _context ;

        public AddRoleModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, MyBlogContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }



        [TempData]
        public string StatusMessage { get; set; }
        [BindProperty]
        [Display(Name ="Các role gán cho User")]
        public string[] RoleNames { get; set; }
       
        public AppUser user { get; set; }
        public SelectList allRoles { get; set; }
        public List<IdentityRoleClaim<string>> RoleClaims { get; set; }
        public List<IdentityUserClaim<string>> UserClaims { get; set; }

        public async Task<IActionResult> OnGetAsync(string userid)
        {
            if (string.IsNullOrEmpty(userid))
            {
                return NotFound("Không tìm thấy User");
            }
            user = await _userManager.FindByIdAsync(userid);
            if(user == null)
            {
                return NotFound($"Không tìm thấy User theo ID: {userid}");
            }
            RoleNames = (await  _userManager.GetRolesAsync(user)).ToArray<string>();
            List<string> roleNames =  await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);
            await GetClaim(userid);
            return Page();
        }
        async Task GetClaim(string userid)
        {
            //Lấy Claim của Role tương ứng với User
            RoleNames = (await _userManager.GetRolesAsync(user)).ToArray<string>();
            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);
            //Lấy ra Role của User
            var listrole = from r in _context.Roles
                           join ur in _context.UserRoles on r.Id equals ur.RoleId
                           where ur.UserId == userid
                           select r;
            //Lấy ra RoleClaim của User
            var listclaim = from rc in _context.RoleClaims
                            join r in listrole on rc.RoleId equals r.Id
                            select rc;
            RoleClaims = await listclaim.ToListAsync();

            //Lấy Claim của chính User đó
            UserClaims = await (from uc in _context.UserClaims
                          where uc.UserId == userid
                          select uc
                          ).ToListAsync();
        }
        public async Task<IActionResult> OnPostAsync(string userid)
        {
            if (String.IsNullOrEmpty(userid))
            {
                return NotFound("Không tìm thấy User");
            }
            user = await _userManager.FindByIdAsync(userid);
            if (user == null)
            {
                return NotFound($"Không tìm thấy User với ID: {userid}");
            }

          
            var OldRoleName = await _userManager.GetRolesAsync(user); //Editor, Member
            var deleteRole = OldRoleName.Where(r => !RoleNames.Contains(r)); // Nếu role của OldRole không có trong RoleName thì xóa (để loại bỏ) Editor
            var addRoles = RoleNames.Where(r => !OldRoleName.Contains(r)); // Nếu role của RoleName không có trong OldRole (thì thêm vào) // khong lay ra 
            var deleteResult = await _userManager.RemoveFromRolesAsync(user, deleteRole);

            //Nạp lại danh sách các Role
            List<string> roleNames = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
            allRoles = new SelectList(roleNames);

            if (!deleteResult.Succeeded)
            {
                foreach (var error in deleteResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
            var addResult =  await _userManager.AddToRolesAsync(user, addRoles); //Thêm thành công mặc dù addRoles không chứa dữ liệu về Role
            if( !addResult.Succeeded)
            {
                addResult.Errors.ToList().ForEach(error =>  ModelState.AddModelError(string.Empty, error.Description) );
                return Page();
            }
            await GetClaim(userid);
            StatusMessage = $"Thiết lập Role thành công cho User {user.UserName}";
            return RedirectToPage("./Index");
        }
        }
    }
