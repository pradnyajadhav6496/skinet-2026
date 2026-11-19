using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductRepository repository) : ControllerBase
    {       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? brand, string? type, string? sort)
        {
            return Ok(await repository.GetProductsAsync(brand, type, sort));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await repository.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            repository.AddProduct(product);
            if(!await repository.SaveChangesAsync())
            {
                return BadRequest("Could not create product.");
            }
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, [FromBody]Product product)
        {
            if (id != product.Id || !ProductExists(id))
            {
                return BadRequest("Can not update product. Product ID mismatch or product does not exist.");
            }

            repository.UpdateProduct(product);
            if(!await repository.SaveChangesAsync())
            {
                return BadRequest("Could not update product.");
            }
            return NoContent();                        
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            repository.DeleteProduct(id);
            if(!await repository.SaveChangesAsync())
            {
                return BadRequest("Could not delete product.");
            }
            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return repository.ProductExists(id);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            return Ok(await repository.GetBrandsAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            return Ok(await repository.GetTypesAsync());
        }
    }
}
