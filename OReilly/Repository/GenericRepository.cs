using Microsoft.EntityFrameworkCore;
using OReilly.Data;
using OReilly.IRepository;
using OReilly.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace OReilly.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DataBaseContext _context;
        private readonly DbSet<T> _db;

        //Dependecy inyection in the generic repository
        public GenericRepository(DataBaseContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task Delete(int id)
        {
            var entity = await _db.FindAsync(id);
            _db.Remove(entity);
        }



        public void DeleteRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
        }

        public async Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes = null)
        {
            IQueryable<T> query = _db;
            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }
            //AsNoTracking avoid to use extramemory. Is nice from read only functions
            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
        {
            IQueryable<T> query = _db;

            //This give us the first filter
            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            //AsNoTracking avoid to use extramemory. Is nice from read only functions
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<IPagedList<T>> GetAll(RequestParams requestParams, List<string> includes = null)
        {
            IQueryable<T> query = _db;

            //We can use query.Take to take the number that we want and query.Skip to skip the index (if we want the page 10 and 2 for page we skip 20 (2*10) and take 2)
            //Its options instead of use the Ipagedlist nuget to use the ToPagedListAsync
                
            if (includes != null)
            {
                foreach (var includeProperty in includes)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.AsNoTracking()
                .ToPagedListAsync(requestParams.PageNumber, requestParams.PageSize);//Now we return a page and not all in the database
        }

        public async Task Insert(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            //Its better because attach compare if any data is different and its optimize the updating
            _db.Attach(entity);
            //ww know wich fields are modified and we entries this ones.
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
