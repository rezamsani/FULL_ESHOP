using Application.BasketsService;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSite.EndPoint.Utilities;

namespace WebSite.EndPoint.Controllers
{
    public class BasketController : Controller
    {
        private readonly IBasketsService basketService;
        private readonly SignInManager<User> signInManager;
        private string userId = null;
        public BasketController(IBasketsService basketService
            , SignInManager<User> signInManager)
        {
            this.basketService = basketService;
            this.signInManager = signInManager;
        }
        public IActionResult Index()
        {
            var data = GetOrSetBasket();
            return View(data);
        }


        [HttpPost]
        public IActionResult Index(int CatalogitemId, int quantity = 1)
        {
            var basket = GetOrSetBasket();
            basketService.AddItemToBasket(basket.Id, CatalogitemId, quantity);
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public IActionResult RemoveItemFromBasket(int ItemId)
        {
            basketService.RemoveItemFromBasket(ItemId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult setQuantity(int basketItemId, int quantity)
        {
            return Json(basketService.SetQuantities(basketItemId, quantity));
        }


        private BasketDto GetOrSetBasket()
        {
            if (signInManager.IsSignedIn(User))
            {
                userId = ClaimUtility.GetUserId(User);
                return basketService.GetOrCreateBasketForUser(userId);
            }
            else
            {
                SetCookiesForBasket();
                return basketService.GetOrCreateBasketForUser(userId);
            }
        }

        private void SetCookiesForBasket()
        {
            string basketCookieName = "BasketId";
            if (Request.Cookies.ContainsKey(basketCookieName))
            {
                userId = Request.Cookies[basketCookieName];
            }
            if (userId != null) return;
            userId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions { IsEssential = true };
            cookieOptions.Expires = DateTime.Today.AddYears(2);
            Response.Cookies.Append(basketCookieName, userId, cookieOptions);


        }
    }
}
