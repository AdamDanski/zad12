namespace WebApplication1.DTOs;

public class TripListResponseDTO
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public List<TripDTO> Trips { get; set; }
}