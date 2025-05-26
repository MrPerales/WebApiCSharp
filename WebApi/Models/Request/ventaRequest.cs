namespace WebApi.Models.Request
{
    public class VentaRequest
    {
        public int IdUser { get; set; }
        public decimal Total { get; set; }
        
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
}
