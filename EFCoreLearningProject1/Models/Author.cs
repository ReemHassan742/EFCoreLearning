using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EFCoreLearningProject1.Models
{
    public class Author
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        //"String?" means this can be null (nullable reference type), Biography is optional.
        public string? Biography { get; set; }

        public DateTime? BirthDate { get; set; }
        public string? Country { get; set; }

        //Navigation property : One Author can have multiple Books.
        public ICollection<Book> Books { get; set; } = new List<Book>();


        // Computed/read-only property - Not sorted in database.
        // "=>" is expression-bodied property (Shorthand for {get {return ..} } ).
        public string FullName => $"{FirstName} {LastName}";
    }
}
