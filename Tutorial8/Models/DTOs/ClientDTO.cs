using System.ComponentModel.DataAnnotations;

namespace Tutorial8.Models.DTOs;

public class ClientDTO
{
    [Required]
    public int IdClient { get; set; }
    
    [StringLength(120)]
    public string FirstName { get; set; }
    
    [StringLength(120)]
    public string LastName { get; set; }
    
    [StringLength(120)]
    public string Email { get; set; }
    
    [StringLength(120)]
    public string Telephone { get; set; }
    
    [StringLength(120)]
    public string Pesel { get; set; }

}