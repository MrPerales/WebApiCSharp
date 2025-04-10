using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class Venta
{
    public long Id { get; set; }

    public DateTime Fecha { get; set; }

    public decimal? Total { get; set; }

    public int IdUser { get; set; }

    public virtual ICollection<Concepto> Conceptos { get; set; } = new List<Concepto>();

    public virtual User IdUserNavigation { get; set; } = null!;
}
