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
    public class FilesService : EntityBaseRepository<File>, IFilesService
    {
        public FilesService(AppDbContext context) : base(context)
        {   
        }
        public async Task UpdateAsync(int id, File entity)
        {
            var file = await _context.Files.FirstOrDefaultAsync(n => n.Id == id);
            file.Name = entity.Name;
            if (entity.FileName != null) file.FileName = entity.FileName;

            await _context.SaveChangesAsync();
        }
    }
}
