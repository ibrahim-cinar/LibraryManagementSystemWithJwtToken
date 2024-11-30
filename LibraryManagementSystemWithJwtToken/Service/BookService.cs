using LibraryManagementSystemWithJwtToken.Models;
using LibraryManagementSystemWithJwtToken.Service.Interfaces;
using LibraryManagementSystemWithJwtToken.Data;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystemWithJwtToken.Pagination;

namespace LibraryManagementSystemWithJwtToken.Service
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }
        public PagedResult<Book> GetAllBooks(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Sayfa ve sayfa boyutu pozitif bir değer olmalıdır.");
            }

            var totalCount = _context.Books.Count();

            var items = _context.Books
                .Skip((page - 1) * pageSize) // Önceki sayfalardaki kayıtları atla
                .Take(pageSize)             // Belirtilen sayfa boyutunda kayıt getir
                .ToList();

            return new PagedResult<Book>
            {
                Items = items,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                CurrentPage = page
            };
        }

        public List<Book> GetBooksByAuthorName(string authorName)
        {
            return _context.Books
       .Where(b => b.AuthorName.Equals(authorName, StringComparison.OrdinalIgnoreCase))
       .ToList();

        }

        public Book getBookByBookTitle(string bookTitle)
        {
            return _context.Books
    .FirstOrDefault(b => b.Title.Equals(bookTitle, StringComparison.OrdinalIgnoreCase));
        }
        public async Task AddBookAsync(Book book)
        {
            ValidateBookInput(book);

            ValidatePublicationYear(book.publicationYear);

            CheckIfBookExists(book);


            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();
            
        }


        private void ValidateBookInput(Book book)
        {
            if (string.IsNullOrEmpty(book.Title) ||
                string.IsNullOrEmpty(book.AuthorName) ||
                string.IsNullOrEmpty(book.publicationYear))
            {
                throw new ArgumentException("Kitap adı, yazar adı ve yayınlama tarihi boş olamaz.");
            }
        }

        private void ValidatePublicationYear(string publicationYearStr)
        {
            if (!int.TryParse(publicationYearStr, out int publicationYear))
            {
                throw new ArgumentException("Yayınlama yılı geçerli bir sayı formatında olmalıdır.");
            }

            if (publicationYear < 1900 || publicationYear > 2100)
            {
                throw new ArgumentOutOfRangeException("Yayınlama yılı 1900 ile 2100 arasında olmalıdır.");
            }
        }

        private void CheckIfBookExists(Book book)
        {
            var exists = _context.Books
                            .Any(b => b.Title.ToLower() == book.Title.ToLower() &&
                                      b.AuthorName.ToLower() == book.AuthorName.ToLower() &&
                                      b.publicationYear == book.publicationYear);

            if (exists)
            {
                throw new InvalidOperationException("Bu kitap zaten mevcut.");
            }
        }



    }
}
