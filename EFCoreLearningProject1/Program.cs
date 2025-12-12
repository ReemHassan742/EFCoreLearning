using EFCoreLearningProject.Services;
using EFCoreLearningProject1.Models;
using EFCoreLearningProject1.Services;
using Microsoft.EntityFrameworkCore;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Library Management System ===\n");

        // Create database context
        using var context = new LibraryContext();

        // Seed the database
        var seeder = new DataSeeder(context);
        await seeder.SeedAsync();

        // Create book service
        var bookService = new BookService(context);

        // Show main menu
        await ShowMenu(bookService, context);
    }

    static async Task ShowMenu(BookService bookService, LibraryContext context)
    {
        while (true)
        {
            Console.WriteLine("\n=== MAIN MENU ===");
            Console.WriteLine("1. View all books");
            Console.WriteLine("2. Add a book");
            Console.WriteLine("3. Search books");
            Console.WriteLine("4. Delete a book");
            Console.WriteLine("5. View statistics");
            Console.WriteLine("6. Exit");
            Console.Write("\nChoose an option: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ViewAllBooks(bookService);
                    break;
                case "2":
                    await AddBook(context, bookService);
                    break;
                case "3":
                    await SearchBooks(bookService);
                    break;
                case "4":
                    await DeleteBook(bookService);
                    break;
                case "5":
                    Console.WriteLine("Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }

    static async Task ViewAllBooks(BookService bookService)
    {
        Console.WriteLine("\n=== ALL BOOKS ===\n");

        var books = await bookService.GetAllBookAsync();

        if (!books.Any())
        {
            Console.WriteLine("No books found.");
            return;
        }

        foreach (var book in books)
        {
            Console.WriteLine($"[{book.Id}] {book.Title}");
            Console.WriteLine($"   Author: {book.Author?.FullName}");
            Console.WriteLine($"   Genre: {book.Genre?.Name}");
            Console.WriteLine($"   Price: ${book.Price:F2}");
            Console.WriteLine($"   Year: {book.PublicationYear}");
            Console.WriteLine();
        }
    }

    static async Task AddBook(LibraryContext context, BookService bookService)
    {
        Console.WriteLine("\n=== ADD NEW BOOK ===\n");

        // Show available authors
        var authors = await context.Authors.ToListAsync();
        Console.WriteLine("Available Authors:");
        foreach (var author in authors)
        {
            Console.WriteLine($"{author.Id}. {author.FullName}");
        }

        Console.Write("\nSelect Author ID: ");
        if (!int.TryParse(Console.ReadLine(), out int authorId))
        {
            Console.WriteLine("Invalid author ID.");
            return;
        }

        // Show available genres
        var genres = await context.Genres.ToListAsync();
        Console.WriteLine("\nAvailable Genres:");
        foreach (var genre in genres)
        {
            Console.WriteLine($"{genre.Id}. {genre.Name}");
        }

        Console.Write("\nSelect Genre ID: ");
        if (!int.TryParse(Console.ReadLine(), out int genreId))
        {
            Console.WriteLine("Invalid genre ID.");
            return;
        }

        // Get book details
        Console.Write("\nBook Title: ");
        var title = Console.ReadLine();

        Console.Write("ISBN: ");
        var isbn = Console.ReadLine();

        Console.Write("Publication Year: ");
        if (!int.TryParse(Console.ReadLine(), out int year))
        {
            Console.WriteLine("Invalid year.");
            return;
        }

        Console.Write("Price: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
        {
            Console.WriteLine("Invalid price.");
            return;
        }

        // Create and save book
        var book = new Book
        {
            Title = title ?? "",
            ISBN = isbn ?? "",
            PublicationYear = year,
            Price = price,
            AuthorId = authorId,
            GenreId = genreId,
            IsAvailable = true,
            AddedDate = DateTime.UtcNow
        };

        try
        {
            await bookService.AddBookAsync(book);
            Console.WriteLine("\n✓ Book added successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async Task SearchBooks(BookService bookService)
    {
        Console.WriteLine("\n=== SEARCH BOOKS ===\n");
        Console.Write("Enter search term: ");
        var term = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(term))
        {
            Console.WriteLine("Please enter a search term.");
            return;
        }

        var books = await bookService.SearchBookByTitleAsync(term);

        if (!books.Any())
        {
            Console.WriteLine("No books found.");
            return;
        }

        Console.WriteLine($"\nFound {books.Count} book(s):");
        foreach (var book in books)
        {
            Console.WriteLine($"- {book.Title} by {book.Author?.FullName}");
        }
    }

    static async Task DeleteBook(BookService bookService)
    {
        Console.WriteLine("\n=== DELETE BOOK ===\n");
        Console.Write("Enter Book ID to delete: ");

        if (!int.TryParse(Console.ReadLine(), out int bookId))
        {
            Console.WriteLine("Invalid book ID.");
            return;
        }

        var success = await bookService.DeleteBookAsync(bookId);
        Console.WriteLine(success ? "✓ Book deleted!" : "✗ Book not found.");
    }
} 