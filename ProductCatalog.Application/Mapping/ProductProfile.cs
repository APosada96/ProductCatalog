using AutoMapper;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Mapping;

public sealed class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.Sku, m => m.MapFrom(s => s.Sku.Value))
            .ForMember(d => d.SalePrice, m => m.MapFrom(s => s.SalePrice.Amount))
            .ForMember(d => d.Cost, m => m.MapFrom(s => s.Cost.Amount))
            .ForMember(d => d.MarginPercent, m => m.MapFrom(s =>
                s.SalePrice.Amount <= 0 ? 0 :
                decimal.Round(((s.SalePrice.Amount - s.Cost.Amount) / s.SalePrice.Amount) * 100m, 2)));
    }
}
