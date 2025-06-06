using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Services;

public class TripService : ITripService
{
    private readonly AppDbContext _context;

    public TripService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TripListResponseDTO> GetTripsAsync(int page, int pageSize)
    {
        var totalCount = await _context.Trips.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var trips = await _context.Trips
            .Include(t => t.Countries)
            .Include(t => t.ClientTrips)
                .ThenInclude(ct => ct.Client)
            .OrderByDescending(t => t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new TripListResponseDTO
        {
            PageNumber = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            Trips = trips.Select(t => new TripDTO
            {
                Name = t.Name,
                Description = t.Description,
                DateFrom = t.DateFrom,
                DateTo = t.DateTo,
                MaxPeople = t.MaxPeople,
                Countries = t.Countries.Select(c => new CountryDTO
                {
                    Name = c.Name
                }).ToList(),
                Clients = t.ClientTrips.Select(ct => new ClientShortDTO
                {
                    FirstName = ct.Client.FirstName,
                    LastName = ct.Client.LastName
                }).ToList()
            }).ToList()
        };

        return result;
    }

    public async Task<string> AssignClientToTripAsync(int idTrip, ClientTripAssignDTO dto)
    {
        var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Pesel == dto.Pesel);

        if (existingClient != null)
        {
            bool alreadyAssigned = await _context.ClientTrips
                .AnyAsync(ct => ct.IdTrip == idTrip && ct.IdClient == existingClient.IdClient);

            if (alreadyAssigned)
                throw new Exception("Client already assigned to this trip.");
        }

        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip == idTrip);

        if (trip == null)
            throw new Exception("Trip not found.");

        if (trip.DateFrom <= DateTime.Now)
            throw new Exception("Trip has already started.");

        var client = existingClient ?? new Client
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Telephone = dto.Telephone,
            Pesel = dto.Pesel
        };

        if (existingClient == null)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
        }

        var assignment = new ClientTrip
        {
            IdClient = client.IdClient,
            IdTrip = trip.IdTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = dto.PaymentDate
        };

        _context.ClientTrips.Add(assignment);
        await _context.SaveChangesAsync();

        return "Client assigned to trip successfully.";
    }
}
