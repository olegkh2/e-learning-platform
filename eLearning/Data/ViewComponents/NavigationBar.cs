using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Diagnostics;

namespace eLearning.Data.ViewComponents
{
    public class NavigationBar : ViewComponent
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        List<(int, string)> navList;
        public NavigationBar(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            navList = new List<(int, string)>();
        }
        public IViewComponentResult Invoke()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value.ToString();
            var courses = _context.Courses_Students.Include(n => n.Course).Where(n => n.UserId == userId);
            foreach (var course in courses)
                navList.Add((course.Course.Id, course.Course.Name));
            return View(navList);
        }
    }
}
