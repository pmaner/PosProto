using System;
namespace POS.Interfaces
{
    /// <summary>
    /// Interface so I can use the SaleItem and my viewmodel interchangebly
    /// </summary>
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
