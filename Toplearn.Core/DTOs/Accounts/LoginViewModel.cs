using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.DTOs.Accounts
{
    public class LoginViewModel
    {
        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        [MaxLength(200, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
        [EmailAddress(ErrorMessage = "{0} وارد شده معتبر نمی باشد .")]
        public string Email { get; set; }

        [Display(Name = "رمز عبور")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        [MaxLength(20, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
        [MinLength(8, ErrorMessage = "{0} شما نمیتواند کمتر از {1} باشه ")]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }

        public string? BackUrl { get; set; }
    }
}
