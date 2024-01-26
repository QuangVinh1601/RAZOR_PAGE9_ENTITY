using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace RAZOR_PAGE9_ENTITY.Pages.Shared
{
    public class _PagingXTLModel : PageModel
    {
        public int currentPage { get; set; }
        public int countPages { get; set; }

        public Func<int?, string> generateUrl { get; set; }
        public _PagingXTLModel()
        {

        }
        public void OnGet()
        {
        }
    }
}
