using WebApplication1.DTOs;

namespace WebApplication1.Services;

public interface ITripService
{
    Task<TripListResponseDTO> GetTripsAsync(int page, int pageSize);
    Task<string> AssignClientToTripAsync(int idTrip, ClientTripAssignDTO dto);
}
