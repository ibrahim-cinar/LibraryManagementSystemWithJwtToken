using LibraryManagementSystemWithJwtToken.Models;
using LibraryManagementSystemWithJwtToken.Service;
using LibraryManagementSystemWithJwtToken.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAllBooks(int page = 1, int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Sayfa ve sayfa boyutu pozitif bir değer olmalıdır.");
            }

            var pagedResult = _bookService.GetAllBooks(page, pageSize);

            if (!pagedResult.Items.Any())
            {
                return NotFound("Bu sayfa için veri bulunamadı.");
            }

            return Ok(pagedResult);
        }
        [Authorize(Roles = "User")]
        [HttpGet("{title}")]
        public IActionResult getBookByTitle(string title)
        {
            var book = _bookService.getBookByBookTitle(title);
            if (book == null) return NotFound();
            return Ok(book);
        }
        [Authorize(Roles = "User")]
        [HttpGet("author/{authorName}")]
        public IActionResult getBooksByAuthor(string authorName)
        {
            return Ok(_bookService.GetBooksByAuthorName(authorName));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> addBookAsync([FromBody] Book book)
        {
            try
            {
                await _bookService.AddBookAsync(book); // Asenkron metod çağrısı
                return CreatedAtAction(nameof(GetAllBooks), new { id = book.Id }, book);
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

