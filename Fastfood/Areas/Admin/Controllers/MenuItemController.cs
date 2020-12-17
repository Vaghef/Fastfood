using Fastfood.Data;
using Fastfood.Models;
using Fastfood.Models.ViewModels;
using Fastfood.Utilities;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        public readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }
        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
            MenuItemVM = new MenuItemViewModel
            {
                Category = _db.Categories,
                MenuItem = new MenuItem()
            };
        }

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
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (!ModelState.IsValid)
                return View(MenuItemVM);

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

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            MenuItemVM.MenuItem = await _db.MenuItems.Include(c => c.Category).Include(sc => sc.SubCategory).FirstOrDefaultAsync(woak => woak.Id == id);

            MenuItemVM.SubCategory = await _db.SubCategories.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
                return NotFound();

            return View(MenuItemVM);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
                return NotFound();

            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if(!ModelState.IsValid)
            {
                MenuItemVM.SubCategory = await _db.SubCategories.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

                return View(MenuItemVM);
            }

            string webRootPath = _hostEnvironment.WebRootPath;

            var files = HttpContext.Request.Form.Files;

            var menuitemFromDb = await _db.MenuItems.FindAsync(MenuItemVM.MenuItem.Id);

            if(files.Count > 0)
            {
                var uploads = Path.Combine(webRootPath, "images");

                var extension_new = Path.GetExtension(files[0].FileName);

                var imagePath = Path.Combine(webRootPath, menuitemFromDb.Image.TrimStart('\\'));

                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                using(var fileStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                menuitemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension_new;
            }

            menuitemFromDb.Name = MenuItemVM.MenuItem.Name;
            menuitemFromDb.Description = MenuItemVM.MenuItem.Description;
            menuitemFromDb.Price = MenuItemVM.MenuItem.Price;
            menuitemFromDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
            menuitemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menuitemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            MenuItemVM.MenuItem = await _db.MenuItems.Include(c => c.Category).Include(s => s.SubCategory).SingleOrDefaultAsync(woak => woak.Id == id);

            if (MenuItemVM.MenuItem == null)
                return NotFound();

            return View(MenuItemVM);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            MenuItemVM.MenuItem = await _db.MenuItems.Include(c => c.Category).Include(s => s.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (MenuItemVM.MenuItem == null)
                return NotFound();

            return View(MenuItemVM);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostEnvironment.WebRootPath;

            MenuItem menuitem = await _db.MenuItems.FindAsync(id);

            if(menuitem != null)
            {
                var imagePath = Path.Combine(webRootPath, menuitem.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);


                _db.MenuItems.Remove(menuitem);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("Index");

        }

    }
}
