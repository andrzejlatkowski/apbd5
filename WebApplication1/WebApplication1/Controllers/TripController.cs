using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[ApiController]
[Route("/api/[controller]")]

public class TripController: ControllerBase
{
    private readonly MasterContext _context;

    public TripController(MasterContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTripAsync()
    {
        var trips = await _context.Trips
            .Include(t => t.ClientTrips)
            .ThenInclude(ct => ct.IdClientNavigation)
            .Include(t => t.IdCountries)
            .ThenInclude(ct => ct.IdTrips)
            .Select(t => new TripDTO()
            {
                IdTrip = t.IdTrip,
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                ClientTrip = t.ClientTrips.Select(ct => new ClientTripDTO()
                {
                    IdClient = ct.IdClient,
                    IdTrip = ct.IdTrip,
                    RegisteredAt = ct.RegisteredAt,
                    PaymentDate = ct.PaymentDate
                }).ToList(),
                Country = t.IdCountries.Select(c => new CountryDTO()
                {
                    IdCountry = c.IdCountry,
                    Name = c.Name
                }).ToList()
            }).ToListAsync();
        return Ok(trips);
    }
}