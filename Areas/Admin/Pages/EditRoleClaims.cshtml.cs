using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RAZOR_PAGE9_ENTITY.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RAZOR_PAGE9_ENTITY.Areas.Admin.Pages
{
    public class EditRoleClaimsModel : RolePageModel
    {
        public EditRoleClaimsModel(RoleManager<IdentityRole> roleManager, AppDbContext context) : base(roleManager,context)
        {
        }


        public class InputModel
        {
            [Display(Name ="Tên Claim")]
            [Required(ErrorMessage ="Phải nhập {0}")]
            [StringLength(  256 , MinimumLength = 3, ErrorMessage ="{0} phải dài từ {2} đến {1}")]
            public string ClaimType {  get; set; }
            [Required]
            [StringLength(256, ErrorMessage ="{0} phải dài từ {2} đến {1} ký tự", MinimumLength = 3)]
            [Display(Name ="Giá trị Claim")]
            public string ClaimValue { get; set; }
        }
        [BindProperty]
        public InputModel Input { set; get; }
        public IdentityRole Role { get; set; }

        public async Task<IActionResult> OnGet(int ? claimid)
        {
           if(claimid == null)
           {
                return NotFound("Không tìm thấy Claim");
           }
            var claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
           if(claim == null)
            {
                return NotFound("Không tìm thấy Claim phù hợp");
            }
           Role =  await _roleManager.FindByIdAsync(claim.RoleId);
           if(Role == null)
            {
                return NotFound("Không tìm thấy Role");
            }
            Input = new InputModel()
            {
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue
            };
            return Page();

        }
        public async Task<IActionResult> OnPostAsync(int ? claimid)
        {
            if( claimid == null)
            {
                return NotFound("Không tìm thấy Claim ");
            }
            var claim =  _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if( claim == null)
            {
                return NotFound("Không tìm thấy Claim phù hợp");
            }
            Role = await _roleManager.FindByIdAsync(claim.RoleId);
            if(Role == null)
            {
                return NotFound("Không tìm thấy Role phù hợp");
            }
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Dữ liệu nhập vào không đúng");
                return Page();
            }
            if(_context.RoleClaims.Any(c => c.RoleId == Role.Id && c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Dữ liệu Claim đã tồn tại");
                return Page();
            }
            claim.ClaimType = Input.ClaimType;
            claim.ClaimValue = Input.ClaimValue;
            await _context.SaveChangesAsync();
            return RedirectToPage("./Edit", new { roleid = Role.Id });
        }
        public async Task<IActionResult> OnPostDeleteAsync (int? claimid)
        {
            if(claimid == null)
            {
                return NotFound("Không tìm thấy Claim phù hợp");
            }
            var claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if( claim == null)
            {
                return NotFound("Không tìm thấy Claim phù hợp");
            }
            Role = await _roleManager.FindByIdAsync(claim.RoleId);
            await _roleManager.RemoveClaimAsync(Role, new Claim (claim.ClaimType, claim.ClaimValue));
            StatusMessage = "Xóa thành công Claim";
            return RedirectToPage("./Edit", new {roleid= Role.Id});
        }
    }
}
