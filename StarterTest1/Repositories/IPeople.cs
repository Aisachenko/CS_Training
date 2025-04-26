using System.Collections.Generic;
using System.Linq;
using WindowsFormsApp1.Model;

namespace WindowsFormsApp1.Repositories
{
    public interface IPeople
    {
        IQueryable<People> GetAll();
        People GetById(int id);
        void Add(People person);
        void Update(People person);
        void Delete(int id);
        void TruncateTable();
    }
}
