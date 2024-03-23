using Microsoft.AspNetCore.Authorization;

namespace RAZOR_PAGE9_ENTITY.Security
{
    public class UpdateArticleRequirement : IAuthorizationRequirement
    {
        public int Year { get; set; }
        public int Month { get; set; } 
        public int Day { get; set; }
        public UpdateArticleRequirement( int year = 2023 , int month = 7 , int day =1 )
        {
            Year = year;
            Month = month;
            Day = day;
        }
    }
}
