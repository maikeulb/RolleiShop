using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RolleiShop.Data.Context;
using RolleiShop.ViewModels;
using RolleiShop.Services;
using RolleiShop.Services.Interfaces;
using RolleiShop.Identity;
using RolleiShop.Models.Entities;
using RolleiShop.Models.Interfaces;

namespace RolleiShop.Apis.Cart
{
    [Route ("api/[Controller]")]
    public class CartController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;
        private readonly ICartViewModelService _cartViewModelService;

        public CartController (
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<CartController> logger,
            ApplicationDbContext context,
            ICartService cartService,
            ICartViewModelService cartViewModelService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _context = context;
            _cartService = cartService;
            _cartViewModelService = cartViewModelService;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart ([FromBody] ProductViewModel product)
        {
            var catalogItem =  await _context.Set<CatalogItem>().FindAsync(product.ProductId);
            var cartViewModel = await GetCartViewModelAsync ();
            await _cartService.AddItemToCart (cartViewModel.Id, catalogItem.Id, catalogItem.Price, 1);

            return Ok ();
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveFromCart ([FromBody] ProductViewModel product)
        {
            var catalogItem =  await _context.Set<CatalogItem>().FindAsync(product.ProductId);
            var cartViewModel = await GetCartViewModelAsync ();
            await _cartService.RemoveItemFromCart (cartViewModel.Id, catalogItem.Id);

            return Ok ();
        }

        private async Task<CartViewModel> GetCartViewModelAsync ()
        {
            if (_signInManager.IsSignedIn (User))
                return await _cartViewModelService.GetOrCreateCartForUser (User.Identity.Name);

            string anonymousId = GetOrSetCartCookie ();
            return await _cartViewModelService.GetOrCreateCartForUser (anonymousId);
        }

        private string GetOrSetCartCookie ()
        {
            if (Request.Cookies.ContainsKey ("RolleiShop"))
                return Request.Cookies["RolleiShop"];

            string anonymousId = Guid.NewGuid ().ToString ();
            var cookieOptions = new CookieOptions ();
            cookieOptions.Expires = DateTime.Today.AddYears (10);
            Response.Cookies.Append ("RolleiShop", anonymousId, cookieOptions);
            return anonymousId;
        }
    }

    public class ProductViewModel
    {
        public int ProductId { get; set; }
    }
}
