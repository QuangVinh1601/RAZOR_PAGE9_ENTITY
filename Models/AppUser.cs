using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RAZOR_PAGE9_ENTITY.Models
{
	public class AppUser : IdentityUser
	{
		[Column(TypeName = "nvarchar")]
		[StringLength(400)]
		public string HomeAddress { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime? Birthday { get; set; }
	}
}