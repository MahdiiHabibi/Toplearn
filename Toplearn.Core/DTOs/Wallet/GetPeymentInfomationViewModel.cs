using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.DTOs.Wallet
{
    public class GetPaymentInformationViewModel
    {
	    [Display(Name = "مبلغ")]
	    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
	    public int Amount { get; set; }


	    [Display(Name = "شرح")]
	    [MaxLength(500, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
	    public string Description { get; set; }

	    public int WalletId { get; set; }

	    
	}
}
