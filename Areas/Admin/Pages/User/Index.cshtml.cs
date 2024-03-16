using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RAZOR_PAGE9_ENTITY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAZOR_PAGE9_ENTITY.Areas.Admin.Pages.User
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        [TempData]
        public string StatusMessage { get; set; }
        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public class UserAndRole : AppUser
        {
            public string RoleNames { get; set; }
        }
        public List<UserAndRole> Users { get; set; }
        public const int ITEMS_PER_PAGE = 10;
        public int countPages { get; set; }
        [BindProperty(SupportsGet = true, Name ="page")]
        public int currentPage { set; get; }
        public int totalUser {  get; set; }
        public async Task OnGet()
        {
            //Users =  await _userManager.Users.OrderBy(user => user.UserName).ToListAsync();
            var qr =   _userManager.Users.OrderBy(user => user.UserName); // lấy ra User và sắp xếp
            totalUser = await qr.CountAsync(); //đếm số User
            countPages = (int)Math.Ceiling((double)totalUser / ITEMS_PER_PAGE); // tính ra số trang dựa theo tổng số User và số User cho mỗi trang
            if(currentPage <1)
            {
                currentPage = 1;
            }
            if(currentPage > countPages)
            {
                currentPage = countPages;
            }
            var qr1 = qr.Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE).
                Select(user => new UserAndRole() { Id = user.Id, UserName = user.UserName });
            Users = qr1.ToList();
            foreach(var user in Users)
            {
                var roleNames = await  _userManager.GetRolesAsync(user);
                user.RoleNames = String.Join(",", roleNames);
            }
        }
    }
}
