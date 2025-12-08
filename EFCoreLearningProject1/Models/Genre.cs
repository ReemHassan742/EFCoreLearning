using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreLearningProject1.Models
{
    public class Genre
    {
        public int Id { get; set; }
        // Default to empty string to avoid null reference issues
        public string Name { get; set; } = string.Empty;

        // Navigation property for related books - defines one-to-many relationship
        // A genre can have multiple books. "ICollection<Book>" is interface, "List<Book> is the implementation.
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
