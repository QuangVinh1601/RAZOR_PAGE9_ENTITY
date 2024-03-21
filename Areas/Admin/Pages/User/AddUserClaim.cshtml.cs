using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RAZOR_PAGE9_ENTITY.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RAZOR_PAGE9_ENTITY.Areas.Admin.Pages.User
{
    public class AddUserClaimModel : PageModel
    {
        private readonly MyBlogContext _context;
        private readonly UserManager<AppUser> _userManager;
        public AddUserClaimModel(MyBlogContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [TempData]
        public string StatusMessage { get; set; }
        public class InputModel
        {
            [Display(Name = "Tên Claim")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1}")]
            public string ClaimType { get; set; }
            [Required]
            [StringLength(256, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự", MinimumLength = 3)]
            [Display(Name = "Giá trị Claim")]
            public string ClaimValue { get; set; }
        }
        [BindProperty]
        public InputModel Input { set; get; }
        public AppUser User { get; set; }
        public IdentityUserClaim<string> userClaim { get; set; }
        public NotFoundObjectResult OnGet()
        {
            return NotFound("Không được truy cập");
        }
        public async Task<IActionResult> OnGetAddClaimAsync(string userid)
        {
            User = await _userManager.FindByIdAsync(userid);
            if( User == null)
            {
                return NotFound("Không tìm thấy User");
            }
            return Page();
        }
        public async Task<IActionResult> OnGetEditClaimAsync (int claimid)
        {
            if(claimid == null)
            {
                return NotFound("Không tìm thấy Claim phù hợp");
            }
            userClaim =  _context.UserClaims.Where(ur => ur.Id == claimid).FirstOrDefault();
            if( userClaim == null)
            {
                return NotFound("Không tìm thấy Claim phù hợp");
            }
            User = await _userManager.FindByIdAsync(userClaim.UserId);
            if( User == null)
            {
                return NotFound("Không tìm thấy User");
            }
            Input = new InputModel()
            {
                ClaimType = userClaim.ClaimType,
                ClaimValue = userClaim.ClaimValue,
            };
            return Page();
        }
        public async Task<IActionResult> OnPostEditClaimAsync(int? claimid)
        {
            if (claimid == null)
            {
                return NotFound("Không tìm thấy Claim phù hợp");
            }
            userClaim = _context.UserClaims.Where(uc => uc.Id == claimid).FirstOrDefault();
            if (userClaim == null)
            {
                return NotFound("Không tìm thấy Claim phù hợp");
            }
            User = await _userManager.FindByIdAsync(userClaim.UserId);
            if (User == null)
            {
                return NotFound("Không tìm thấy User");
            }
            if(!ModelState.IsValid)
            {
                return Page();
            }
            if(_context.UserClaims.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.UserId == User.Id ))
            {
                ModelState.AddModelError(string.Empty, "Claim đã tồn tại");
                return Page();
            }

            userClaim.ClaimType = Input.ClaimType;
            userClaim.ClaimValue = Input.ClaimValue;
            await _context.SaveChangesAsync();
            StatusMessage = "Bạn đã cập nhật Claim";
            return RedirectToPage("./AddRole", new {userid = User.Id});
        }



        public async Task<IActionResult> OnPostAddClaimAsync(string userid)
        {
            User = await _userManager.FindByIdAsync(userid);
            if (User == null)
            {
                return NotFound("Không tìm thấy User phù hợp");
            }
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Dữ liệu nhập vào không hợp lệ");
                return Page();
            }
            var claim = _context.UserClaims.Where(ur => ur.UserId == userid).ToList();
            if (claim.Any(c => c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Dữ liệu Claim bị trùng");
                return Page();
            }
            await _userManager.AddClaimAsync(User, new Claim(Input.ClaimType, Input.ClaimValue));
            StatusMessage = "Đã thêm đặc tính cho User";
            return RedirectToPage("./AddRole", new { userid = User.Id });


        }
        public async Task<IActionResult> OnPostDeleteClaimAsync (int? claimid)
        {
            if(claimid == null)
            {
                return NotFound("Không tìm thấy Claim");
            }
            userClaim  = _context.UserClaims.Where(c => c.Id == claimid).FirstOrDefault();
            User = await _userManager.FindByIdAsync(userClaim.UserId);
            if(User == null) 
            {
                return NotFound("Không tìm thấy User");
            }
            await _userManager.RemoveClaimAsync(User, new Claim (userClaim.ClaimType, userClaim.ClaimValue));
            StatusMessage = "Đã xóa Claim thành công";
            return RedirectToPage("./AddRole", new {userid = User.Id});

        }
    }
}
