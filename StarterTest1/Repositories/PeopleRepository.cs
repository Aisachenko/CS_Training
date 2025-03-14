using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WindowsFormsApp1.Data;
using WindowsFormsApp1.Model;

namespace WindowsFormsApp1.Repositories
{
    class PeopleRepository : IPeople
    {
        private readonly DataContext _context;

        public PeopleRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<People> GetAll()
        {
            return _context.people.ToList();
        }

        public People GetById(int id)
        {
            return _context.people.Find(id);
        }

        public void Add(People person)
        {
            _context.people.Add(person);
            _context.SaveChanges();
        }

        public void Update(People person)
        {
            _context.Entry(person).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var person = GetById(id);
            if (person != null)
            {
                _context.people.Remove(person);
                _context.SaveChanges();
            }
        }
    }
}
