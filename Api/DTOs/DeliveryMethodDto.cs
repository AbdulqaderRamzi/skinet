using System.ComponentModel.DataAnnotations;

namespace Api.DTOs;

public class DeliveryMethodDto
{
    [Required] 
    public string ShortName { get; set; } = null!;
    [Required]
    public string DeliveryTime { get; set; } = null!;
    [Required]
    public string Description { get; set; } = null!;
    [Required]
    public decimal Price { get; set; }
}