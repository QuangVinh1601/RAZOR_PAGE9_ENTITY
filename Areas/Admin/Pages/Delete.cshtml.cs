using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using System.Threading.Tasks;

namespace RAZOR_PAGE9_ENTITY.Areas.Admin.Pages
{
    public class DeleteModel : RolePageModel
    {
        public DeleteModel(RoleManager<IdentityRole> roleManager) : base(roleManager)
        {
        }
        public IdentityRole Role { get; set; }
        public async Task<IActionResult> OnGet(string roleid)
        {
            if(roleid == null)
            {
                return NotFound("Không tìm thấy Role phù hợp");
            }
            Role =    await _roleManager.FindByIdAsync(roleid);
            if(Role == null)
            {
                return NotFound("Không tìm thấy Role phù hợp");
            }
            return Page();
        }
        public async Task<IActionResult> OnPost(string roleid)
        {
            if (roleid == null)
            {
                return NotFound("Không tìm thấy Role phù hợp");
            }
            Role = await _roleManager.FindByIdAsync(roleid);
            if(Role == null)
            {
                return NotFound("Không tìm thấy Role phù hợp");
            }
            var result =  await _roleManager.DeleteAsync(Role);
            if (result.Succeeded)
            {
                StatusMessage = $"Xóa thành công role: {Role.Name}";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(error => ModelState.AddModelError(string.Empty, error.Description) );
            }
            return Page();
        }
    }
}
