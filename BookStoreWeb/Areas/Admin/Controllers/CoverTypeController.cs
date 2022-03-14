using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreWeb.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.Role_Admin)]
public class CoverTypeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CoverTypeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public IActionResult Index()
    {
        IEnumerable<CoverType> coverTypes = _unitOfWork.CoverType.GetAll();
        return View(coverTypes);
    }

    //GET - CREATE
    public IActionResult Create()
    {
        return View();
    }

    //POST - CREATE
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CoverType coverType)
    {
        if (int.TryParse(coverType.Name, out int number))
        {
            ModelState.AddModelError("name", "Cover Type can not be number.");
        }
        if (ModelState.IsValid)
        {
            _unitOfWork.CoverType.Add(coverType);
            _unitOfWork.Save();
            TempData["success"] = "Cover Type created succesfully.";
            return RedirectToAction("Index");
        }

        return View(coverType);
    }

    //GET - EDIT
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        CoverType coverType = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);

        if (coverType == null)
        {
            return NotFound();
        }
        return View(coverType);
    }

    //POST - EDIT
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(CoverType coverType)
    {
        if (int.TryParse(coverType.Name, out int number))
        {
            ModelState.AddModelError("name", "Cover Type can not be number.");
        }
        if (ModelState.IsValid)
        {
            _unitOfWork.CoverType.Update(coverType);
            _unitOfWork.Save();
            TempData["success"] = "Cover Type updated succesfully.";
            return RedirectToAction("Index");
        }

        return View(coverType);
    }

    //GET - DELETE
    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        CoverType coverType = _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);

        if (coverType == null)
        {
            return NotFound();
        }

        return View(coverType);
    }

    //POST - DELETE
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        CoverType coverType = _unitOfWork.CoverType.GetFirstOrDefault(u => u.Id == id);
        if (coverType == null)
        {
            return NotFound();
        }

        _unitOfWork.CoverType.Remove(coverType);
        _unitOfWork.Save();
        TempData["success"] = "Cover Type deleted succesfully.";

        return RedirectToAction("Index");
    }
}
