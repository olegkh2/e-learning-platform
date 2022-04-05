using eLearning.Data.Base;
using eLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Data.Services
{
    public interface IExercisesService : IEntityBaseRepository<Exercise>
    {
        Task<Exercise> GetExerciseResultsAsync(int id);
        Task AddExerciseResultAsync(ExerciseResult result);
        Task UpdateResult(ExerciseResult result);
    }
}
