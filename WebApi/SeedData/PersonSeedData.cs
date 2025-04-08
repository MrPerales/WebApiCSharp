using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using WebApi.Models;

namespace WebApi.SeedData
{
    public class PersonSeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            //solo para que id empice autoincremetal bien ,
            //el HasData tiene que llenar todos los campos de la tabla ya que son los primeros datos que se le agrega
            //y el id no puede ser null,
            //la configuracion de ValueGeneratedOnAdd() en AppDBContext la va a respetar 
            var persons = new[]
            {
                new Person { 
                            Id = 1,
                            Age = 52,
                            Email = "Colt_Kris80@example.com",
                            Name = "Amos Gerhold" 
                            },
                new Person{ 
                            Id = 2,
                            Age = 44,
                            Email = "Rory.Konopelski@example.org",
                            Name = "Adell Ebert"
                            },
                new Person{ 
                            Id = 3,
                            Age = 48,
                            Email = "Maximilian.Hoppe10@example.com",
                            Name = "Larue Graham"
                            },
                new Person{
                            Id = 4,
                            Age = 36,
                            Email = "Edna30@example.com",
                            Name = "Milford Kerluke"
                            },
                new Person{ 
                            Id = 5,
                            Age = 65,
                            Email = "Florine_Morissette16@example.org",
                            Name = "Jan Parisian"
                            },
                new Person {
                            Id = 6,
                            Age = 70,
                            Email = "Manuel_Romaguera@example.org",
                            Name = "Alejandrin Gaylord"
                            },
                new Person{ 
                            Id = 7,
                            Age = 34,
                            Email = "Consuelo6@example.com",
                            Name = "Dillan Schinner"
                            },

            };
            
           
            modelBuilder.Entity<Person>().HasData(persons);
        }
    }
}
