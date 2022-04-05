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
    [Authorize(Roles = UserRoles.Admin)]
    public class TopicsController : Controller
    {
        private readonly ITopicsService _service;

        public TopicsController(ITopicsService service)
        {
            _service = service;
        }

        public IActionResult Add(int id)
        {
            ViewBag.CourseId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add([Bind("Name,CourseId")] Topic topic)
        {
            if (!ModelState.IsValid)
            {
                return View(topic);
            }
            topic.Position = (short)await _service.GetMaxPosition(topic.CourseId);
            await _service.AddAsync(topic);

            return RedirectToAction("Index","Courses", new {id = topic.CourseId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var topic = await _service.GetByIdAsync(id);
            return View(topic);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Name")] Topic topic)
        {
            if (!ModelState.IsValid)
            {
                return View(topic);
            }
            await _service.UpdateAsync(id, topic);

            return RedirectToAction("Index", "Courses", new { id = await _service.GetCourseIdAsync(id) });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            int courseId = await _service.GetCourseIdAsync(id);
            await _service.DeleteAsync(id);

            return RedirectToAction("Index", "Courses", new { id = courseId });
        }

        [HttpPost]
        public async Task<IActionResult> Hide(int id)
        {
            await _service.HideAsync(id);

            return RedirectToAction("Index", "Courses", new { id = await _service.GetCourseIdAsync(id) });
        }
    }
}
