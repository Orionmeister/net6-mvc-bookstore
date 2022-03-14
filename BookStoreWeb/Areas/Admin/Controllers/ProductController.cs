using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreWeb.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }
    public IActionResult Index()
    {
        return View();
    }

    //GET - UPSERT
    public IActionResult Upsert(int? id)
    {
        ProductViewModel productVM = new ProductViewModel()
        {
            Product = new Product(),
            CategoryList = _unitOfWork.Category.GetAll().Select(
                c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                }),
            CoverTypeList = _unitOfWork.CoverType.GetAll().Select(
                c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),
                })
        };

        if (id == null || id == 0)
        {
            //create product
            //ViewBag.CategoryList = CategoryList;
            //ViewData["CoverTypeList"] = CoverTypeList;
            return View(productVM);
        }
        else
        {
            //update product
            productVM.Product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
            return View(productVM);
        }
    }

    //POST - UPSERT
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(ProductViewModel productVM, IFormFile? file)
    {
        var wwwRootPath = _webHostEnvironment.WebRootPath;
        if (ModelState.IsValid)
        {
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var uploads = Path.Combine(wwwRootPath, @"images\products");
                var extension = Path.GetExtension(file.FileName);

                if (productVM.Product.ImageUrl != null)
                {
                    var existingImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                    if (System.IO.File.Exists(existingImagePath))
                    {
                        System.IO.File.Delete(existingImagePath);
                    }
                }

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    file.CopyTo(fileStreams);
                }
                productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
            }

            if (productVM.Product.Id == 0)
            {
                _unitOfWork.Product.Add(productVM.Product);
                TempData["success"] = "Product created succesfully.";
            }
            else
            {
                _unitOfWork.Product.Update(productVM.Product);
                TempData["success"] = "Product updated succesfully.";
            }

            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        return View(productVM);
    }

    #region API CALLS
    [HttpGet]
    public IActionResult GetAll()
    {
        var productList = _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
        return Json(new { data = productList });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var product = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
        if (product == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        var wwwRootPath = _webHostEnvironment.WebRootPath;


        var existingImagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('\\'));
        if (System.IO.File.Exists(existingImagePath))
        {
            System.IO.File.Delete(existingImagePath);
        }

        _unitOfWork.Product.Remove(product);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Product has been deleted." });

    }

    #endregion
}


