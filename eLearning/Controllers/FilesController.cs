using eLearning.Data.Services;
using eLearning.Data.Static;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        private readonly IFilesService _service;
        private readonly IWebHostEnvironment _appEnvironment;

        public FilesController(IFilesService service, IWebHostEnvironment appEnvironment)
        {
            _service = service;
            _appEnvironment = appEnvironment;
        }
        
        public async Task<FileResult> Details(int id)
        {
            var file = await _service.GetByIdAsync(id);

            string file_path = Path.Combine("~\\Files\\", file.FileName);
            string file_type = MimeTypes.GetMimeType(file_path);
            string file_name = file.Name + Path.GetExtension(file_path);
            return File(file_path, file_type, file_name);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult Add(int id)
        {
            ViewBag.CourseId = id;
            return View();
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Add([Bind("Name,CourseId")] Models.File fileModel, IFormFile file)
        {
            if (!ModelState.IsValid)
            {     
                return View(fileModel);
            }

            if (file != null)
            {
                fileModel.Position = (short)await _service.GetMaxPosition(fileModel.CourseId);
                await _service.AddAsync(fileModel);
                string path = "/Files/CourseFile_" + fileModel.Id + "_" + file.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                fileModel.FileName = "CourseFile_" + fileModel.Id + "_" + file.FileName;
                await _service.UpdateAsync(fileModel.Id, fileModel);
            }
            else
            {
                return View(fileModel);
            }

            return RedirectToAction("Index", "Courses", new { id = fileModel.CourseId });
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var file = await _service.GetByIdAsync(id);
            return View(file);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Name")] Models.File fileModel, IFormFile file)
        {
            if (!ModelState.IsValid)
            {
                return View(fileModel);
            }
            if (file != null)
            {
                Models.File currentFile = await _service.GetByIdAsync(id);
                System.IO.File.Delete(_appEnvironment.WebRootPath + "/Files/" + currentFile.FileName);

                string path = "/Files/CourseFile_" + file.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                fileModel.FileName = "CourseFile_" + id + "_" + file.FileName;
            }
            await _service.UpdateAsync(id, fileModel);

            return RedirectToAction("Index", "Courses", new { id = await _service.GetCourseIdAsync(id) });
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Models.File file = await _service.GetByIdAsync(id);
            int courseId = file.CourseId;
            System.IO.File.Delete(_appEnvironment.WebRootPath + "/Files/" + file.FileName);
            await _service.DeleteAsync(id);

            return RedirectToAction("Index", "Courses", new { id = courseId });
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Hide(int id)
        {
            await _service.HideAsync(id);

            return RedirectToAction("Index", "Courses", new { id = await _service.GetCourseIdAsync(id) });
        }
    }
}
