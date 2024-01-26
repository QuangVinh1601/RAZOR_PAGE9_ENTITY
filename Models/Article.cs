using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RAZOR_PAGE9_ENTITY.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255,MinimumLength =5,ErrorMessage ="{0} phải nhập từ {2} đến {1} ký tự")]
        [Required(ErrorMessage ="{0} phải nhập")]
        [Column(TypeName ="nvarchar")]
        [DisplayName("Tiêu đề")]
        public string Title { get; set; }
        [Required(ErrorMessage ="{0} phải nhập")]
        [DataType(DataType.Date)]
        [Display(Name ="Ngày khởi tạo")]
        public DateTime Created { get;set; }
        [Column(TypeName = "ntext")]
        [Display(Name ="Nội dung bài viết")]
        public string Content { get; set; }
    }
}
