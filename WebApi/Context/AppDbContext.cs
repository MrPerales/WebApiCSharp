using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.SeedData;

namespace WebApi.Context
{
    // DbContext base principal para que entityFrameworkCore funcione como ORM
    public class AppDbContext : DbContext
    {               //DbContextOptions toma un argumento para conf el comportamiento del DbContext
                    //y le mandamos las opciones al contructor del DbContext 
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }
        public DbSet<Person> Persons { get; set; }
        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd(); //el id se genera en la dataBase si es que la tenemos configurada asi
            PersonSeedData.Seed(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
