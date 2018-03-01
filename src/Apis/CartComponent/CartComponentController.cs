using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using RolleiShop.Models.Entities;
using RolleiShop.Models.Interfaces;
using RolleiShop.Identity;
using RolleiShop.Data.Context;
using RolleiShop.Services;
using RolleiShop.Services.Interfaces;
using RolleiShop.ViewModels;
using RolleiShop.ViewComponents;

namespace RolleiShop.Apis.CartComponent
{
    [Route ("api/[Controller]")]
    public class CartComponentController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly ICartViewModelService _cartViewModelService;

        public CartComponentController (
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<CartComponentController> logger,
            ICartViewModelService cartViewModelService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _cartViewModelService = cartViewModelService;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart()
        {
            var vm = new CartComponentViewModel();
            vm.ItemsCount = (await GetCartViewModelAsync()).Items.Sum(i => i.Quantity);
            return Ok(vm);
        }

        private async Task<CartViewModel> GetCartViewModelAsync()
        {
            if (_signInManager.IsSignedIn(User))
                return await _cartViewModelService.GetOrCreateCartForUser(User.Identity.Name);

            string anonymousId = GetOrSetCartCookie();
            return await _cartViewModelService.GetOrCreateCartForUser(anonymousId);
        }

        private string GetOrSetCartCookie()
        {
            if (Request.Cookies.ContainsKey("RolleiShop"))
                return Request.Cookies["RolleiShop"];

            string anonymousId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Today.AddYears(10);
            Response.Cookies.Append("RolleiShop", anonymousId, cookieOptions);
            return anonymousId;
        }
    }

    public class UpdateCartViewModel
    {
        public string UserName{ get; set; }
    }
}
