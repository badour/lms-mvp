using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolLMS.Application.Services.Lessons;
using SchoolLMS.Application.Services.Students;
using SchoolLMS.Domain.Interfaces;

namespace SchoolLMS.Web.Controllers;

[Authorize]
public class FilesController : Controller
{
    private readonly IQimamCertificateService _certificateService;
    private readonly ILessonService _lessonService;
    private readonly IFileStorage _fileStorage;

    public FilesController(
        IQimamCertificateService certificateService,
        ILessonService lessonService,
        IFileStorage fileStorage)
    {
        _certificateService = certificateService;
        _lessonService = lessonService;
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

    [HttpGet]
    public async Task<IActionResult> LessonVideo(int id, CancellationToken cancellationToken = default)
    {
        var meta = await _lessonService.GetVideoFileAsync(id, cancellationToken);
        if (meta is null)
        {
            return NotFound();
        }

        var stream = await _fileStorage.OpenReadAsync(meta.RelativePath, cancellationToken);
        return File(stream, meta.ContentType);
    }

    [HttpGet]
    public async Task<IActionResult> LessonMaterial(int id, CancellationToken cancellationToken = default)
    {
        var meta = await _lessonService.GetMaterialFileAsync(id, cancellationToken);
        if (meta is null)
        {
            return NotFound();
        }

        var stream = await _fileStorage.OpenReadAsync(meta.RelativePath, cancellationToken);
        return File(stream, meta.ContentType, meta.OriginalFileName);
    }
}
