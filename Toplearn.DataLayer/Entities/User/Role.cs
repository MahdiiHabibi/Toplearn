using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.Entities.User
{
    public class Role
    {


        [Key]
        public int RoleId { get; set; }

        [Display(Name = "نام و عنوان مقام")]
        [Required(ErrorMessage = "{0} ضروری است .")]
        [MaxLength(20, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
        public string RoleDetail { get; set; }


        #region Relations || Nav Prop

        public List<User_Role> UserRoles { get; set; }

        #endregion
    }
}
