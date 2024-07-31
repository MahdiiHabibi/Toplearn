using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.Entities.User
{
    public class User_Role
    {
        // When We Write virtual in Navigation props we need have ctor 
	    public User_Role()
	    {
		    
	    }


        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }

        public int RoleId { get; set; }


        #region Relations || Nav Prop

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }


        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; }

        #endregion
    }
}
