SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER VIEW dbo.vw_DashboardEnrollmentStats
AS
SELECT
    c.CourseID,
    c.CourseTitle,
    COUNT(e.EnrollmentID) AS TotalEnrollments,
    SUM(CASE WHEN e.EnrollmentStatus = 'Completed' THEN 1 ELSE 0 END) AS CompletedEnrollments,
    AVG(CAST(e.CompletionPercentage AS DECIMAL(9,2))) AS AvgCompletionPercent
FROM dbo.Courses c
LEFT JOIN dbo.Enrollments e ON e.CourseID = c.CourseID
GROUP BY c.CourseID, c.CourseTitle;
GO

CREATE OR ALTER VIEW dbo.vw_DashboardExamPerformance
AS
SELECT
    a.AssessmentID,
    a.Title AS AssessmentTitle,
    a.CourseID,
    COUNT(es.SessionID) AS TotalAttempts,
    SUM(CASE WHEN es.ResultStatus = 'Passed' THEN 1 ELSE 0 END) AS PassedAttempts,
    SUM(CASE WHEN es.ResultStatus = 'Failed' THEN 1 ELSE 0 END) AS FailedAttempts,
    AVG(CAST(es.Score AS DECIMAL(9,2))) AS AvgScore
FROM dbo.Assessments a
LEFT JOIN dbo.ExamSessions es ON es.ExamID = a.AssessmentID
GROUP BY a.AssessmentID, a.Title, a.CourseID;
GO

CREATE OR ALTER VIEW dbo.vw_DashboardRevenue
AS
SELECT
    CAST(PaymentDate AS DATE) AS RevenueDate,
    SUM(CASE WHEN PaymentStatus = 'Paid' THEN Amount ELSE 0 END) AS PaidRevenue,
    SUM(CASE WHEN PaymentStatus = 'Pending' THEN Amount ELSE 0 END) AS PendingRevenue,
    SUM(CASE WHEN PaymentStatus = 'Refunded' THEN Amount ELSE 0 END) AS RefundedAmount
FROM dbo.Payments
GROUP BY CAST(PaymentDate AS DATE);
GO

CREATE OR ALTER VIEW dbo.vw_CertificateStatusSummary
AS
SELECT
    CertificateStatus,
    COUNT(*) AS TotalCertificates
FROM dbo.Certificates
GROUP BY CertificateStatus;
GO
