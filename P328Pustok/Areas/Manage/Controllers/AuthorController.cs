using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P328Pustok.DAL;
using P328Pustok.Helpers;
using P328Pustok.Migrations;
using P328Pustok.Models;

namespace P328Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AuthorController : Controller
    {
        private readonly PustokContext _context;
        private readonly IWebHostEnvironment _env;

        public AuthorController(PustokContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Authors.Include(x=>x.Books).ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Author author)
        {
            if (!ModelState.IsValid)
                return View();

            author.ImgName = FileManager.Save(_env.WebRootPath, "uploads/author", author.AuthorImage);

            if (_context.Authors.Any(x => x.FullName == author.FullName))
            {
                ModelState.AddModelError("FullName", "Name is already taken");
                return View();
            }
            _context.Authors.Add(author);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Author author = _context.Authors.Find(id);

            if (author == null) return View("Error");

            return View(author);
        }

        [HttpPost]
        public IActionResult Edit(Author author)
        {
            if (!ModelState.IsValid) return View();

            Author existAuthor = _context.Authors.Find(author.Id);

            if (existAuthor == null) return View("Error");

            if (author.FullName != existAuthor.FullName && _context.Authors.Any(x => x.FullName == author.FullName))
            {
                ModelState.AddModelError("FullName", "THis Author  is already have");
                return View();
            }

            string OldImgName = null;

            if (author.ImgName != null)
            {
                OldImgName = existAuthor.ImgName;
                existAuthor.ImgName = FileManager.Save(_env.WebRootPath, "uploads/sliders", author.AuthorImage);
            }

            existAuthor.FullName = author.FullName;

            _context.SaveChanges();
            if (OldImgName != null)
                FileManager.Delete(_env.WebRootPath, "uploads/sliders", OldImgName);


            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Author author = _context.Authors.Include(x => x.Books).FirstOrDefault(x => x.Id == id);

            if (author == null) return View("Error");
            return View(author);
        }

        [HttpPost]
        public IActionResult Delete(Author author)
        {
            Author existAuthor = _context.Authors.Find(author.Id);

            if (existAuthor == null) return View("Error");

            _context.Authors.Remove(existAuthor);
            _context.SaveChanges();
            FileManager.Delete(_env.WebRootPath, "uploads/sliders", existAuthor.ImgName);

            return RedirectToAction("index");
        }
    }
}
