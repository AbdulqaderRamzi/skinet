using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class CartController(ICartService cartService) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(string id)
    {
        var cart = await cartService.GetAsync(id);
        return Ok(cart ?? new ShoppingCart{Id = id});
    }

    [HttpPost]
    public async Task<IActionResult> UpdateAsync(ShoppingCart cart)
    {
        var updatedCart = await cartService.SetAsync(cart);
        return updatedCart is null ? BadRequest("Problem updating cart")
            : Ok(updatedCart);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var isDeleted = await cartService.DeleteAsync(id);
        return isDeleted ? Ok() : BadRequest("Problem deleting cart");
    }
}