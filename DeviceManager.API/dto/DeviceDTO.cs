using System.Text.Json.Serialization;
using device_manager.models;

namespace HTTPApi.dto;

public class DeviceDTO
{
    public string Type {get; set;}
    public Device Device {get; set;}
}