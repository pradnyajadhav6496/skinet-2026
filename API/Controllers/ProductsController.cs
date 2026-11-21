using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Specifications;
using API.RequestHelpers;

namespace API.Controllers
{

    public class ProductsController(IGenericRepository<Product> repository) : BaseApiController
    {       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts([FromQuery]ProductSpecParams specParams)
        {
            var spec = new ProductSpecification(specParams);
            return await CreatePagedResult(repository, spec, specParams.PageIndex, specParams.PageSize);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await repository.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            repository.Add(product);
            if(!await repository.SaveAllAsync())
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

            repository.Update(product);
            if(!await repository.SaveAllAsync())
            {
                return BadRequest("Could not update product.");
            }
            return NoContent();                        
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            repository.Remove(id);
            if(!await repository.SaveAllAsync())
            {
                return BadRequest("Could not delete product.");
            }
            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return repository.Exists(id);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            var spec = new BrandListSpecification();
            var brands = await repository.ListAsync(spec);
            return Ok(brands);
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            var spec = new TypeListSpecification();
            var types = await repository.ListAsync(spec);
            return Ok(types);
        }
    }
}
