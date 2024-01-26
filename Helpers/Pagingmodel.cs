using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace RAZOR_PAGE9_ENTITY.Helpers
{
    public class Pagingmodel 
    {
        public int currentpage { get; set; }
        public int countpages { get; set; }
        public Func<int?, string> generateUrl { get; set; }//public delegate string Func ( int ?)
        }
    }
    

