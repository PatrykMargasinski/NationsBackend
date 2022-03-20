using NationsAPI.Models;
using System.Collections.Generic;

namespace NationsAPI.Repositories {

    public interface ICrudRepository<T> where T: Model{
      
        public IList<T> GetAll();
        public T GetById(long id);
        //Probably two methods bellow can be rewrite as one method.
        public long Create(T model);
        public void Update(T model);
        public void Delete(T model);
        public void DeleteById(long id);
        public void DeleteByIds(long[] ids);

    }

}