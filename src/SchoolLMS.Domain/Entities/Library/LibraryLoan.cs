using SchoolLMS.Domain.Common;

namespace SchoolLMS.Domain.Entities.Library;

public class LibraryLoan : SchoolOwnedEntity
{
    public int Id { get; set; }
    public int BookCopyId { get; set; }
    public int StudentId { get; set; }
    public DateOnly BorrowedOn { get; set; }
    public DateOnly DueOn { get; set; }
    public DateOnly? ReturnedOn { get; set; }
    public decimal PenaltyAmount { get; set; }
    public string Status { get; set; } = "Borrowed";

    public BookCopy? BookCopy { get; set; }
}
