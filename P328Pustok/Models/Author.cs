using P328Pustok.Attributes.ValidationAttributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P328Pustok.Models
{
    public class Author
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string FullName { get; set; }
        [NotMapped]
        [CheckFileType("image/jpeg", "image/png")]
        [MaxFileSize(5242880)]
        public IFormFile AuthorImage { get; set; }
        public string ImgName { get; set; }

        public List<Book> Books { get; set; }
    }
}
