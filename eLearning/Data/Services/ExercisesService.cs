using eLearning.Data.Base;
using eLearning.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace eLearning.Data.Services
{
    public class ExercisesService : EntityBaseRepository<Exercise>, IExercisesService
    {
        public ExercisesService(AppDbContext context) : base(context)
        {
        }

        public async Task<Exercise> GetExerciseResultsAsync(int id)
        {
            var exercise = await _context.Exercises.Include(n => n.ExerciseResults).ThenInclude(n => n.User)
                .FirstOrDefaultAsync(n => n.Id == id);
            return exercise;
        }

        public async Task AddExerciseResultAsync(ExerciseResult result)
        {
            await _context.ExerciseResults.AddAsync(result);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateResult(ExerciseResult result)
        {
            var exercise = await _context.ExerciseResults
                .FirstOrDefaultAsync(n => n.UserId == result.UserId && n.ExerciseId == result.ExerciseId);

            exercise.Comment = result.Comment;
            exercise.Grade = result.Grade;

            await _context.SaveChangesAsync();
        }

    }
}
