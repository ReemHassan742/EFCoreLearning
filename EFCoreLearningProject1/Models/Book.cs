using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreLearningProject1.Models
{
    public class Book
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string ISBN {get; set; } = string.Empty; // International Standard Book Number.

        public int PublicationYear { get; set; }
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true; //Default value .

        public DateTime AddedDate { get; set; } = DateTime.UtcNow; //Default to current date and time.

        //Foreign Key property for Author and Genre relationships.
        //Convention: Property name "AuthorId" automatically links to Author entity.
        public int AuthorId { get; set; }
        public int GenreId { get; set; }

        // Navigation properties - refrence to related entities.
        // "Author" property will be populated with Author entity related to this Book.
        // "null!" tells compiler that we will ensure this is not null at runtime.
        public Author Author { get; set; } = null!;
        public Genre Genre { get; set; } = null!;


        // Computed property - display formatted book info.
        // "?." is null-conditional operator (safe if Author is null).
        //"??" is null-coalescing operator (use "Unkown" if left side is null).
        public String DisplayInfo => $"{Title} by {Author?.FullName ?? "Unknown"} ({PublicationYear})";
    }
}
