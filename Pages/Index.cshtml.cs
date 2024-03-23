using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using RAZOR_PAGE9_ENTITY.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAZOR_PAGE9_ENTITY.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AppDbContext myBlogContext;

        public IndexModel(ILogger<IndexModel> logger, AppDbContext _myblogcontext)
        {
            _logger = logger;
            myBlogContext = _myblogcontext;
        }

        public void OnGet()
        {
            var data = (from a in myBlogContext.articles
                       orderby a.Created descending
                       select a).ToList();

            ViewData["posts"] = data;


        }
    }
}
