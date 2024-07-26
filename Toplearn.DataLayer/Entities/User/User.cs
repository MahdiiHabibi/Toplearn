using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.Entities.User
{
    public class User
    {


        [Key]
        public int UserId { get; set; }

        [Display(Name = "نام کاربری")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        [MaxLength(20, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]

        public string UserName { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        [MaxLength(60, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]

        public string FullName { get; set; }

		[Display(Name = "رمز عبور")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        [MaxLength(200, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
        [MinLength(8, ErrorMessage = "{0} شما نمیتواند کمتر از {1} باشه ")]
        public string Password { get; set; }

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        [MaxLength(200, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "تاریخ ثبت نام ")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        public DateTime DateTime { get; set; }

        [Display(Name = " وضعیت حساب کاربری")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        public bool IsActive { get; set; }

        [Display(Name = "کد فعالسازی")]
        [MaxLength(50, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
        public string ActiveCode { get; set; }

        [Display(Name = "عکس")]
        [MaxLength(500, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
        [DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }


        #region Relations || Nav Prop

        public List<User_Role> UserRoles { get; set; }

        #endregion

    }
}
