using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace api.TransferModels;

public class CreateNewBoxRequest
{
    [NotNull]
    [Required]
    [StringLength(30, MinimumLength = 1)]
    public string? BoxName { get; set; }

    [NotNull]
    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string? Description { get; set; }

    [NotNull] [Required] [Url] public string? ImageUrl { get; set; }

    [NotNull]
    [Required]
    [RegularExpression("^(small|medium|large|extra-large)$")]

    public string? Size { get; set; }

    [NotNull]
    [Required(ErrorMessage = "Price is required")]
    public int Price { get; set; }

}