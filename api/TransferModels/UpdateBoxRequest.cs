using System.ComponentModel.DataAnnotations;

namespace api.TransferModels;

public class UpdateBoxRequest
{
    [Required]
    [MinLength(2)]
    public string BoxName { get; set; }

    public string Description { get; set; }
    
    
    [Required]
    [RegularExpression(@"^(?i)(small|medium|large|extra-large|[0-9])$")]
    public string? Size { get; set; }
    

    [Required(ErrorMessage = "Price is required")]
    [Range(0, 9999, ErrorMessage = "Price must be between 0 and 9999")]
    public int Price { get; set; }
    
    [Required]
    [Url]
    public string? ImageUrl { get; set; }
}

    
