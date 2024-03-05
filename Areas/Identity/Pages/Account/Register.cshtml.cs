using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using RAZOR_PAGE9_ENTITY.Models;

namespace RAZOR_PAGE9_ENTITY.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage ="Phải nhập Email")]
            [EmailAddress(ErrorMessage ="Sai định dạng Email")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage ="Phải nhập mật khẩu")]
            [StringLength(100, ErrorMessage = "{0} phải dài từ {2} đến {1}", MinimumLength = 2)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Nhập lại mật khẩu")]
            [Compare("Password", ErrorMessage = "Mật khẩu nhập lại không phù hợp.")]
            public string ConfirmPassword { get; set; }

            [DataType(DataType.Text)]
            [Required(ErrorMessage ="Phải nhập {0}")]
            [StringLength(30, ErrorMessage ="{0} phải dài từ {2} đến {1} ký tự", MinimumLength = 5)]
            [Display(Name ="Tên tài khoản")]
            public string UserName { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;

            // Lấy các Provider (dịch vụ ngoài)
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            // Hiển thị tên các Provider
            foreach(var providers in ExternalLogins)
            {
                _logger.LogInformation(providers.Name);
            }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new AppUser { UserName = Input.UserName, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Đã tạo tài khoản thành công");
                    // Phát sinh token để xác thực email
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    // Encode token để đính kèm trên địa chỉ Url
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //https://localhost:5001/confirm-email?userId=hdfth5&code=thdthdhg@returnUrl=
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email,
                        "Xác thực email",
                        $"Bạn đã đăng ký tài khoản trên Razor Web, vui lòng <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>bấm vào đây</a> để xác thực email!.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
