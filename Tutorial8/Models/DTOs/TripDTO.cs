using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTOs;

public class TripDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<CountryDTO> Countries { get; set; }
    
    [StringLength(220)]
    public string Description { get; set; }
    
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
}

public class CountryDTO
{
    public string Name { get; set; }
}