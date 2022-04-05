using eLearning.Data.Services;
using eLearning.Data.Static;
using eLearning.Models;
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
using System.Security.Claims;
using System.Threading.Tasks;

namespace eLearning.Controllers
{
    [Authorize]
    public class ExercisesController : Controller
    {
        private readonly IExercisesService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _appEnvironment;

        public ExercisesController(IExercisesService service, IHttpContextAccessor httpContextAccessor, 
            IWebHostEnvironment appEnvironment)
        {
            _service = service;
            _httpContextAccessor = httpContextAccessor;
            _appEnvironment = appEnvironment;
        }

        public async Task<IActionResult> Details(int id)
        {
            Exercise exercise = null;
            if (User.IsInRole("User"))
            {
                exercise = await _service.GetByIdAsync(id, n => n.ExerciseResults);
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
                // Get user result
                exercise.ExerciseResults = exercise.ExerciseResults.Where(n => n.UserId == userId).ToList();
            }
            else if (User.IsInRole("Admin")) exercise = await _service.GetExerciseResultsAsync(id);
            return View(exercise);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult Add(int id)
        {
            ViewBag.CourseId = id;
            return View();
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Add([Bind("Name,Text,CourseId")] Exercise note)
        {
            if (!ModelState.IsValid)
            {
                return View(note);
            }
            note.Position = (short)await _service.GetMaxPosition(note.CourseId);
            await _service.AddAsync(note);

            return RedirectToAction("Index", "Courses", new { id = note.CourseId });
        }

        [HttpPost]
        public async Task<IActionResult> AddAnswer(int exerciseId, string text, IFormFile file)
        {
            if (text == null && file == null) return RedirectToAction(nameof(Details));

            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
            string fileName = null;
            if (file != null)
            {
                // File name pattern - "ExerciseResultFile_userId-exersureId_fileName"
                fileName = "ExerciseResultFile_" + userId + " - " + exerciseId + "_" + file.FileName;
                string path = "/Files/" + fileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            
            ExerciseResult result = new ExerciseResult()
            {
                ExerciseId = exerciseId,
                UserId = userId,
                Date = DateTime.Now,
                Answer = text,
                FileName = fileName,
            };

            await _service.AddExerciseResultAsync(result);

            return RedirectToAction("Details", "Exercises", new { id = exerciseId });
        }

        public async Task<FileResult> GetFileById(int id)
        {
            var exercise = await _service.GetByIdAsync(id, n => n.ExerciseResults);
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
            exercise.ExerciseResults = exercise.ExerciseResults.Where(n => n.UserId == userId).ToList();

            string file_path = Path.Combine("~\\Files\\", exercise.ExerciseResults[0].FileName);
            string file_type = MimeTypes.GetMimeType(file_path);
            string file_name = "ExerciseResult" + Path.GetExtension(file_path);

            return File(file_path, file_type, file_name);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public FileResult GetFileByName(string fileName)
        {
            string file_path = Path.Combine("~\\Files\\", fileName);
            string file_type = MimeTypes.GetMimeType(file_path);
            string file_name = "ExerciseResult" + Path.GetExtension(file_path);

            return File(file_path, file_type, file_name);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(List<ExerciseResult> ExerciseResult)
        {
            foreach (var item in ExerciseResult)
            {
                await _service.UpdateResult(item);
            }
            return RedirectToAction("Details", "Exercises", new { id = ExerciseResult[0].ExerciseId });
        }
    }
}
