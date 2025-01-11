using TestTask.Models;

namespace TestTask.Repositories
{
    public interface IContactRepository
    {
        Task AddAsync(Contact contact);
        Task AddRangeAsync(IEnumerable<Contact> contacts);
        Task DeleteAsync(int id);
        Task<IEnumerable<Contact>> GetAllAsync();
        Task<Contact> GetByIdAsync(int id);
        Task SaveChangesAsync();
        void Update(Contact contact);
    }
}
