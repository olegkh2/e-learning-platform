using eLearning.Data;
using eLearning.Data.Static;
using eLearning.Data.ViewModels;
using eLearning.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AppDbContext _context;
        public AccountController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginVM());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid) return View(loginVM);

            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            if(user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, loginVM.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    ViewData["Error"] = "Something went wrong, sorry...";
                }
                ViewData["Error"] = "Wrong cadentials!";
                return View(loginVM);
            }

            ViewData["Error"] = "Wrong cadentials!";
            return View(loginVM);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Users()
        {
            var students = await _context.Users.Include(n => n.Courses_Students).ThenInclude(n => n.Course)
                .ToListAsync();
            return View(students);
        }

        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Add()
        {
            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(),"Id","Name");
            return View(new AddAccountVM());
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Add(AddAccountVM accountVM)
        {
            ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name");
            if (!ModelState.IsValid)
            {
                List<string> errorList = new List<string>();
                foreach (var item in ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        errorList.Add(error.ToString());
                    }
                }
                ViewBag.Errors = errorList;
                return View(accountVM);
            }

            var user = await _userManager.FindByEmailAsync(accountVM.Email);
            if(user != null)
            {
                List<string> errorList = new List<string>() {
                "User with this email already exists",
                };
                ViewBag.Errors = errorList;
                return View(accountVM);
            }

            var newUser = new ApplicationUser()
            {
                FullName = accountVM.FullName,
                Email = accountVM.Email,
                UserName = accountVM.Email,
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, accountVM.Password);
            if (newUserResponse.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, UserRoles.User);
            }
            else
            {
                List<string> errorList = new List<string>();
                foreach (var item in newUserResponse.Errors)
                {
                    errorList.Add(item.Description.ToString());
                }
                ViewBag.Errors = errorList;
                return View(accountVM);
            }

            if(accountVM.CourseIds == null)
                return RedirectToAction(nameof(Users));

            foreach (var course in accountVM.CourseIds)
            {
                var newCourseStudent = new Course_Student()
                {
                    CourseId = course,
                    UserId = newUser.Id,
                };
                await _context.Courses_Students.AddAsync(newCourseStudent);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Users));
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _context.Users.Where(n=>n.Id == id).FirstOrDefaultAsync();
            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
