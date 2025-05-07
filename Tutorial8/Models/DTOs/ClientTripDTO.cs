using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTOs;

public class ClientTripDTO
{
    [Required]
    public int IdTrip { get; set; }
    
    [StringLength(120)]
    public string Name { get; set; }
    
    [StringLength(220)]
    public string Description { get; set; }
    
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public int? RegisteredAt { get; set; }
    public int? PaymentDate { get; set; }
}