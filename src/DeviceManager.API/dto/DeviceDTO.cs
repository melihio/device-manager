using System.Text.Json.Serialization;
using device_manager.models;

namespace HTTPApi.dto;

public class DeviceDTO
{
    public required string Type { get; set; }
    public required string Name { get; set; }
    public bool TurnedOn { get; set; }
    public int? Battery { get; set; }
    public string? OperatingSystem { get; set; } 
    public string? NetworkName { get; set; } 
    public string? IpAddress { get; set; }
}