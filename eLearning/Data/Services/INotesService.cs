using eLearning.Data.Base;
using eLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eLearning.Data.Services
{
    public interface INotesService : IEntityBaseRepository<Note>
    {
        Task UpdateAsync(int id, Note entity);
    }
}
