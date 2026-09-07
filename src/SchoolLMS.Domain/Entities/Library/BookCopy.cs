using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Library;

public class BookCopy : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string Status { get; set; } = "Available";

    public Book? Book { get; set; }
}
