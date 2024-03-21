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
    public class AddRoleClaimsModel : RolePageModel
    {
        public AddRoleClaimsModel(RoleManager<IdentityRole> roleManager, MyBlogContext context) : base(roleManager,context)
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

        public async Task<IActionResult> OnGet(string roleid)
        {
           Role =  await _roleManager.FindByIdAsync(roleid);
            if(Role   == null)
            {
                return NotFound("Không tìm thấy Role phù hợp");
            }
            return Page();
        }
        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            Role =   await  _roleManager.FindByIdAsync(roleid);
            if(Role == null)
            {
                return NotFound("Không tìm thấy Role phù hợp");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //Kiểm tra xem Type và Value từ Input có trùng với Type và Value của Claim trên CSDL hay không
            if ((await _roleManager.GetClaimsAsync(Role)).Any(c => c.Type == Input.ClaimType && c.Value == Input.ClaimValue)){
                ModelState.AddModelError(string.Empty, "Claim đã bị trùng");
                return Page();
            }
            var newClaim = new Claim(Input.ClaimType, Input.ClaimValue);
            var result =   await _roleManager.AddClaimAsync(Role, newClaim);
            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(e => ModelState.AddModelError(string.Empty, e.Description));
            }
            StatusMessage = $"Đã thêm Claim cho Role {Role.Name} thành công";
            return RedirectToPage("./Edit", new {roleid = Role.Id});

        }
    }
}
