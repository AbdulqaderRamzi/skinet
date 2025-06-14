﻿using Api.Helpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class ProductsController(IUnitOfWork unit) : ApiController
{
    [Cache(600)]
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] ProductSpecParams specParams)
    {
        var spec = new ProductSpecification(specParams);
        return await CreatePagedResult(unit.Repository<Product>(), spec, specParams.PageIndex, specParams.PageSize);
    }
    
    [Cache(600)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var product = await unit.Repository<Product>().GetByIdAsync(id);
        if (product is null) return NotFound();
        return Ok(product);
    }
    
    [Cache(10000)]
    [HttpGet("brands")]
    public async Task<IActionResult> GetBrands()
    {
        var spec = new BrandListSpecification(); 
        return Ok(await unit.Repository<Product>().GetAllAsync(spec));
    }
    
    [Cache(10000)]
    [HttpGet("types")]
    public async Task<IActionResult> GetTypes()
    {
        var spec = new TypeListSpecification();
        return Ok(await unit.Repository<Product>().GetAllAsync(spec));
    }

    [InvalidateCache("api/products|")]
    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {
        await unit.Repository<Product>().AddAsync(product);
        return await unit.CompleteAsync()
            ? CreatedAtAction(nameof(Get), new { id = product.Id }, product)
            : BadRequest();
    }

    [InvalidateCache("api/products|")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, Product product)
    {
        if (id != product.Id) 
            return BadRequest();
        
        if (!await unit.Repository<Product>().IsExistsAsync(id))
            return NotFound();
        
        unit.Repository<Product>().Update(product);

        return await unit.CompleteAsync()
            ? NoContent()
            : BadRequest();
    }

    [InvalidateCache("api/products|")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await unit.Repository<Product>().GetByIdAsync(id);
        if (product is null) return NotFound();
        unit.Repository<Product>().Remove(product);
        return await unit.CompleteAsync()
            ? NoContent()
            : BadRequest();
    }
}