using WebApplication1.Data;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Services;

public class ClientService : IClientService
{
    private readonly AppDbContext _context;

    public ClientService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> RemoveClientAsync(int idClient)
    {
        var client = await _context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null)
        {
            throw new Exception($"Client with ID {idClient} does not exist.");
        }

        if (client.ClientTrips.Any())
        {
            throw new Exception("Cannot delete client – they are assigned to a trip.");
        }

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();

        return $"Client with ID {idClient} deleted successfully.";
    }
}