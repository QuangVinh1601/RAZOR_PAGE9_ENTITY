﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RAZOR_PAGE9_ENTITY.Models;
using RAZOR_PAGE9_ENTITY.Security;
using RAZOR_PAGE9_ENTITY.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RAZOR_PAGE9_ENTITY
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddDbContext<AppDbContext>((options) =>
            {
                string connectionString = Configuration.GetConnectionString("MyBlogContext");
                options.UseSqlServer(connectionString);
            });
            //Dang ky Identity
            services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

            //services.AddDefaultIdentity<AppUser>() //them dich vu Identity vs cau hinh mac dinh cho AppUserModel
            //        .AddEntityFrameworkStores<MyBlogContext>() //Them EF de luu tru thong tin User
            //        .AddDefaultTokenProviders(); // them Token: Reset password, confirm email,..
            // Truy cập IdentityOptions
            services.Configure<IdentityOptions>(options =>
            {
                // Thiết lập về Password 
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 5 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
                options.SignIn.RequireConfirmedAccount = true;
            });
            services.AddOptions();
            var mailSetting = Configuration.GetSection("MailSettings");
            //Tạo lớp Configure cho ứng dụng
            services.Configure<MailSetting>(mailSetting);
            services.AddTransient<IEmailSender, SendMailService>();
            // Lấy ra lớp AppIdentityErrorDecriber chuyên hiển thị lỗi của ModelState
            services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();
            services.ConfigureApplicationCookie((options) =>
            {
                options.LoginPath = "/login/";
                options.LogoutPath = "/logout/";
                options.AccessDeniedPath = "/khongduoctruycap.html";
            });

            //services.Configure<SecurityStampValidatorOptions>(option => option.ValidationInterval = TimeSpan.FromSeconds(20));

            //Thêm dịch vụ xác thực bằng Google 

            services.AddAuthentication().
                     AddGoogle((options) =>
                     {
                         var gconfigure = Configuration.GetSection("Application:Google");
                         options.ClientId = gconfigure["ClientId"];
                         options.ClientSecret = gconfigure["ClientSecret"];
                         options.CallbackPath = "/dang-nhap-bang-google";
                     })
                    .AddFacebook((options) =>
                    {
                        var config = Configuration.GetSection("Application:Facebook");
                        options.AppId = config["AppId"];
                        options.AppSecret = config["AppSecret"];
                        options.CallbackPath = "/dang-nhap-bang-facebook";
                    });
            // Policy-based authorization
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AllowEditRole", policybuilder =>
                {
                    //Thiết lập điều kiện cho Policy
                    policybuilder.RequireAuthenticatedUser(); // Yêu cầu người dùng đăng nhập
                    //policybuilder.RequireRole("Administrator"); //Người dùng phải có Role là Admin
                    //policybuilder.RequireRole("VipProMember"); // Người dùng phải có Role là Editor

                    policybuilder.RequireClaim("duocphepxoa", "post", "get");
                    //Claim-based authorization
                    //policybuilder.RequireClaim("Ten Claim", "Gia tri 1 cua claim", "Gia tri 2 cua Claim");
                    //policybuilder.RequireClaim("Ten Claim 2", new string[] { "Gia tri 1 cua claim", "Gia tri 2 cua Claim" });

                    //IdentityRoleClaim<string> claim1; -> tra ve tu DbContext
                    //IdentityUserClaim<string> claim2; -> tra ve tu DbContext
                    //Claim claim3;  -> tra ve tu dich vu Identity
                    
                });
                options.AddPolicy("InGenZ", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.Requirements.Add(new GenZRequirement());
                    //User, Requirement -> Authorization handler 
                });
                options.AddPolicy("AdminMenuDropdown", policyBuilder =>
                {
                    policyBuilder.RequireRole("Administrator");
                });
                options.AddPolicy("CanUpdateArticle", policyBuilder =>
                {
                    policyBuilder.Requirements.Add(new UpdateArticleRequirement());
                });
            });
            services.AddTransient<IAuthorizationHandler, AppAuthorizationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
               
            });
        }
    }
}
// CREATE,READ, UPDATE, DELETE (CRUD)
//dotnet aspnet-codegenerator razorpage -m RAZOR_PAGE9_ENTITY.Models.Article -dc RAZOR_PAGE9_ENTITY.Models.MyBlogContext -outDir Pages/Blog -udl --referenceScriptLibraries

//IDENTITY
// Athentication: Xác định danh tính -> Login , logout
// Authorization: Xác định quyền truy cập  
   //Role-based authorization
// Quản lý user : Sign Up, User, Role