using eLearning.Data.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using eLearning.Data;
using eLearning.Data.ViewModels;
using eLearning.Models;
using Microsoft.AspNetCore.Authorization;
using eLearning.Data.Static;

namespace eLearning.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ICoursesService _service;
        private readonly AppDbContext _context;
        public CoursesController(ICoursesService service, AppDbContext context)
        {
            _service = service;
            _context = context;
        }

        public async Task<IActionResult> Index(int id)
        {
            var course = await _service.GetCourseByIdAsync(id);
            return View(course);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> All()
        {
            var courses = await _service.GetAllAsync();
            return View(courses);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Add()
        {
            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "Id", "FullName");
            return View(new AddCourseVM());
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Add(AddCourseVM courseVM)
        {
            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "Id", "FullName");
            if (!ModelState.IsValid)
            {
                return View(courseVM);
            }

            var newCourse = new Course()
            {
                Name = courseVM.Name,
                Description = courseVM.Description,
            };

            await _context.Courses.AddAsync(newCourse);
            await _context.SaveChangesAsync();

            foreach (var course in courseVM.UserIds)
            {
                var newCourseStudent = new Course_Student()
                {
                    CourseId = newCourse.Id,
                    UserId = course,
                };
                await _context.Courses_Students.AddAsync(newCourseStudent);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _service.GetCourseAndUsersByIdAsync(id);
            var courseVM = new AddCourseVM()
            {
                Name = course.Name,
                Description = course.Description,
                UserIds = course.Courses_Students.Select(n => n.UserId).ToList(),
            };
            ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "Id", "FullName");

            return View(courseVM);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, AddCourseVM course)
        {
            
            if (!ModelState.IsValid)
            {
                ViewBag.Users = new SelectList(await _context.Users.ToListAsync(), "Id", "FullName");

                return View(course);
            }

            course.Id = id;
            await _service.UpdateAsync(course);
            return RedirectToAction(nameof(All));
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.Where(n => n.Id == id).FirstOrDefaultAsync();
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> ItemUp(int id, int position)
        {
            await _service.GetItemUp(id, position);
            return RedirectToAction("Index", "Courses", new { id = id });
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> ItemDown(int id, int position)
        {
            await _service.GetItemDown(id, position);
            return RedirectToAction("Index", "Courses", new { id = id });
        }

    }
}
