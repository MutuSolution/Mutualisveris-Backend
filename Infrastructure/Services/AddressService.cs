using Application.Services;
using AutoMapper;
using Common.Requests.Addresses;
using Common.Responses.Addresses;
using Common.Responses.Wrappers;
using Domain;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services;

public class AddressService : IAddressService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddressService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IResponseWrapper<AddressResponse>> AddAddressAsync(CreateAddressRequest request)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists)
            return ResponseWrapper<AddressResponse>.Fail("Kullanıcı bulunamadı.");

        var newAddress = _mapper.Map<Address>(request);
        _context.Addresses.Add(newAddress);
        await _context.SaveChangesAsync();

        return ResponseWrapper<AddressResponse>.Success(_mapper.Map<AddressResponse>(newAddress), "Adres başarıyla eklendi.");
    }

    public async Task<IResponseWrapper<AddressResponse>> UpdateAddressAsync(UpdateAddressRequest request)
    {
        var address = await _context.Addresses.FindAsync(request.Id);
        if (address == null)
            return ResponseWrapper<AddressResponse>.Fail("Adres bulunamadı.");

        _mapper.Map(request, address);
        await _context.SaveChangesAsync();

        return ResponseWrapper<AddressResponse>.Success(_mapper.Map<AddressResponse>(address), "Adres başarıyla güncellendi.");
    }

    public async Task<IResponseWrapper<bool>> DeleteAddressAsync(int addressId)
    {
        var address = await _context.Addresses.FindAsync(addressId);
        if (address == null)
            return ResponseWrapper<bool>.Fail("Adres bulunamadı.");

        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();

        return ResponseWrapper<bool>.Success(true, "Adres başarıyla silindi.");
    }

    public async Task<IResponseWrapper<AddressResponse>> GetAddressByIdAsync(int addressId)
    {
        var address = await _context.Addresses.FindAsync(addressId);
        if (address == null)
            return ResponseWrapper<AddressResponse>.Fail("Adres bulunamadı.");

        return ResponseWrapper<AddressResponse>.Success(_mapper.Map<AddressResponse>(address), "Adres başarıyla getirildi.");
    }

    public async Task<IResponseWrapper<List<AddressResponse>>> GetUserAddressesAsync(string userId)
    {
        var addresses = await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();
        if (!addresses.Any())
            return ResponseWrapper<List<AddressResponse>>.Fail("Kullanıcıya ait adres bulunamadı.");

        return ResponseWrapper<List<AddressResponse>>.Success(_mapper.Map<List<AddressResponse>>(addresses), "Adresler başarıyla getirildi.");
    }
}
