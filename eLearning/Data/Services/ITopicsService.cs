using eLearning.Data.Base;
using eLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Data.Services
{
    public interface ITopicsService : IEntityBaseRepository<Topic>
    {
        Task UpdateAsync(int id, Topic entity);
    }
}
