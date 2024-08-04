using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Toplearn.Core.DTOs.Accounts
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string ActiveCode { get; set; }

        [Display(Name = "رمز عبور")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        [MaxLength(20, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
        [MinLength(8, ErrorMessage = "{0} شما نمیتواند کمتر از {1} باشه ")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,20}$", ErrorMessage = "رمز عبور باید از حروف بزرگ و کوچک انگلیسی و علامت ها اختصاری و اعداد  تشکیل شده باشد .")]

		public string Password { get; set; }

        [Display(Name = "تکرار رمز عبور")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        [Compare(nameof(Password), ErrorMessage = "رمز عبور وارد با تکرار آن مطابقت ندارد .")]
        public string RePassword { get; set; }

    }
}