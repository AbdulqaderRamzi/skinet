using Api.Helpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class ProductsController(IGenericRepository<Product> repo) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] ProductSpecParams specParams)
    {
        var spec = new ProductSpecification(specParams);
        return await CreatePagedResult(repo, spec, specParams.PageIndex, specParams.PageSize);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var product = await repo.GetByIdAsync(id);
        if (product is null) return NotFound();
        return Ok(product);
    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetBrands()
    {
        var spec = new BrandListSpecification();
        return Ok(await repo.GetAllAsync(spec));
    }
    
    [HttpGet("types")]
    public async Task<IActionResult> GetTypes()
    {
        var spec = new TypeListSpecification();
        return Ok(await repo.GetAllAsync(spec));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        await repo.AddAsync(product);
        return await repo.SaveAsync()
            ? CreatedAtAction(nameof(Get), new { id = product.Id }, product)
            : BadRequest();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, Product product)
    {
        if (id != product.Id) 
            return BadRequest();
        
        if (!await repo.IsExistsAsync(id))
            return NotFound();
        
        repo.Update(product);

        return await repo.SaveAsync()
            ? NoContent()
            : BadRequest();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await repo.GetByIdAsync(id);
        if (product is null) return NotFound();
        repo.Remove(product);
        return await repo.SaveAsync() 
            ? NoContent()
            : BadRequest();
    }
}