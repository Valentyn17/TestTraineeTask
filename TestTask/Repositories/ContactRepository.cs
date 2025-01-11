using Microsoft.EntityFrameworkCore;
using TestTask.Context;
using TestTask.Models;

namespace TestTask.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly TestTaskDBContext _context;

        public ContactRepository(TestTaskDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Contact>> GetAllAsync()
        {
            return await _context.Contacts.ToListAsync();
        }

        public async Task<Contact> GetByIdAsync(int id)
        {
            return await _context.Contacts.FindAsync(id);
        }

        public async Task AddAsync(Contact contact)
        {
            await _context.Contacts.AddAsync(contact);
        }

        public async Task AddRangeAsync(IEnumerable<Contact> contacts)
        {
            await _context.Contacts.AddRangeAsync(contacts);
        }

        public void Update(Contact contact)
        {
            _context.Entry(contact).State = EntityState.Modified;
        }

        public async Task DeleteAsync(int id)
        {
            var contact = await GetByIdAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
