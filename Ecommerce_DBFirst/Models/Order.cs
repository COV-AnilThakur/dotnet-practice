using System;
using System.Collections.Generic;

namespace Ecommerce_DBFirst.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public virtual Customer? Customer { get; set; }
}
