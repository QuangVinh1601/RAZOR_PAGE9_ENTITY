using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAZOR_PAGE9_ENTITY.Areas.Admin.Pages
{
    [Authorize(Roles ="Administrator")]
    [Authorize(Roles ="Member")]
    public class IndexModel : RolePageModel
    {
        
        public IndexModel(RoleManager<IdentityRole> roleManager): base(roleManager) 
        { 
        
        }
        public List<IdentityRole> Roles { get; set; }
        public async Task OnGet()
        {
            Roles =  await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
        }
        public void OnPost()
        {
            RedirectToPage();
        }


    }
}
