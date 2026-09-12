namespace Nuvia.API.DTOs;

public class MedicineSearchRequest
{
    public string Query { get; set; } = string.Empty;
}

public class RelatedVariant
{
    public string Name { get; set; } = string.Empty;
    public string Form { get; set; } = string.Empty;
    public string Strength { get; set; } = string.Empty;
}

public class MedicineInfo
{
    public string BrandName { get; set; } = string.Empty;
    public string GenericName { get; set; } = string.Empty;
    public string DrugClass { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Strength { get; set; } = string.Empty;
    public string Form { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string WhatIsIt { get; set; } = string.Empty;
    public List<string> UsedFor { get; set; } = new();
    public List<string> SideEffects { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Directions { get; set; } = new();
    public List<RelatedVariant> RelatedVariants { get; set; } = new();
    public string Disclaimer { get; set; } = string.Empty;
    public bool Found { get; set; } = true;
    public string NotFoundMessage { get; set; } = string.Empty;
}