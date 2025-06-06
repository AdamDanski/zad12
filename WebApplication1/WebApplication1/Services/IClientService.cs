namespace WebApplication1.Services;

public interface IClientService
{
    Task<string> RemoveClientAsync(int idClient);
}