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
            .OrderByDescending(t=>t.DateFrom)
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
                })
                    .ToList(),
                Country = t.IdCountries.Select(c => new CountryDTO()
                {
                    IdCountry = c.IdCountry,
                    Name = c.Name
                }).ToList()
            })
            
            .ToListAsync();
        return Ok(trips);
    }

    [HttpDelete("{idClient}")]
    public async Task<IActionResult> DeleteClientAsync(int idClient)
    {
        var client = await _context.Clients.FindAsync(idClient);
        if (client == null)
        {
            return NotFound("Client not found");
        }

        var hasTrips = await _context.ClientTrips.AnyAsync(ct => ct.IdClient == idClient);
        if (hasTrips)
        {
            return BadRequest("Client has trips assigned");
        }
        
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("{idTrip}/clients")]
    public async Task<IActionResult> AddClientToTrip(int idTrip, [FromBody] ClientTripAssignmentDTO request)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == request.Pesel);
        if (client == null)
        {
            client = new Client
            {
                IdClient = request.IdClient,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Telephone = request.Telephone,
                Pesel = request.Pesel,
            };
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }
        
        var existingAssignment = await _context.ClientTrips
            .FirstOrDefaultAsync(ct => ct.IdClient == client.IdClient && ct.IdTrip == idTrip);
        if (existingAssignment != null)
        {
            return BadRequest("Client already assigned to this trip");
        }
        
        var trip = await _context.Trips.FindAsync(idTrip);
        if (trip == null)
        {
            return NotFound("Trip not found");
        }

        var clientTrip = new ClientTrip()
        {
            IdClient = client.IdClient,
            IdTrip = trip.IdTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = request.PaymentDate,
        };
        _context.ClientTrips.Add(clientTrip);
        await _context.SaveChangesAsync();

        return Ok("Client assigned to the trip");
    }
}