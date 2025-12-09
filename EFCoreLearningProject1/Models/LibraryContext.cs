using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EFCoreLearningProject1.Models;

namespace EFCoreLearningProject1.Models
{
    // DbContext is the main class that coordinates EF Core functionality
    // Think of it as a bridge between your code and the database
    public class LibraryContext : DbContext //inherits from DbContext base class
    {
        //Dbset properties represent database tables
        //Each Dbset corresponds to a table in the database 
        public DbSet<Book> Books { get; set; } //will create a Books table

        public DbSet<Author> Authors { get; set; } //will create an Authors table

        public DbSet<Genre> Genres { get; set; } //will create a Genres table

        // Override this method to configure database provider and connection string

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure to use SQLite database with file "Library.db"
            // SQLite is a lightweight, file-based database
            optionsBuilder.UseSqlite("Data Source=Library.db");

            //shows parameters values in logs for debugging
            // We don't do this in production for security reasons
            optionsBuilder.EnableSensitiveDataLogging();

            //logs all EF Core operations to the console
            // Helps you see what SQL is being generated and executed.
            optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);

        }

        // Override to configure entity relationships, indexes, constraints, etc.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fluent API configurations can go here (alternative to attributes)

            // Configure Book -> Author relationship (many-to-one)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)  // Each Book has one Author
                .WithMany(a => a.Books)  // Each Author has many Books
                .HasForeignKey(b => b.AuthorId) // Foreign key in Book table
                .OnDelete(DeleteBehavior.Cascade); //Cascade delete books when author is deleted

            // Configure Book -> Genre relationship (many-to-one)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Genre)  // Each Book has one Genre
                .WithMany(g => g.Books)  // Each Genre has many Books
                .HasForeignKey(b => b.GenreId) // Foreign key in Book table
                .OnDelete(DeleteBehavior.SetNull); //Set GenreId to null when genre is deleted

            // Create unique index on ISBN to enforce uniqueness (No duplicate ISBNs allowed)
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)  //Create index on ISBN column
                .IsUnique();  // Makes it unique constraint

            // Create composite index on Author's FirstName and LastName for faster searches
            modelBuilder.Entity<Author>()
                .HasIndex(a => new { a.FirstName, a.LastName }); //Composite index

            // Seed initial data - inserts these records when database is created

            // Seed Genres table 
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Fiction", Description = "Fictional Literature" },
                new Genre { Id = 2, Name = "Science Fiction", Description = "Sci-Fi books" },
                new Genre { Id = 3, Name = "Mystery", Description = "Mystery and Thriller" },
                new Genre { Id = 4, Name = "Biography", Description = "Biographical works" }
                );

            // Seed Authors table
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FirstName = "George", LastName = "Orwell", Biography = "English novelist and essayist.", BirthDate = new DateTime(1903, 6, 25), country = "United Kingdom" },
                new Author { Id = 2, FirstName = "Isaac", LastName = "Asimov", Biography = "American science fiction writer and professor of biochemistry.", BirthDate = new DateTime(1920, 1, 2), country = "United States" },
                new Author { Id = 3, FirstName = "Agatha", LastName = "Christie", Biography = "English writer known for her detective novels.", BirthDate = new DateTime(1890, 9, 15), country = "United Kingdom" }
                );

            // Seed Books table (note foreign keys AuthorId and GenreId)
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "1984",
                    ISBN = "978-0451524935",
                    PublicationYear = 1949,
                    Price = 9.99m,
                    AuthorId = 1, //George Orwell
                    GenreId = 1  //Fiction
                },
                new Book
                { Id = 2,
                    Title = "Animal Farm",
                    ISBN = "978-0451526342",
                    PublicationYear = 1945,
                    Price = 7.99m,
                    AuthorId = 1, //George Orwell
                    GenreId = 1  //Fiction
                },
                new Book
                {
                    Id = 3,
                    Title = "Foundation",
                    ISBN = "978-0553293357",
                    PublicationYear = 1951,
                    Price = 8.99m,
                    AuthorId = 2, //Isaac Asimov
                    GenreId = 2  //Science Fiction
                },
                new Book
                {
                    Id = 4,
                    Title = "I, Robot",
                    ISBN = "978-0553382563",
                    PublicationYear = 1950,
                    Price = 6.99m,
                    AuthorId = 2, //Isaac Asimov
                    GenreId = 2  //Science Fiction
                }
                );
        }
    }
}
