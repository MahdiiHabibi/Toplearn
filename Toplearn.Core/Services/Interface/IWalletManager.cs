using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.Core.DTOs.Wallet;
using Toplearn.DataLayer.Entities.User;
using Toplearn.DataLayer.Entities.Wallet;

namespace Toplearn.Core.Services.Interface
{
	public interface IWalletManager
	{
		public Task<int> UserWalletBalanceInquiry(int userId);

		public Task<int> GetBalanceOfUser(int userId);

		public Task<Wallet?> SetWalletIncrease(int userId, int amount, bool? isPay = false);
		public Task<bool> WalletIncrease(int walletId, int refId, int authority);

		public Task<bool> IsUserNameExist(int userId);

		public GetPaymentInformationViewModel GetPaymentInformation(int userId);

	}
}
