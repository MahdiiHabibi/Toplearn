﻿using Dto.Other;
using Dto.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Toplearn.Core.Checker;
using Toplearn.Core.DTOs.Wallet;
using Toplearn.Core.Security.Attribute.CheckIdentityValidationGuid;
using Toplearn.Core.Services.Implement;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.Wallet;
using Toplearn.Web.Security;
using ZarinPal.Class;

namespace Toplearn.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
	[CheckIVG]
	public class WalletController : TopLearnController
	{
		private readonly IWalletManager _walletManager;
		private readonly IUserPanelService _userPanelService;

		#region ZarinPal Parametrs

		private readonly Payment _payment;
		private readonly Authority _authority;
		private readonly Transactions _transactions;

		#endregion

		public WalletController(IWalletManager walletManager, IUserPanelService userPanelService)
		{
			_walletManager = walletManager;
			_userPanelService = userPanelService;

			#region inject ZarinPal Parametrs

			var expose = new Expose();
			_payment = expose.CreatePayment();
			_authority = expose.CreateAuthority();
			_transactions = expose.CreateTransactions();

			#endregion
		}


		[HttpGet]
		[Route("UserPanel/Wallet")]
		public async Task<IActionResult> index()
		{
			ViewBag.ListWallet = await _walletManager.GetShowWallets(GetUserIdFromClaims());
			return View();
		}


		[HttpPost]
		[Route("UserPanel/IncreaseWalletBalance")]
		public async Task<IActionResult> IncreaseWalletBalance(DepositWalletViewModel depositWalletViewModel)
		{
			if (ModelState.IsValid == false) return RedirectToAction("index", "Wallet");

			var userId = GetUserIdFromClaims();
			var setWallet = await _walletManager.SetWalletIncrease(userId, depositWalletViewModel.Amount);


			if (setWallet != null)
			{
				return RedirectToAction("RequestZarinPal", "Wallet");
			}
			else
			{
				CreateMassageAlert("danger", "در ثبت درخواست مبلغ شارژ مشکلی به وجود آمده است .", "خطا !");
				return RedirectToAction("index", "Wallet");
			}

		}

		#region ZarinPal Actions

		public async Task<IActionResult> RequestZarinPal()
		{
			var userId = GetUserIdFromClaims();
			var paymentInformation = _walletManager.GetPaymentInformation(userId);

			var result = await _payment.Request(new DtoRequest()
			{
				Mobile = "09233420331",
				CallbackUrl = $"{HttpContext.Request.Scheme + "://" + HttpContext.Request.Host}/UserPanel/Wallet/validate",
				Description = paymentInformation.Description,
				Email = "mahdihabibi813@gmail.com",
				// The amount we receive from the user is in Rials, so we have to convert it to Tomans. Because ZarinPal takes the amount from us in Tomans
				Amount = paymentInformation.Amount / 10,
				MerchantId = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX"
			}, Payment.Mode.sandbox);

			if (result.Status == 100)
			{
				return Redirect($"https://sandbox.zarinpal.com/pg/StartPay/{result.Authority}");
			}
			else
			{
				CreateMassageAlert("danger", "در اتصال  به درگاه پرداخت مشکلی به وجود آمده است .", "خطا از طرف درگاه پرداخت !");
				return RedirectToAction("index", "Home");
			}
		}


		public async Task<IActionResult> Validate(string authority, string status)
		{
			var userId = GetUserIdFromClaims();
			var paymentInformation = _walletManager.GetPaymentInformation(userId);

			var verification = await _payment.Verification(new DtoVerification
			{
				// The amount we receive from the user is in Rials, so we have to convert it to Tomans. Because ZarinPal takes the amount from us in Tomans
				Amount = paymentInformation.Amount / 10,
				MerchantId = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
				Authority = authority
			}, Payment.Mode.sandbox);

			if (verification.Status == 100)
			{
				var res = await _walletManager.WalletIncrease(paymentInformation.WalletId, verification.RefId, int.Parse(authority));
				if (res)
				{
					CreateMassageAlert("success", "افزایش شارژ کیف پول شما با موفقیت انجام شد .", "موفق");
				}
				else
				{
					CreateMassageAlert("danger", "در ثبت اطلاعات مربوطه مشکلی به وجود آمده است ." +
												$"شماره ی پیگیری شما {paymentInformation.WalletId}  و شماره ی درخواست شما {verification.RefId} است"
						+ "به ادمین سایت خبر دهید تا برای شما مبلغ پرداختی را ثبت کند.", "بروز خطا ! ");
				}
			}
			else
			{
				CreateMassageAlert("danger", "مبلغ شارژ شما پرداخت نشد . درصورتی که پول از حسابتان کسر شده تا 72 ساعت آینده برخواهد گشت .", "نا موفق");
			}
			return RedirectToAction("index", "Wallet");
		}



		#endregion

		

	}
}
