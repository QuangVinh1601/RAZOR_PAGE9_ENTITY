using Microsoft.AspNetCore.Identity;

namespace RAZOR_PAGE9_ENTITY
{
	public class AppUser : IdentityUser
	{
		public string Name { get; set; }
	}
}