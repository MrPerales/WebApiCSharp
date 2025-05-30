﻿using System;
using System.Collections.Generic;

namespace WebApi.Models;

public partial class Concepto
{
    public long Id { get; set; }

    public long IdVenta { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Importe { get; set; }

    public int IdProducto { get; set; }

    public virtual Producto IdProductoNavigation { get; set; } = null!;

    public virtual Venta IdVentaNavigation { get; set; } = null!;
}
