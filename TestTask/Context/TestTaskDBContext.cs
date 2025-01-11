using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TestTask.Models;

namespace TestTask.Context
{
    public class TestTaskDBContext: DbContext
    {
        public DbSet<Contact> Contacts { get; set; }

        public TestTaskDBContext(DbContextOptions<TestTaskDBContext> dbContextOptions) : base(dbContextOptions) 
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Contact>().HasData(
                new Contact
                {
                    Id = 1,
                    Name = "Ivan Ivanov",
                    DateOfBirth = new DateTime(1998, 12, 1),
                    Married = false,
                    Phone = "068-000-1110",
                    Salary = 50000
                },
                new Contact
                {
                    Id = 2,
                    Name = "Petro Petrov",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    Married = true,
                    Phone = "096-000-0010",
                    Salary = 30000
                },
                new Contact
                {
                    Id = 3,
                    Name = "Maria Marianova",
                    DateOfBirth = new DateTime(1985, 7, 25),
                    Married = true,
                    Phone = "098-000-1000",
                    Salary = 20000
                }
            );
        }
    }
}
