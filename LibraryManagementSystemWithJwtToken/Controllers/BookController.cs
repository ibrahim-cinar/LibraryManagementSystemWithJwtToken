using LibraryManagementSystemWithJwtToken.Models;
using LibraryManagementSystemWithJwtToken.Service;
using LibraryManagementSystemWithJwtToken.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystemWithJwtToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public IActionResult GetAllBooks()
        {
            return Ok(_bookService.GetAllBooks());
        }

        [HttpGet("{title}")]
        public IActionResult getBookByTitle(string title)
        {
            var book = _bookService.getBookByBookTitle(title);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpGet("author/{authorName}")]
        public IActionResult getBooksByAuthor(string authorName)
        {
            return Ok(_bookService.GetBooksByAuthorName(authorName));
        }

        [HttpPost]
        public IActionResult AddBook([FromBody] Book book)
        {
            try
            {
                var addedBook = _bookService.addBook(book); // Servis aracılığıyla kitap eklenir
                return CreatedAtAction(nameof(getBookByTitle), new { title = addedBook.Title }, addedBook);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Eksik bilgi varsa
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // Kitap zaten varsa
            }
        }

    }
}

