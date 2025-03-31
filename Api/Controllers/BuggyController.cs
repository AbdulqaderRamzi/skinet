using Api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class BuggyController : ApiController
{
    [HttpGet("internalerror")]
    public IActionResult GetInternalError()
    {
        throw new Exception("This is a test exception.");
    } 
    
    [HttpPost("validation")]
    public IActionResult GetValidationError(CreateProductDto product)
    {
        return Ok();
    }
}