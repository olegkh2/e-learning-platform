using eLearning.Data.Base;
using eLearning.Data.ViewModels;
using eLearning.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace eLearning.Data.Services
{
    public class CoursesService : ICoursesService
    {
        private readonly AppDbContext _context;
        public CoursesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            var courses = await _context.Courses
                .Include(n => n.Courses_Students)
                .ThenInclude(n=>n.User)
                .ToListAsync();
            return courses;
        }

        public async Task<Course> GetCourseByIdAsync(int id)
        {
            var courseDetails = await _context.Courses
                .Include(n => n.Notes)
                .Include(n => n.Files)
                .Include(n => n.Topics)
                .Include(n => n.Exercises)
                .FirstOrDefaultAsync(n => n.Id == id);
            if(courseDetails.Topics != null)
                courseDetails.Topics = courseDetails.Topics.OrderBy(n => n.Position).ToList();
            if (courseDetails.Files != null)
                courseDetails.Files = courseDetails.Files.OrderBy(n => n.Position).ToList();
            if (courseDetails.Notes != null)
                courseDetails.Notes = courseDetails.Notes.OrderBy(n => n.Position).ToList();
            if (courseDetails.Exercises != null)
                courseDetails.Exercises = courseDetails.Exercises.OrderBy(n => n.Position).ToList();
            return courseDetails;
        }

        public async Task<Course> GetCourseAndUsersByIdAsync(int id)
        {
            var courseDetails = await _context.Courses
               .Include(n => n.Courses_Students)
               .ThenInclude(n => n.User)
               .FirstOrDefaultAsync(n => n.Id == id);
            return courseDetails;
        }

        public async Task UpdateAsync(AddCourseVM newCourse)
        {
            var dbCourse = await _context.Courses.FirstOrDefaultAsync(n => n.Id == newCourse.Id);
            if (dbCourse != null)
            {
                dbCourse.Name = newCourse.Name;
                dbCourse.Description = newCourse.Description;
                await _context.SaveChangesAsync();
            }

            //Remove existing students
            var existingStudents = _context.Courses_Students.Where(n => n.CourseId == newCourse.Id).ToList();
            _context.Courses_Students.RemoveRange(existingStudents);
            await _context.SaveChangesAsync();

            //Add new students
            foreach (var studentId in newCourse.UserIds)
            {
                var newCourseStudent = new Course_Student()
                {
                    CourseId = dbCourse.Id,
                    UserId = studentId,
                };
                await _context.Courses_Students.AddAsync(newCourseStudent);
            }
            await _context.SaveChangesAsync();
        }

        public async Task GetItemUp(int id, int pos)
        {
            await ChangePosition(id, pos, -1);
            await ChangePosition(id, (pos - 1), pos);
            await ChangePosition(id, -1, (pos - 1));
        }

        public async Task GetItemDown(int id, int pos)
        {
            await ChangePosition(id, pos, -1);
            await ChangePosition(id, (pos + 1), pos);
            await ChangePosition(id, -1, (pos + 1));
        }

        private async Task ChangePosition(int courseid, int oldPos, int newPos)
        {
            if (await _context.Notes.FirstOrDefaultAsync(n => n.CourseId == courseid && n.Position == oldPos) != null)
            {
                var note = await _context.Notes.FirstOrDefaultAsync(n => n.CourseId == courseid && n.Position == oldPos);
                note.Position = (short)newPos;
            }
            else if (await _context.Files.FirstOrDefaultAsync(n => n.CourseId == courseid && n.Position == oldPos) != null)
            {
                var file = await _context.Files.FirstOrDefaultAsync(n => n.CourseId == courseid && n.Position == oldPos);
                file.Position = (short)newPos;
            }
            else if (await _context.Topics.FirstOrDefaultAsync(n => n.CourseId == courseid && n.Position == oldPos) != null)
            {
                var topic = await _context.Topics.FirstOrDefaultAsync(n => n.CourseId == courseid && n.Position == oldPos);
                topic.Position = (short)newPos;
            }
            else if (await _context.Exercises.FirstOrDefaultAsync(n => n.CourseId == courseid && n.Position == oldPos) != null)
            {
                var exercise = await _context.Exercises.FirstOrDefaultAsync(n => n.CourseId == courseid && n.Position == oldPos);
                exercise.Position = (short)newPos;
            }
            await _context.SaveChangesAsync();
        }
    }
}
