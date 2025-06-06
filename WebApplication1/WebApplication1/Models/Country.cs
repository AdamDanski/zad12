using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Country
{
    [Key]
    public int IdCountry { get; set; }

    public string Name { get; set; }

    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
}