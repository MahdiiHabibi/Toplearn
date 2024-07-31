using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.DTOs.Wallet
{
	public class DepositWalletViewModel
	{

		[Display(Name = "مبلغ به ریال")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		public int Amount { get; set; }

	}
}
