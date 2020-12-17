using Fastfood.Data;
using Fastfood.Models;
using Fastfood.Models.ViewModels;
using Fastfood.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fastfood.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            var subCategories = await _db.SubCategories.Include(s => s.Category).ToListAsync();

            return View(subCategories);
        }

        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = new SubCategory(),
                SubCategoryList = await _db.SubCategories.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel modelVM)
        {
            if(ModelState.IsValid)
            {
                var subCategoryExists = _db.SubCategories.Include(c => c.Category).Where(s => s.Name == modelVM.SubCategory.Name && s.Category.Id == modelVM.SubCategory.CategoryId);

                if(subCategoryExists.Count() > 0)
                {
                    //خطا
                    StatusMessage = "خطا:زیر گروه ثبت شده برای فهرست انتخابی موجود است";

                }
                else
                {
                    _db.SubCategories.Add(modelVM.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }
            return View(modelVM);
        }


        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from sb in _db.SubCategories where sb.CategoryId == id select sb).ToListAsync();

            


            return Json(new SelectList(subCategories, "Id", "Name"));
        }

        public async Task<IActionResult> Edit(int id)
        {

            var subCategory = await _db.SubCategories.FirstOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
                return NotFound();

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await _db.SubCategories.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {
            if(ModelState.IsValid)
            {
                var doesSubCategoryExisit = _db.SubCategories.Include(s => s.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);

                if(doesSubCategoryExisit.Count() > 0)
                {
                    StatusMessage = "خطا: زیرگروه برای فهرست جاری موجود عست";
                }
                else
                {
                    var subCategoryFromDb = await _db.SubCategories.FindAsync(model.SubCategory.Id);
                    subCategoryFromDb.Name = model.SubCategory.Name;

                    await _db.SaveChangesAsync();

                    return RedirectToAction("Index");
                }
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await _db.Categories.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await _db.SubCategories.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync(),
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var subCategory = await _db.SubCategories.Include(c => c.Category).FirstOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
                return NotFound();

            return View(subCategory);
        }

        public async Task<IActionResult> Delete (int? id)
        {
            if (id == null)
                return NotFound();

            var subCategory = await _db.SubCategories.Include(c => c.Category).FirstOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
                return NotFound();

            return View(subCategory);
        }
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subCategory = await _db.SubCategories.FirstOrDefaultAsync(m => m.Id == id);
            _db.SubCategories.Remove(subCategory);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
