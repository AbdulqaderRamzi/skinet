using Api.Helpers;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApiController : ControllerBase 
{
    protected async Task<IActionResult> CreatePagedResult<T>(IGenericRepository<T> repo,
        ISpecification<T> spec, int pageIndex, int pageSize) where T : BaseEntity
    {
        var items = await repo.GetAllAsync(spec);
        var count = await repo.CountAsync(spec);
        var pagination = new Pagination<T>(pageSize, pageIndex, count, items);

        return Ok(pagination);
    }
}