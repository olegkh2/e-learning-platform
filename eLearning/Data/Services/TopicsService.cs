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
    public class TopicsService : EntityBaseRepository<Topic>, ITopicsService
    {
        public TopicsService(AppDbContext context) : base(context)
        {
        }

        public async Task UpdateAsync(int id, Topic entity)
        {
            var topic = await _context.Topics.FirstOrDefaultAsync(n => n.Id == id);
            topic.Name = entity.Name;

            await _context.SaveChangesAsync();
        }
    }
}
