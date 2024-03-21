using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RAZOR_PAGE9_ENTITY.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAZOR_PAGE9_ENTITY.Areas.Admin.Pages
{
    [Authorize]
    public class IndexModel : RolePageModel
    {

        public IndexModel(RoleManager<IdentityRole> roleManager, MyBlogContext context) : base(roleManager, context)
        { 
        
        }
        public class RoleModel : IdentityRole
        {
            public string[] Claims { get; set; }
        }
        public List<RoleModel> Roles { get; set; }
        public async Task OnGet()
        {
            var roles =  await _roleManager.Roles.OrderBy(r => r.Name).ToListAsync();
            Roles = new List<RoleModel>();
            foreach (var role in roles)
            {
                var claim = await _roleManager.GetClaimsAsync(role);
                var claimString = claim.Select(c => c.Type + " : " + c.Value).ToArray();

                var roleModel = new RoleModel()
                {
                    Name = role.Name,
                    Id = role.Id,
                    Claims = claimString
                };
                Roles.Add(roleModel);
            }
        }   
        public void OnPost()
        {
            RedirectToPage();
        }


    }
}
