using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EFCoreLearningProject1.Models;
using Microsoft.VisualBasic;
namespace EFCoreLearningProject1.Services
{
    // Service class that handles all database operations related to Books
    // This follwows the Service Layer design pattern - separates business logic from controllers/UI
    public class BookService
    {
        private readonly LibraryContext _context;


        // Constructor - Called when creating an instance of BookService
        // Receives LibraryContext via Dependency Injection (DI)
        public BookService(LibraryContext context)
        {
            // Store the database context for use in all methods
            _context = context;
        }

        #region CRUD Operations 
        // Create: Add a new book to the database
        // async Task<Book> means: asynchronous method that returns a Book
        public async Task<Book> AddBookAsync(Book book)
        {
            //Add the book to the Books DbSet (marks it for insertion)
            _context.Books.Add(book);
            // Save changes to the database asynchronously
            // This generates and executes INSERT SQL command
            await _context.SaveChangesAsync();

            return book;
        }

        // READ: Get all books with related data (Eager Loading)
        public async Task<List<Book>> GetAllBookAsync()
        {
            // Start query on Books table
            return await _context.Books
                // include Author data (JOIN Authors table)
                // Without this, book.Author would be null
                .Include (b => b.Author)
                // Include Genre data (JOIN Genres table)
                .Include (b=> b.Genre)
                // Execute query and convert results to List
                .ToListAsync(); // Async version of ToList()
        }
        // READ: Get single book by ID
        // Book? means it can return null if not found
        public async Task<Book?> GetBookByIdAsync(int id)
        {
            // Query Books table
            return await _context.Books
                // Include related entities
                .Include(b=> b.Author)
                .Include(b => b.Genre)
                // Find first book where Id matches. or null is none found
                // FirstOrDefaultAsync is async version of FirstOrDefault
                .FirstOrDefaultAsync(b => b.Id == id); //Find book with matching ID
        }
        // READ: Get books by a specific author
        public async Task<List<Book>> GetBookByAuthorAsync(int authorId)
        {
            // Start query on Books table
            return await _context.Books
                // WHERE clause: filter by AuthorId
                // Lambda expression b => b.AuthorId == authorId   
                .Where(b => b.AuthorId == authorId)
                .Include(b => b.Author)
                .Include(b => b.Genre)
                // Execute query and convert results to List
                .ToListAsync();
        }

        // READ: Search books by title (partial match)
        public async Task<List<Book>> SearchBookByTitleAsync(string searchTerm)
        {
            // Cheak if search term is provided
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Book>(); // Return empty list if search term is null

            // Convert search term to lowercase for case-insensitive comparison
            string lowerSearchTerm = searchTerm.ToLower();

            return await _context.Books
                // WHERE Title contains search term (case-insensitive)
                // Contains translates to SQL LIKE '%searchterm%'
                .Where(b => b.Title.ToLower().Contains(lowerSearchTerm))
                // include another data
                .Include(b => b.Author)
                // Order results by Title A-Z
                .OrderBy(b => b.Title)
                .ToListAsync();
        }
        // UPDATE: Modify an existing book
        public async Task<Book?> UpdateBookAsync(int id, Book updatedBook)
        {
            // Find existing book by ID
            var book = await _context.Books.FindAsync(id);
            // If book not found , return null
            if (book == null)
                return null;

            // Update book properties one by one
            // We update each property instead of replacing entire object 
            // to preserve properties we don't want to change (like AddedDate)
            book.Title = updatedBook.Title;
            book.ISBN = updatedBook.ISBN;
            book.PublicationYear = updatedBook.PublicationYear;
            book.Price = updatedBook.Price;
            book.IsAvailable = updatedBook.IsAvailable;
            book.AuthorId = updatedBook.AuthorId;
            book.GenreId = updatedBook.GenreId;

            // Save changes to database
            // Generates UPDATE SQL command
            await _context.SaveChangesAsync();

            // Return the updated book 
            return book;
        }
        // DELETE: Remove a book by ID
        public async Task<bool> DeleteBookAsync(int id)
        {
            // Find book by Id
            var book = await _context.Books.FindAsync(id);

            // If book doesn't exist, return false
            if (book == null)
                return false;

            // Mark book for deletion
            _context.Books.Remove(book);

            // Execute DELETE command in database
            await _context.SaveChangesAsync();

            // Return true to indicate successful deletion
            return true;

        }
        #endregion
        #region Advanced Queries
        #endregion
    }
}
