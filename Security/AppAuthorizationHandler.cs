using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using RAZOR_PAGE9_ENTITY.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RAZOR_PAGE9_ENTITY.Security
{
    public class AppAuthorizationHandler : IAuthorizationHandler
    {
        private readonly ILogger<AppAuthorizationHandler> _logger;
        private readonly UserManager<AppUser> _userManager;
        public AppAuthorizationHandler(ILogger<AppAuthorizationHandler> logger, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var requirements = context.PendingRequirements.ToList();
            _logger.LogInformation($"Resource: {context.Resource?.GetType().Name}");
            foreach ( var requirement in requirements)
            {
                if(requirement is GenZRequirement)
                {
                    if(IsGenZ(context.User, (GenZRequirement)requirement))
                    {
                        context.Succeed(requirement);
                    }
                    //code xu ly kiem tra User co phu hop vơi Requirement không ?
                    //context.Succeed();
                }
                if(requirement is UpdateArticleRequirement)
                {
                    if(UpdateArticle(context.User, context.Resource,(UpdateArticleRequirement)requirement))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
            return Task.CompletedTask;
        }

        private bool UpdateArticle(ClaimsPrincipal user, object resource, UpdateArticleRequirement requirement)
        {
            if (user.IsInRole("Administrator"))
            {
                _logger.LogInformation("Admin cap nhat...");
                return true;
            }
            Article article = (Article)resource;
            var articleCreated = article.Created; //lay ra ngay da tao
            DateTime articleTimeRequired = new DateTime(requirement.Year, requirement.Month, requirement.Day);
            if(articleCreated >= articleTimeRequired)
            {
                return true;
            }
            else
            {
                _logger.LogInformation("Qua ngay cap nhat");
                return false;
            }
        }

        private bool IsGenZ(ClaimsPrincipal user, GenZRequirement requirement)
        {
            var appUserTask = _userManager.GetUserAsync(user);
            Task.WaitAll(appUserTask);
            var appUser = appUserTask.Result;
            if(appUser.Birthday == null)
            {
                _logger.LogInformation($"{appUser.UserName} không có ngày sinh, không thỏa mãn Requirement");
                return false;
            }
            int year = appUser.Birthday.Value.Year;
            var success = (year >= requirement.FromYear && year <= requirement.ToYear);
            if(success)
            {
                _logger.LogInformation($"{appUser.UserName} có năm sinh phù hợp với Requirement");
            }
            else
            {
                _logger.LogInformation($"{appUser.UserName} có năm sinh không phù hợp với Requirement");
            }
            return success;


        }
    }
}
