namespace SchoolLMS.Domain.Enums;

public enum Gender
{
    Male = 1,
    Female = 2
}

public enum SchoolType
{
    Private = 1,
    International = 2,
    Mixed = 3
}

public enum GenderType
{
    Boys = 1,
    Girls = 2,
    CoEducational = 3
}

public enum AcademicYearStatus
{
    Draft = 1,
    Open = 2,
    Closed = 3,
    Frozen = 4
}

public enum EnrollmentStatus
{
    Active = 1,
    Promoted = 2,
    Repeated = 3,
    Transferred = 4,
    Withdrawn = 5,
    Frozen = 6
}

public enum StudentStatus
{
    Active = 1,
    Inactive = 2,
    Graduated = 3,
    Withdrawn = 4,
    Suspended = 5
}

public enum PublicationStatus
{
    Draft = 1,
    Published = 2,
    Archived = 3,
    Cancelled = 4
}

public enum AssignmentStatus
{
    Draft = 1,
    Published = 2,
    Pending = 3,
    Submitted = 4,
    SubmittedLate = 5,
    Reviewed = 6,
    ReturnedForCorrection = 7,
    Missing = 8,
    Cancelled = 9
}

public enum SubmissionType
{
    Text = 1,
    File = 2,
    Image = 3,
    Pdf = 4,
    Link = 5,
    None = 6,
    MultipleFiles = 7
}

public enum QuestionType
{
    MultipleChoice = 1,
    MultipleAnswer = 2,
    TrueFalse = 3,
    FillInBlank = 4,
    ShortAnswer = 5,
    Essay = 6,
    Matching = 7,
    Ordering = 8
}

public enum ExamType
{
    DailyQuiz = 1,
    Monthly = 2,
    Midterm = 3,
    Final = 4,
    Practical = 5,
    Oral = 6,
    Supplementary = 7
}

public enum GradeWorkflowStatus
{
    Draft = 1,
    Entered = 2,
    SubmittedForReview = 3,
    Approved = 4,
    Published = 5,
    Locked = 6
}

public enum AttendanceStatus
{
    Present = 1,
    Absent = 2,
    Late = 3,
    ExcusedAbsence = 4,
    SickLeave = 5,
    OfficialLeave = 6,
    LeftEarly = 7,
    Suspended = 8,
    NotRecorded = 9
}

public enum AnnouncementPriority
{
    Normal = 1,
    Important = 2,
    Urgent = 3,
    Emergency = 4
}

public enum PaymentStatus
{
    Pending = 1,
    Paid = 2,
    PartiallyPaid = 3,
    Cancelled = 4,
    Refunded = 5
}

public enum NotificationDeliveryStatus
{
    Pending = 1,
    Sent = 2,
    Delivered = 3,
    Failed = 4,
    Read = 5
}

public enum StudentMessageTargetType
{
    SystemAdmin = 1,
    SchoolManagement = 2,
    Instructor = 3
}

public enum BehaviourPolarity
{
    Positive = 1,
    Negative = 2,
    Neutral = 3
}

public enum MeetingStatus
{
    Available = 1,
    Booked = 2,
    Approved = 3,
    Completed = 4,
    Cancelled = 5,
    Rescheduled = 6
}
