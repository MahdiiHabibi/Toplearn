using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.Entities.Wallet
{
	public class Wallet
	{

		// When We Write virtual in Navigation props we need have ctor 
		public Wallet()
		{

		}

		[Key]
		public int WalletId { get; set; }

		[Display(Name = "نوع تراکنش")]
		[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
		public int TypeId { get; set; }

		[Display(Name = "کاربر")]
		[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
		public int UserId { get; set; }

		[Display(Name = "مبلغ")]
		[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
		public int Amount { get; set; }

		[Display(Name = "شرح")]
		[MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
		public string Description { get; set; }

		[Display(Name = "وضعیت تراکنش ")]
		public bool IsPay { get; set; }

		[Display(Name = "تاریخ و ساعت")]
		public DateTime CreateDate { get; set; }

		public int RefId { get; set; } = 0;
		public int Authority { get; set; } = 0;


		#region Relations || Nav Prop

		public virtual User.User User { get; set; }

		[ForeignKey(nameof(TypeId))]
		public virtual WalletType WalletType { get; set; }

		#endregion

	}
}
