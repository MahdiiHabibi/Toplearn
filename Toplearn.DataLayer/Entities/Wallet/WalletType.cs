using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.Entities.Wallet
{
	public class WalletType
	{
		// When We Write virtual in Navigation props we need have ctor 
		public WalletType()
		{
			
		}

		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int TypeId { get; set; }

		[Required]
		[MaxLength(150)]
		public string TypeTitle { get; set; }



		#region Relations || Nav Prop

		public virtual List<Wallet> Wallets { get; set; }


		#endregion
	}
}
