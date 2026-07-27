using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolLMS.Application.Services.Students;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Web.Controllers;

[Authorize]
public class FilesController : Controller
{
    private readonly IQimamCertificateService _certificateService;
    private readonly IFileStorage _fileStorage;

    public FilesController(IQimamCertificateService certificateService, IFileStorage fileStorage)
    {
        _certificateService = certificateService;
        _fileStorage = fileStorage;
    }

    [HttpGet]
    public async Task<IActionResult> QimamCertificate(int id, string kind = "document", CancellationToken cancellationToken = default)
    {
        var meta = await _certificateService.GetFileAsync(id, kind, cancellationToken);
        if (meta is null)
        {
            return NotFound();
        }

        var stream = await _fileStorage.OpenReadAsync(meta.RelativePath, cancellationToken);
        if (meta.IsImage)
        {
            return File(stream, meta.ContentType);
        }

        return File(stream, meta.ContentType, meta.OriginalFileName);
    }
}
