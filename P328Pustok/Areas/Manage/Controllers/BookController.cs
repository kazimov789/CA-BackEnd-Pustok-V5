﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P328Pustok.DAL;
using P328Pustok.Helpers;
using P328Pustok.Models;
using P328Pustok.ViewModels;

namespace P328Pustok.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BookController : Controller
    {
        private readonly PustokContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(PustokContext pustokContext,IWebHostEnvironment env)
        {
            _context = pustokContext;
            _env = env;
        }
        public IActionResult Index(int page=1, string search=null)
        {
            var query = _context.Books
                .Include(x => x.Author).Include(x => x.Genre).Include(x=>x.BookImages).AsQueryable();

            if (search != null)
                query = query.Where(x => x.Name.Contains(search));
            
            ViewBag.Search = search;

            return View(PaginatedList<Book>.Create(query,page,3));
        }

        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (!ModelState.IsValid)
                return View();


            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "AuthorIs is not correct");
                return View();
            }

            if (!_context.Genres.Any(x => x.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "GenreId is not correct");
                return View();
            }

            if (book.PosterImage == null)
            {
                ModelState.AddModelError("PosterImage", "posterImage is required");
                return View();
            }
            if (book.HoverImage == null)
            {
                ModelState.AddModelError("HoverPosterImage", "posterImage is required");
                return View();
            }

            foreach (var tagId in book.TagIds)
            {
                BookTag bookTag = new BookTag
                {
                    TagId = tagId,
                };

                book.BookTags.Add(bookTag);
            }

            BookImage PosterImage = new BookImage
            {
                ImageName = FileManager.Save(_env.WebRootPath,"uploads/books",book.PosterImage),
                PosterStatus = true,
            };
            book.BookImages.Add(PosterImage);
            BookImage HoverImage = new BookImage
            {
                ImageName = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverImage),
                PosterStatus = false,
            };
            book.BookImages.Add(HoverImage);


            foreach (var img in  book.Images)
            {
                BookImage Images = new BookImage
                {
                    ImageName = FileManager.Save(_env.WebRootPath, "uploads/books",img),
                    PosterStatus = null,
                };
                book.BookImages.Add(Images);
            }

            _context.Books.Add(book);
            _context.SaveChanges();


            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();


            Book book = _context.Books.Include(x => x.BookImages).Include(x => x.BookTags).FirstOrDefault(x => x.Id == id);

            book.TagIds = book.BookTags.Select(x => x.TagId).ToList();
            

            return View(book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Book book)
        {
            if (!ModelState.IsValid) return View();

            Book existBook = _context.Books.Include(x => x.BookTags).Include(x => x.BookImages).FirstOrDefault(x => x.Id == book.Id);

            if (existBook == null) return View("Error");

            if (book.AuthorId != existBook.AuthorId && !_context.Authors.Any(x => x.Id == book.AuthorId))
            {
                ModelState.AddModelError("AuthorId", "AuthorId is not correct,Have not this author.");
                return View();
            }

            if (book.GenreId != existBook.GenreId && !_context.Genres.Any(x => x.Id == book.GenreId))
            {
                ModelState.AddModelError("GenreId", "GenreId is not correct,,Have not this author.");
                return View();
            }

            existBook.BookTags.RemoveAll(x => !book.TagIds.Contains(x.TagId));

            var newTagIds = book.TagIds.FindAll(x => !existBook.BookTags.Any(bt => bt.TagId == x));
            foreach (var tagId in newTagIds)
            {
                BookTag bookTag = new BookTag { TagId = tagId };
                existBook.BookTags.Add(bookTag);
            }


            string oldPoster = null;
            if (book.PosterImage != null)
            {
                BookImage poster = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == true);
                oldPoster = poster?.ImageName;

                if (poster == null)
                {
                    poster = new BookImage { PosterStatus = true };
                    poster.ImageName = FileManager.Save(_env.WebRootPath, "uploads/books", book.PosterImage);
                    existBook.BookImages.Add(poster);
                }
                else
                    poster.ImageName = FileManager.Save(_env.WebRootPath, "uploads/books", book.PosterImage);
            }

            string oldHoverPoster = null;
            if (book.HoverImage != null)
            {
                BookImage hoverPoster = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == false);
                oldHoverPoster = hoverPoster?.ImageName;

                if (hoverPoster == null)
                {
                    hoverPoster = new BookImage { PosterStatus = false };
                    hoverPoster.ImageName = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverImage);
                    existBook.BookImages.Add(hoverPoster);
                }
                else
                    hoverPoster.ImageName = FileManager.Save(_env.WebRootPath, "uploads/books", book.HoverImage);
            }

            var removedImages = existBook.BookImages.FindAll(x => x.PosterStatus == null && !book.BookImageIds.Contains(x.Id));
            existBook.BookImages.RemoveAll(x => x.PosterStatus == null && !book.BookImageIds.Contains(x.Id));

            foreach (var item in book.Images)
            {
                BookImage bookImage = new BookImage
                {
                    ImageName = FileManager.Save(_env.WebRootPath, "uploads/books", item),
                };
                existBook.BookImages.Add(bookImage);
            }

            existBook.Name = book.Name;
            existBook.SalePrice = book.SalePrice;
            existBook.CostPrice = book.CostPrice;
            existBook.Desc = book.Desc;
            existBook.IsFeatured = book.IsFeatured;
            existBook.IsNew = book.IsNew;
            existBook.StockStatus = book.StockStatus;
            existBook.DiscountPercent = book.DiscountPercent;
            existBook.AuthorId = book.AuthorId;
            existBook.GenreId = book.GenreId;

            _context.SaveChanges();


            if (oldPoster != null) FileManager.Delete(_env.WebRootPath, "uploads/books", oldPoster);
            if (oldHoverPoster != null) FileManager.Delete(_env.WebRootPath, "uploads/books", oldHoverPoster);

            if (removedImages.Any())
                FileManager.DeleteAll(_env.WebRootPath, "uploads/books", removedImages.Select(x => x.ImageName).ToList());


            return RedirectToAction("index");
        }

    }
}
