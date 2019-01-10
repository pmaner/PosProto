using System;
namespace POS.Interfaces
{
    public interface ISaleItem
    {

        int ProductId { get; }

        int Quantity { get; set; }

        string Description { get; set; }

        decimal UnitPrice { get; set; }

        decimal Discount { get; set; }

        decimal TotalPrice { get; }

    }
}
