using Api.DTOs;
using Core.Entities;

namespace Api.Extensions;

public static class AddressMappingExtension
{
    public static AddressDto? ToDto(this Address? address)
    {
        if (address is null) return null;
        
        return new AddressDto
        {
            Line1 = address.Line1,
            Line2 = address.Line2,
            City = address.City,
            Country = address.Country,
            PostalCode = address.PostalCode,
            State = address.State,
        };
    }
    
    public static Address ToEntity(this AddressDto addressDto)
    {
        ArgumentNullException.ThrowIfNull(addressDto);
        return new Address
        {
            Line1 = addressDto.Line1,
            Line2 = addressDto.Line2,
            City = addressDto.City,
            Country = addressDto.Country,
            PostalCode = addressDto.PostalCode,
            State = addressDto.State,
        };
    }
    
    public static void UpdateFromDto(this Address address, AddressDto addressDto)
    {
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(addressDto);
        
        address.Line1 = addressDto.Line1;
        address.Line2 = addressDto.Line2;
        address.City = addressDto.City;
        address.Country = addressDto.Country;
        address.PostalCode = addressDto.PostalCode;
        address.State = addressDto.State;
    }
}