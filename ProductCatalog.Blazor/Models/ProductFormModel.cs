using System.ComponentModel.DataAnnotations;

namespace ProductCatalog.Blazor.Models;

public sealed class ProductFormModel
{
    [Required(ErrorMessage = "Nombre es requerido.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "SKU es requerido.")]
    public string Sku { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Precio de venta debe ser >= 0.")]
    public decimal SalePrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Costo debe ser >= 0.")]
    public decimal Cost { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock debe ser >= 0.")]
    public int Stock { get; set; }

    public Guid? RequestId { get; set; }
}
