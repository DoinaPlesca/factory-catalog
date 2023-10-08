using System.ComponentModel.DataAnnotations;

namespace api.TransferModels;

public class UpdateBoxRequest
{
    
    [MinLength(2)] 
    public string BoxName { get; set; }
    
    public string Description { get; set; }
    
    [Required]
    [RegularExpression("^(Small|Medium|Large|Extra-large)$")]
    public string Size { get; set; }
    
    [Required]
    [Range(0, 9999, ErrorMessage = "Price must be this format: 0,00...")]
    public int Price { get; set; }
    
    public string ImageUrl { get; set; }
    
}