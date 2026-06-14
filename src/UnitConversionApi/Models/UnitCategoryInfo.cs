namespace UnitConversionApi.Models;

/// <summary>
/// Represents information about a supported unit within a category.
/// </summary>
public class UnitCategoryInfo
{
    /// <summary>
    /// The measurement category name.
    /// </summary>
    /// <example>length</example>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// The list of unit names supported in this category.
    /// </summary>
    /// <example>["meter", "kilometer", "foot", "inch"]</example>
    public List<string> Units { get; set; } = [];
}
