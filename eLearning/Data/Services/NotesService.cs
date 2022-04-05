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
    public class NotesService : EntityBaseRepository<Note>, INotesService
    {
        public NotesService(AppDbContext context) : base(context)
        {
        }

        public async Task UpdateAsync(int id, Note entity)
        {
            var note = await _context.Notes.FirstOrDefaultAsync(n => n.Id == id);
            note.Name = entity.Name;
            note.Text = entity.Text;

            await _context.SaveChangesAsync();
        }
    }
}
