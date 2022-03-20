using NationsAPI.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NationsAPI.Database;
using System.Linq;

namespace NationsAPI.Repositories
{
    public abstract class CrudRepository<T> : ICrudRepository<T> where T : Model
    {
        protected readonly NationsDbContext _context;
        protected DbSet<T> entities;
        public CrudRepository(NationsDbContext context)
        {
            _context = context;
            entities = _context.Set<T>();
        }

        public IList<T> GetAll()
        {
            return entities.ToList();
        }
        public T GetById(long id)
        {
            return entities.SingleOrDefault(s => s.Id == id);
        }
        public long Create(T model)
        {
            entities.Add(model);
            _context.SaveChanges();
            return model.Id;
        }
        public void Update(T model)
        {
            entities.Update(model);
            _context.SaveChanges();
        }
        public void Delete(T model)
        {
            entities.Remove(model);
            _context.SaveChanges();
        }

        public void DeleteById(long id)
        {
            entities.Remove(GetById(id));
            _context.SaveChanges();
        }

        public void DeleteByIds(long[] ids)
        {
            entities.RemoveRange(entities.Where(x=>ids.Contains(x.Id)));
            _context.SaveChanges();
        }
    }
}