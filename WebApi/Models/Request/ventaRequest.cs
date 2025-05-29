using System.ComponentModel.DataAnnotations;
using WebApi.Context;

namespace WebApi.Models.Request
{
    public class VentaRequest
    {
        [Required ]
        [Range(1,Double.MaxValue,ErrorMessage ="El valor del idUser debe ser mayor a 0")]
        [IsUserExist(ErrorMessage ="El user no existe")]
        public int IdUser { get; set; }
        //public decimal Total { get; set; }//

        [Required]
        [MinLength(1,ErrorMessage ="Deben existir conceptos")] //minimo 1 elemento 
       public List<ConceptoRequest> Conceptos { get; set; }
        public VentaRequest() {
            //para que no nos agregen otra cosa inicializamos 
            this.Conceptos = new List<ConceptoRequest>();
        }
    }

    public class ConceptoRequest
    {

        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Importe { get; set; }

        public int IdProducto{ get; set;}
    }


    #region validaciones propias

   public class IsUserExist:ValidationAttribute {
        
        protected override ValidationResult IsValid(object value,ValidationContext validationContext)
        {
            int idUser = (int)value;
            var context = validationContext.GetService(typeof(AppDbContext)) as AppDbContext;
            //buscamos en la database al user 
            var UserExists = context.Users.Any(u => u.Id == idUser);
            if(!UserExists)
            {
                return new ValidationResult($"No se encontro usuario con el id {idUser}.");
            }
            return ValidationResult.Success;
        }
    }
    #endregion
}
