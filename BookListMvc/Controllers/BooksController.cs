using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookListMvc.Data;
using BookListMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace BookListMvc.Controllers
{
    public class BooksController : Controller
    {
        private readonly DataContext _context;

        [BindProperty]
        public Book Book { get; set; }

        public BooksController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Book = new Book();
            if (id is null)
            {
                //create
                return View(Book);
            }
            //update
            Book = _context.Books.Find(id);
            if (Book is null)
            {
                return NotFound();
            }

            return View(Book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (Book.Id == 0)
                {
                    //CREATE
                    _context.Books.Add(Book);
                }
                else
                {
                    _context.Books.Update(Book);
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Book);
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _context.Books.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var bookFromDb = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (bookFromDb is null)
            {
                return Json(new { success = false, message = "Error while deleting.." });
            }

            _context.Remove(bookFromDb);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Delete was successful" });
        }

        #endregion

    }
}
