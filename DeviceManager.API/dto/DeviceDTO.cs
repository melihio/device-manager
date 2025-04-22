using System.Text.Json.Serialization;
using device_manager.models;

namespace HTTPApi.dto;

public class DeviceDTO
{
    public required string Type { get; set; }
    public SmartwatchDTO? Smartwatch { get; set; }
    public PersonalComputerDTO? PersonalComputer { get; set; }
    public EmbeddedDeviceDTO? EmbeddedDevice { get; set; }
}

public class SmartwatchDTO
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required int Battery { get; set; }
    public bool TurnedOn { get; set; }
}

public class PersonalComputerDTO
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string OperatingSystem { get; set; }
    public bool TurnedOn { get; set; }
}

public class EmbeddedDeviceDTO
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string NetworkName { get; set; }
    public required string IpAddress { get; set; }
}