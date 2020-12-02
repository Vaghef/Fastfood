using Fastfood.Data;
using Fastfood.Models;
using Fastfood.Models.ViewModels;
using Fastfood.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Fastfood.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        public readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }


        public async Task<IActionResult> Index()
        {
            var menuItem = await _db.MenuItems.Include(c => c.Category).Include(sc => sc.SubCategory).ToListAsync();
            return View(menuItem);
        }

        public IActionResult Create()
        {
            return View(MenuItemVM);
        }
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            //MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            //if (!ModelState.IsValid)
            //    return View(MenuItemVM);

            _db.MenuItems.Add(MenuItemVM.MenuItem);
            await _db.SaveChangesAsync();

            MenuItem menuItemFromDb = await _db.MenuItems.FindAsync(MenuItemVM.MenuItem.Id);

            string webPathRoot = _hostEnvironment.WebRootPath;

            var files = HttpContext.Request.Form.Files;

            if(files.Count > 0)
            {
                var uploads = Path.Combine(webPathRoot, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(uploads, menuItemFromDb.Id + extension),FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                
                menuItemFromDb.Image = @"\images\" + menuItemFromDb.Id + extension;
            }
            else
            {
                var uploads = Path.Combine(webPathRoot, @"\images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webPathRoot + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
            }

            await _db.SaveChangesAsync();

            return View();
        }
    }
}
