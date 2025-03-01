using System.ComponentModel.DataAnnotations;

namespace ImageHandling;

public class LatLngFileNameModel: LatLngModel
{
    [Key] // Marks 'Id' as the primary key
    public int Id { get; set; }

    public string? FileName { get; set; }    
}