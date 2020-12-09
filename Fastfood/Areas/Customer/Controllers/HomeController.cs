using Fastfood.Data;
using Fastfood.Models.ViewModels;
using Fastfood.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fastfood.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItems = await _context.MenuItems.Include(s => s.Category).Include(s => s.SubCategory).ToListAsync(),
                Categories = await _context.Categories.ToListAsync(),
                Coupons = await _context.Coupons.Where(woak => woak.IsActive == true).ToListAsync()
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                var count = _context.shoppingCarts.Where(u => u.ApplicationUserId == claim.Value).ToList().Count;

                HttpContext.Session.SetInt32(SD.ssShopingCartCount, count);
            }
            return View(IndexVM);
        }
    }
}
