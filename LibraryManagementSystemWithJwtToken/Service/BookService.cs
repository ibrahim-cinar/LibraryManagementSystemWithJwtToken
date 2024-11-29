using LibraryManagementSystemWithJwtToken.Models;
using LibraryManagementSystemWithJwtToken.Service.Interfaces;
using LibraryManagementSystemWithJwtToken.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemWithJwtToken.Service
{
    public class BookService : IBookService
    {
        private readonly AppDbContext _context;

        public BookService(AppDbContext context)
        {
            _context = context;
        }


        public List<Book> GetAllBooks()
        {
            return _context.Books.ToList();
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

        public Book addBook(Book book)
        {
            if (string.IsNullOrEmpty(book.Title) || string.IsNullOrEmpty(book.AuthorName) || string.IsNullOrEmpty(book.publicationYear))
            {
                throw new ArgumentException("Kitap adı , yazar adı ve yayınlama tarihi boş olamaz.");
            }

            var existingBook = _context.Books
                .FirstOrDefault(b => b.Title.Equals(book.Title, StringComparison.OrdinalIgnoreCase) &&
                                     b.AuthorName.Equals(book.AuthorName, StringComparison.OrdinalIgnoreCase));
            if (existingBook != null)
            {
                throw new InvalidOperationException("Bu kitap zaten mevcut.");
            }

            _context.Books.Add(book);
            _context.SaveChanges();
            return book;
        }
    }
    }
