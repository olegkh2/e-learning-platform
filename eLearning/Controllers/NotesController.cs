using eLearning.Data.Services;
using eLearning.Data.Static;
using eLearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly INotesService _service;

        public NotesController(INotesService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Details(int id)
        {
            var note = await _service.GetByIdAsync(id);
            return View(note);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public IActionResult Add(int id)
        {
            ViewBag.CourseId = id;
            return View();
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Add([Bind("Name,Text,CourseId")] Note note)
        {
            if (!ModelState.IsValid)
            {
                return View(note);
            }
            note.Position = (short)await _service.GetMaxPosition(note.CourseId);
            await _service.AddAsync(note);

            return RedirectToAction("Index","Courses", new {id = note.CourseId });
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var note = await _service.GetByIdAsync(id);
            return View(note);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Text")] Note note)
        {
            if (!ModelState.IsValid)
            {
                return View(note);
            }
            await _service.UpdateAsync(id, note);

            return RedirectToAction("Index", "Courses", new { id = await _service.GetCourseIdAsync(id) });
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            int courseId = await _service.GetCourseIdAsync(id);
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
