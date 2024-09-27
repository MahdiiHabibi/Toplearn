using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Toplearn.Core.DTOs.Wallet;
using Toplearn.Core.Services.Interface;
using Toplearn.Core.Services.Interface.Mapper;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.User;
using Toplearn.DataLayer.Entities.Wallet;

namespace Toplearn.Core.Services.Implement
{
	public class WalletManager(IContextActions<Wallet?> _contextActionsForWallet, IContextActions<User> _contextActionsForUser, TopLearnContext _db, IMapperWallet _mapperWallet) : IWalletManager
	{
		public async Task<int> UserWalletBalanceInquiry(int userId)
		{
			var enter = (await _contextActionsForWallet.Get(w => w.UserId == userId && w.TypeId == 2 && w.IsPay))
				.Select(w => w.Amount).ToList();

			var exit = (await _contextActionsForWallet.Get(w => w.UserId == userId && w.TypeId == 1 && w.IsPay))
				.Select(w => w.Amount).ToList();

			return (enter.Sum() - exit.Sum());
		}

		public async Task<int> GetBalanceOfUser(int userId) =>
			((await _contextActionsForUser.GetOne(x => x.UserId == userId))!).WalletBalance;

		public async Task<Wallet?> SetWalletIncrease(int userId, int amount, bool? isPay = false, string? AdminUsername = null)
		{
			try
			{
				var userRes = await IsUserNameExist(userId);

				if (!userRes) return null;

				var newWalletModel = new Wallet()
				{
					UserId = userId,
					Amount = amount,
					CreateDate = DateTime.Now,
					TypeId = 2,
					IsPay = (bool)isPay!,
					Description = "افزایش شارژ" + (AdminUsername ?? "")
				};

				bool res = await _contextActionsForWallet.AddToContext(newWalletModel);

				if (res)
				{
					var user = await _contextActionsForUser.GetOne(x => x.UserId == userId);
					user.WalletBalance += amount;
					await _contextActionsForUser.UpdateTblOfContext(user);
					return newWalletModel;
				}
				else
				{
					return null;
				}

			}
			catch
			{
				return null;
			}
		}

		public async Task<bool> WalletIncrease(int walletId, int refId, int authority)
		{
			try
			{
				var wallet = _db.Wallets.Include(x => x.User).Single(x => x.WalletId == walletId);
				wallet.IsPay = true;
				wallet.RefId = refId;
				wallet.Authority = authority;
				wallet.User.WalletBalance += wallet.Amount;
				return await _contextActionsForWallet.UpdateTblOfContext(wallet);
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> IsUserNameExist(int userId) =>
			await _contextActionsForUser.Exists(x => x.UserId == userId);

		public GetPaymentInformationViewModel GetPaymentInformation(int userId)
		{
			var Finalwallet = _db.Wallets.OrderByDescending(x => x.WalletId).First(x => x.UserId == userId && !x.IsPay && x.TypeId == 2);
			return _mapperWallet.MapTheGetPaymentInformationViewModelFromWallet(Finalwallet);
		}

		public async Task<List<ShowWalletsViewModel>> GetShowWallets(int userId)
		{
			var wallet = await _contextActionsForWallet.Get(x => x.UserId == userId);
			return _mapperWallet.MapTheShowWalletsViewModelFromWallet(wallet.ToList());
		}
	}
}
