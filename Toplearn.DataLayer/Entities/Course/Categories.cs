using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.Entities.Course
{
	public class Category
	{
		public Category()
		{
			ChildCategories = new List<Category>();
		}


		[Key]
		public int CategoryId { get; set; }

		[Display(Name = "نام دسته بندی")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(100, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		public string CategoryName { get; set; }
		
		public int? ParentCategoryId { get; set; }

		[DefaultValue(true)]
		public bool IsActive { get; set; } = true;

		#region Nav Property

		[ForeignKey(nameof(ParentCategoryId))]
		public virtual List<Category>? ChildCategories { get; set; }

		public virtual ICollection<Course> Courses { get; set; }
		#endregion

	}

}
