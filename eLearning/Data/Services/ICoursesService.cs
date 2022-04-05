using eLearning.Data.Base;
using eLearning.Data.ViewModels;
using eLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Data.Services
{
    public interface ICoursesService
    {
        Task<IEnumerable<Course>> GetAllAsync();
        /// <summary>
        /// Get course with all existing items
        /// </summary>
        Task<Course> GetCourseByIdAsync(int id);
        /// <summary>
        /// Get course and all related users
        /// </summary>
        Task<Course> GetCourseAndUsersByIdAsync(int id);
        Task UpdateAsync(AddCourseVM newCourse);
        /// <summary>
        /// Moves an element up one position
        /// </summary>
        /// <param name="id">Item's id</param>
        /// <param name="pos">Current item's position</param>
        Task GetItemUp(int id, int pos);
        /// <summary>
        /// Moves an element down one position
        /// </summary>
        /// <param name="id">Item's id</param>
        /// <param name="pos">Current item's position</param>
        Task GetItemDown(int id, int pos);
    }
}
