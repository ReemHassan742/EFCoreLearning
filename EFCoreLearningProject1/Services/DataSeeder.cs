using EFCoreLearningProject1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreLearningProject.Services;

public class DataSeeder
{
    private readonly LibraryContext _context;

    public DataSeeder(LibraryContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        Console.WriteLine("=== Database Seeding ===");

        // Create database if it doesn't exist
        await _context.Database.EnsureCreatedAsync();

        // Seed data only if tables are empty
        await SeedAuthors();
        await SeedGenres();
        await SeedBooks();

        Console.WriteLine(" Seeding completed!");
    }

    private async Task SeedAuthors()
    {
        // Only seed if no authors exist
        if (await _context.Authors.AnyAsync())
        {
            Console.WriteLine("Authors already exist, skipping...");
            return;
        }

        Console.WriteLine("Adding authors...");

        var authors = new List<Author>
        {
            new Author { FirstName = "George", LastName = "Orwell", Country = "UK" },
            new Author { FirstName = "Isaac", LastName = "Asimov", Country = "USA" },
            new Author { FirstName = "Agatha", LastName = "Christie", Country = "UK" },
            new Author { FirstName = "J.K.", LastName = "Rowling", Country = "UK" },
            new Author { FirstName = "Stephen", LastName = "King", Country = "USA" }
        };

        _context.Authors.AddRange(authors);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Added {authors.Count} authors");
    }

    private async Task SeedGenres()
    {
        // Only seed if no genres exist
        if (await _context.Genres.AnyAsync())
        {
            Console.WriteLine("Genres already exist, skipping...");
            return;
        }

        Console.WriteLine("Adding genres...");

        var genres = new List<Genre>
        {
            new Genre { Name = "Fiction", Description = "Fictional books" },
            new Genre { Name = "Science Fiction", Description = "Sci-fi books" },
            new Genre { Name = "Mystery", Description = "Mystery books" },
            new Genre { Name = "Fantasy", Description = "Fantasy books" },
            new Genre { Name = "Horror", Description = "Horror books" }
        };

        _context.Genres.AddRange(genres);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Added {genres.Count} genres");
    }

    private async Task SeedBooks()
    {
        // Only seed if no books exist
        if (await _context.Books.AnyAsync())
        {
            Console.WriteLine("Books already exist, skipping...");
            return;
        }

        Console.WriteLine("Adding books...");

        // Get all authors and genres to use their IDs
        var authors = await _context.Authors.ToListAsync();
        var genres = await _context.Genres.ToListAsync();

        // Get the IDs we need (simplest approach - just use what we know exists)
        var orwell = authors.First(a => a.LastName == "Orwell");
        var asimov = authors.First(a => a.LastName == "Asimov");
        var christie = authors.First(a => a.LastName == "Christie");
        var rowling = authors.First(a => a.LastName == "Rowling");
        var king = authors.First(a => a.LastName == "King");

        var fiction = genres.First(g => g.Name == "Fiction");
        var scifi = genres.First(g => g.Name == "Science Fiction");
        var mystery = genres.First(g => g.Name == "Mystery");
        var fantasy = genres.First(g => g.Name == "Fantasy");
        var horror = genres.First(g => g.Name == "Horror");

        var books = new List<Book>
        {
            // George Orwell books
            new Book {
                Title = "1984",
                ISBN = "978-0451524935",
                PublicationYear = 1949,
                Price = 9.99m,
                AuthorId = orwell.Id,
                GenreId = fiction.Id,
                IsAvailable = true
            },
            new Book {
                Title = "Animal Farm",
                ISBN = "978-0451526342",
                PublicationYear = 1945,
                Price = 7.99m,
                AuthorId = orwell.Id,
                GenreId = fiction.Id,
                IsAvailable = true
            },
            
            // Isaac Asimov books
            new Book {
                Title = "Foundation",
                ISBN = "978-0553293357",
                PublicationYear = 1951,
                Price = 12.99m,
                AuthorId = asimov.Id,
                GenreId = scifi.Id,
                IsAvailable = true
            },
            
            // Agatha Christie books
            new Book {
                Title = "Murder on the Orient Express",
                ISBN = "978-0062693662",
                PublicationYear = 1934,
                Price = 8.99m,
                AuthorId = christie.Id,
                GenreId = mystery.Id,
                IsAvailable = true
            },
            
            // J.K. Rowling books
            new Book {
                Title = "Harry Potter and the Philosopher's Stone",
                ISBN = "978-0747532743",
                PublicationYear = 1997,
                Price = 15.99m,
                AuthorId = rowling.Id,
                GenreId = fantasy.Id,
                IsAvailable = true
            },
            
            // Stephen King books
            new Book {
                Title = "The Shining",
                ISBN = "978-0307743657",
                PublicationYear = 1977,
                Price = 11.99m,
                AuthorId = king.Id,
                GenreId = horror.Id,
                IsAvailable = true
            }
        };

        _context.Books.AddRange(books);
        await _context.SaveChangesAsync();

        Console.WriteLine($"Added {books.Count} books");
    }
}