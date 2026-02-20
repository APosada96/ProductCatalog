using ProductCatalog.Blazor.Models;

namespace ProductCatalog.Blazor.Services;

public sealed class ProductsApi(ApiClient api)
{
    public Task<QueryResult<PagedResult<ProductDto>>> GetProductsAsync(
        int pageNumber, int pageSize, string? sortField, string? sortDirection, CancellationToken ct = default)
        => api.GetAsync<QueryResult<PagedResult<ProductDto>>>(
            $"/api/products?pageNumber={pageNumber}&pageSize={pageSize}&sortField={sortField}&sortDirection={sortDirection}", ct);

    public Task<QueryResult<ProductDto?>> GetByIdAsync(Guid id, CancellationToken ct = default)
        => api.GetAsync<QueryResult<ProductDto?>>($"/api/products/{id}", ct);

    public Task<SkuExistsResponse> SkuExistsAsync(string sku, CancellationToken ct = default)
        => api.GetAsync<SkuExistsResponse>($"/api/products/sku-exists/{Uri.EscapeDataString(sku)}", ct);

    public Task<CreateProductResponse> CreateAsync(CreateProductRequest req, CancellationToken ct = default)
        => api.PostAsync<CreateProductRequest, CreateProductResponse>("/api/products", req, ct);

    public Task UpdateAsync(Guid id, UpdateProductRequest req, CancellationToken ct = default)
        => api.PutAsync($"/api/products/{id}", req, ct);

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => api.DeleteAsync($"/api/products/{id}", ct);
}
