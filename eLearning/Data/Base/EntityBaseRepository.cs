using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace eLearning.Data.Base
{
    public class EntityBaseRepository<T> : IEntityBaseRepository<T> where T: CourseEntity
    {
        protected readonly AppDbContext _context;
        public EntityBaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.Set<T>().FirstOrDefaultAsync(n => n.Id == id);
            var courseId = entity.CourseId;
            var position = entity.Position;
            _context.Set<T>().Remove(entity);

            //Decrease position of all following items
            var topics = await _context.Topics.Where(n => n.CourseId == courseId && n.Position > position).ToListAsync();
            var notes = await _context.Notes.Where(n => n.CourseId == courseId && n.Position > position).ToListAsync();
            var files = await _context.Files.Where(n => n.CourseId == courseId && n.Position > position).ToListAsync();
            var exersices = await _context.Exercises.Where(n => n.CourseId == courseId && n.Position > position).ToListAsync();
            for (int i = ++position, t = 0, n = 0, f = 0, e = 0 ; ; i++)
            {
                if (topics.Count > t && topics[t].Position == i)
                {
                    topics[t].Position = (short)(i - 1);
                    t++;
                }
                else if (notes.Count > n && notes[n].Position == i)
                {
                    notes[n].Position = (short)(i - 1);
                    n++;
                }
                else if (files.Count > f && files[f].Position == i)
                {
                    files[f].Position = (short)(i - 1);
                    f++;
                }
                else if (exersices.Count > e && exersices[e].Position == i)
                {
                    exersices[e].Position = (short)(i - 1);
                    e++;
                }
                else break;
            }
            await _context.SaveChangesAsync();
        }

        public async Task HideAsync(int id)
        {
            var entity = await _context.Set<T>().FirstOrDefaultAsync(n => n.Id == id);
            entity.Visible = !entity.Visible;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return await query.FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<int> GetMaxPosition(int id)
        {
            int notesPosition, filesPosition, topicsPosition, exercisePosition;
            notesPosition = filesPosition = topicsPosition = exercisePosition = 0;
            if (await _context.Notes.Where(n => n.CourseId == id).CountAsync() > 0)
                notesPosition = await _context.Notes.Where(n => n.CourseId == id).MaxAsync(n => n.Position);
            if (await _context.Files.Where(n => n.CourseId == id).CountAsync() > 0)
                filesPosition = await _context.Files.Where(n => n.CourseId == id).MaxAsync(n => n.Position);
            if (await _context.Topics.Where(n => n.CourseId == id).CountAsync() > 0)
                topicsPosition = await _context.Topics.Where(n => n.CourseId == id).MaxAsync(n => n.Position);
            if (await _context.Exercises.Where(n => n.CourseId == id).CountAsync() > 0)
                topicsPosition = await _context.Exercises.Where(n => n.CourseId == id).MaxAsync(n => n.Position);
            int max = (new int[] { notesPosition, filesPosition, topicsPosition, exercisePosition }).Max();
            return ++max;
        }

        public async Task<int> GetCourseIdAsync(int id)
        {
            var entity = await _context.Set<T>().FirstOrDefaultAsync(n => n.Id == id);
            return entity.CourseId;
        }
    }
}
