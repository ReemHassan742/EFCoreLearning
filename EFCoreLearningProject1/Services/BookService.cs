using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EFCoreLearningProject1.Models;
using Microsoft.VisualBasic;
using System.Globalization;
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
        // Query books within a price range 
        public async Task<List<Book>> GetBookByPriceRangeAsync (decimal minPrice, decimal maxPrice)
        {
            return await _context.Books
                // WHERE price between min and max
                .Where (b => b.Price >= minPrice && b.Price <= maxPrice)
                .Include (b => b.Author)
                // ORDER BY price ascending 
                .OrderBy (b => b.Price)
                .ToListAsync();
        }

        // Query books by publication year
        public async Task<List<Book>> GetBookByPublicationYearAsync (int year)
        {
            return await _context.Books
                .Where (b => b.PublicationYear == year)
                .Include (b => b.Author)
                .Include (b => b.Genre)
                .OrderBy (b => b.Title)
                .ToListAsync();
        }

        // Get books by Gerne name
        public async Task<List<Book>> GetBooksByGenreAsync (string genreName)
        {
            return await _context.Books
                // Join Genres table to filter by genre name
                .Where (b => b.Genre.Name.ToLower() == genreName.ToLower())
                .Include (b => b.Genre)
                .Include (b => b.Author)
                .OrderBy (b => b.Title)
                .ToListAsync();
        }
        // Get books published after a specific year
        public async Task<List<Book>> GetBooksPublishedAfterAsync (int year)
        {
            return await _context.Books
                .Where (b => b.PublicationYear > year)
                .Include (b => b.Author)    
                .OrderByDescending (b => b.PublicationYear)
                .ToListAsync();
        }
        #endregion
        #region Aggregate Queries
        // Get statistics about the library
        public async Task<Object> GetLibraryStatisticsAsync()
        {
            // Count all books - CountAsync execute SELECT COUNT(*) FROM Books
           var totalBooks = await _context.Books.CountAsync();

            // Count all authors
            var totalAuthors = await _context.Authors.CountAsync();

            // Count all genres
            var totalGenres = await _context.Genres.CountAsync();

            // Calculate average book price
            // AverageAsync generates SELECT AVG(Price) FROM Books
            var averagePrice = await _context.Books.AverageAsync(b => b.Price);

            // Find most expensive book
            var mostExpensiveBook = await _context.Books
                .Include(b => b.Author)
                .OrderByDescending(b => b.Price)  //Sort by price high to low
                .FirstOrDefaultAsync();     // Take the first one

            // Find cheapest book
            var cheapestBook = await _context.Books
                .Include(b => b.Author)
                .OrderBy(b => b.Price)  // Sort by price low to high
                .FirstOrDefaultAsync(); // Take the first one

            // Count available books vs unavailable books 
            var availableBooks = await _context.Books.CountAsync(b => b.IsAvailable);
            var unavailableBooks = totalBooks - availableBooks;


            // Group books by publication year 
            var booksByYear = await _context.Books
                .GroupBy(b => b.PublicationYear) // Group by year
                .Select(g => new        // Project each group
                {
                    Year = g.Key,
                    count = g.Count(),
                    AveragePrice = g.Average(b => b.Price)
                })
                .OrderByDescending(g => g.Year) // Newest year first
                .ToListAsync();

            // Group books by genre
            var booksByGenre = await _context.Genres
                .Select(g => new
                {
                    GenreName = g.Name,
                    BooksCount = g.Books.Count,
                    AveragePrice = g.Books.Average(b => b.Price)
                })
                .OrderByDescending(g => g.BooksCount)
                .ToListAsync();
            // Return anonymous object with all statistics
            return new
            {
                TotalBooks = totalBooks,
                TotalAuthors = totalAuthors,
                TotalGenres = totalGenres,
                AvailableBooks = availableBooks,
                UnavailableBooks = unavailableBooks,
                AverageBookPrice = Math.Round(averagePrice, 2),
                MostExpensiveBook = mostExpensiveBook?.DisplayInfo ?? "No books found",
                CheapestBook = cheapestBook?.DisplayInfo ?? "No books found",
                BooksByYear = booksByYear,
                BooksByGenre = booksByGenre
            };
        }
        // Get total value of all books in library
        public async Task<decimal> GetTotalLibraryValueAsync()
        {
            // Sum all book prices
            return await _context.Books.SumAsync(b => b.Price);
        }
        #endregion
        #region Raw SQL Queries
        // Example of raw SQL Query (When LINQ isn't enough)
        public async Task<List<Book>> GetBooksUsingRawSqlAsync(string searchTerm)
        {
            // Use FromSqlRaw to execute custom SQL query
            // {0} is parameter placeholder to prevent SQL injection
            return await _context.Books
                .FromSqlRaw("SELECT * FROM Books WHERE Title LIKE {0}", $"%{searchTerm}%")

                // Can still chain LINQ methods after raw SQL
                .Include(b => b.Author)
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        // Raw SQl for complex joins 
        public async Task<List<Book>> GetBooksByAuthorCountryAsync (String country)
        {
            return await  _context.Books
                .FromSqlRaw(@"
                      SELECT b.*
                      FROM Books b
                      INNER JOIN Authors a ON b.AuthorId = a.Id
                      WHERE a.Country = {0}", country)
                .Include (b => b.Author)
                .Include (b => b.Genre)
                .ToListAsync();
        }
        #endregion
        #region Transaction Examples 
        // Transfer book to new author (transaction ensures operations succeed or fail together)
        public async Task<bool> TransferBookToNewAuthorAsync(int bookId, int newAuthorId)
        {
            // Begin transaction - all operations will be atomic
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Find the book
                var book = await _context.Books.FindAsync(bookId);
                if (book == null)
                    return false; // Book not found

                // Find the new author
                var newAuthor = await _context.Authors.FindAsync(newAuthorId);
                if (newAuthor == null)
                    return false; // Author not found

                // Update book's author
                book.AuthorId = newAuthorId;

                // Optional : Add audit log 
                // await AddAuditLogAsync ($"Book {bookId} transferred to author {newAuthorId}"); 

                // Save changes
                await _context.SaveChangesAsync();

                // Commit transaction - all changes are saved
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex) 
            {
                // Rollback transaction - undo all changes
                await transaction.RollbackAsync();

                // log error 
                Console.WriteLine($"Error transferring book: {ex.Message}");

                return false;
            }
        }

        // Bulk update prices with transaction 
        public async Task<bool> ApplyDiscountToGenreAsync (int genreId, decimal discountPercent)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Get all books in the genre
                var books = await _context.Books
                    .Where (b => b.GenreId == genreId)
                    .ToListAsync();

                // Apply discount to each book
                foreach (var book in books)
                {
                    book.Price *= (1 - discountPercent / 100); // Reduce price by discount percent%
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        #endregion
        #region Bulk Operations
        // Add multiple books at once 
        public async Task<int> AddBooksBulkAsync (List<Book> books)
        {
            // Add all books to _context
            _context.Books.AddRange(books);

            // Save once for all books (more efficient than saving individually)
            return await _context.SaveChangesAsync();
        }
        // Delete multipe books by IDs 
        public async Task<int> DeleteBooksBulkAsync(List<int> bookIds)
        {
            // Find all books with IDs in the list 
            var booksToDelete = await _context.Books
               .Where(b => bookIds.Contains(b.Id))
               .ToListAsync();
            // Remove all found books
            _context.Books.RemoveRange(booksToDelete);

            // Save changes 
            return await _context.SaveChangesAsync();
        }

        #endregion
        #region Validation and business logic 
        // Check if ISBN already exists
        public async Task<bool> IsIsbnUniqueAsync (string isbn, int? excludeBookId = null)
        {
            // Query to check if ISBN exits 
            var query = _context.Books.Where(b => b.ISBN == isbn);

            // If excludeBookId provided, exclude that book (For updates)
            if (excludeBookId.HasValue)
            {
                query = query.Where(b => b.Id != excludeBookId.Value);

            }
            // AnyAsync returns true if any records match the query
            return !await query.AnyAsync();
        }

        // Validate book before adding/ updating
        public (bool IsValid, string ErrorMessage) ValidateBook(Book book)
        {
            // Check required fields
            if (string.IsNullOrWhiteSpace(book.Title))
                return (false, "Title is required.");
            if (string.IsNullOrWhiteSpace(book.ISBN))
                return (false, "ISBN is required.");
            if (book.PublicationYear < 1000 || book.PublicationYear > DateTime.Now.Year + 1)
                return (false, "Publication year is invalid.");
            if (book.Price < 0)
                return (false, "Price cannot be negative.");

            return (true, string.Empty);

        }
        #endregion
        #region Pagination Example
        //Pagination allows you to retrieve a large number of records split into pages, instead of returning all the results at once. 
        // This is especially useful in scenarios where you need to retrieve a large number of records.

        // Get books with pagination 
        public async Task<(List<Book> Books, int TotalCount, int TotalPages)> 
            GetBooksPaginatedAsync(int pageNumber, int pageSize, string sortBy = "Title")
        {
            // Start with base query 
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .AsQueryable(); // Keep as IQueryable for further filtering

            // Get total count for pagination info 
            int totalCount = await query.CountAsync();

            // Calculate total pages
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            // Apply sorting 
            query = sortBy.ToLower() switch
            {
                "title" => query.OrderBy(b => b.Title),
                "price" => query.OrderBy(b => b.Price),
                "year" => query.OrderByDescending(b => b.PublicationYear),
                "author" => query.OrderBy(b => b.Author.LastName).ThenBy(b => b.Author.FirstName),
                _ => query.OrderBy(b => b.Title), // Default sort by Title
            };

            // Apply pagination
            var books = await query 
                .Skip((pageNumber - 1) * pageSize) // Skip previous pages
                .Take(pageSize)
                .ToListAsync();

            return (books , totalCount, totalPages);
        }

        #endregion
        #region Chaching 
        //we can say that cache is a place where we store information to retrieve it in a very fast and efficient way.
        // Imagine you have an application with many users, all making the same query. Do we really want to hit the database every single time? Of course not. This is where cache comes into play.
        // Simple caching of expensive query results
        private List<Book> _cachedBooks;
        private DateTime _cacheLastUpdated = DateTime.MinValue;

        public async Task<List<Book>> GetBooksWithCachingAsync()
        {
            // If cache is fresh (less then 5 minutes old), return cached data
            if (_cachedBooks != null &&
                DateTime.Now.Subtract(_cacheLastUpdated).TotalMinutes < 5)
            {
                return _cachedBooks;

            }
            // Otherwise, query database
            _cachedBooks = await GetAllBookAsync();
            _cacheLastUpdated = DateTime.Now;

            return _cachedBooks;
        }

        #endregion
    }
}
