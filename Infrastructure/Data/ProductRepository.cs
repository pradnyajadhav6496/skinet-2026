using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ProductRepository(StoreContext context) : IProductRepository
{
    private readonly StoreContext _context = context;
    public void AddProduct(Product product)
    {
        _context.Products.Add(product);        
    }

    public bool DeleteProduct(int id)
    {
        var product = _context.Products.Find(id);
        if(product == null)
        {
            return false;
        }
        _context.Products.Remove(product);
        return true;
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);        
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(brand))
        {
            query = query.Where(p => p.Brand == brand);
        }

        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(p => p.Type == type);
        }       
        switch (sort)
        {
            case "priceAsc":
                query = query.OrderBy(p => p.Price);
                break;
            case "priceDesc":
                query = query.OrderByDescending(p => p.Price);
                break;
            default:
                query = query.OrderBy(p => p.Name);
                break;
        }        
        return await query.ToListAsync();    
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void UpdateProduct(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
    }
    public bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.Id == id);
    }

    public async Task<IReadOnlyList<string>> GetBrandsAsync()
    {
        return await _context.Products.Select(x => x.Brand).Distinct().ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetTypesAsync()
    {
        return await _context.Products.Select(x => x.Type).Distinct().ToListAsync();
    }
}
