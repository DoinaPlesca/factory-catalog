﻿using System.ComponentModel.DataAnnotations;
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

    [NotNull]
    [Required]
    [Url]
    public string? ImageUrl { get; set; }

    [NotNull]
    [Required]
    [RegularExpression(@"^(?i)(small|medium|large|extra-large|[0-9])$")]
    public string? Size { get; set; }

   
    [Required(ErrorMessage = "Price is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Price must be a positive number")]
    [DataType(DataType.Currency)]
    public int Price { get; set; }
}
