using System.Text.Json;
using device_manager.managers;
using device_manager.models;
using HTTPApi.dto;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var deviceManager = DeviceManager.GetInstance("../../../../DeviceManager.Logic/input.txt");

app.MapGet("api/devices", () =>
{
    try
    {
        var devices = deviceManager.GetAllDevices();
        return Results.Ok(devices);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("api/devices/{deviceType}-{deviceId}", (string deviceType,string deviceId) =>
{
    try
    {
        var devices = deviceManager.GetDeviceById(deviceType,deviceId);
        return Results.Ok(devices);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("api/device", ([FromBody] DeviceDTO dto) =>
{
    try {
        Device concreteDevice = (dto.Type switch
        {
            "SW" => JsonSerializer.Deserialize<Smartwatch>(JsonSerializer.Serialize(dto.Device)),
            "P"  => JsonSerializer.Deserialize<PersonalComputer>(JsonSerializer.Serialize(dto.Device)),
            "ED" => JsonSerializer.Deserialize<EmbeddedDevice>(JsonSerializer.Serialize(dto.Device)),
            _ => throw new ArgumentException("Invalid device type")
        })!;

        deviceManager.AddDevice(dto.Type, concreteDevice);
        return Results.Ok("Device successfully added");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPut("api/device", ([FromBody] DeviceDTO dto) =>
{
    try {
        Device concreteDevice = (dto.Type switch
        {
            "SW" => JsonSerializer.Deserialize<Smartwatch>(JsonSerializer.Serialize(dto.Device)),
            "P"  => JsonSerializer.Deserialize<PersonalComputer>(JsonSerializer.Serialize(dto.Device)),
            "ED" => JsonSerializer.Deserialize<EmbeddedDevice>(JsonSerializer.Serialize(dto.Device)),
            _ => throw new ArgumentException("Invalid device type")
        })!;

        deviceManager.UpdateDevice(dto.Type, concreteDevice);
        return Results.Ok("Device successfully updated");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapDelete("api/device/{deviceType}-{deviceId}", (string deviceType, string deviceId) =>
{
    try
    {
        deviceManager.DeleteDevice(deviceType, deviceId);
        return Results.Ok("Device successfully deleted");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.UseHttpsRedirection();

app.Run();