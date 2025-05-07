using System.Text.Json;
using device_manager.exceptions;
using device_manager.managers;
using device_manager.models;
using HTTPApi.dto;
using HTTPApi.utils;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DeviceManagerDB");

if (connectionString == null)
{
    Console.WriteLine("Connection string not defined");
    Environment.Exit(-1);
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<DeviceManager>(sp => new DeviceManager(connectionString));

var app = builder.Build();

app.MapGet("api/devices", async (DeviceManager deviceManager) =>
{
    try
    {
        var devices = await deviceManager.GetAllDevices();
        return Results.Ok(devices);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapGet("api/devices/{deviceId}", async (string deviceId, DeviceManager deviceManager) =>
{
    try
    {
        var device = await deviceManager.GetDeviceById(deviceId);
        return Results.Ok(device);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/api/devices", async (HttpRequest request, DeviceManager deviceManager, ILogger<Program> logger) =>
{
    try
    {
        DeviceDTO? dto = null;

        if (request.ContentType == "application/json")
        {
            dto = await JsonSerializer.DeserializeAsync<DeviceDTO>(request.Body);
        }
        else if (request.ContentType == "text/plain")
        {
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(body))
                return Results.BadRequest("Request body is empty.");
            dto = JsonSerializer.Deserialize<DeviceDTO>(body);
        }

        if (dto == null || string.IsNullOrWhiteSpace(dto.Type))
            return Results.BadRequest("Device type is required.");

        Device device = dto.Type.ToLower() switch
        {
            "sw" => new Smartwatch(
                battery: dto.Battery ?? throw new ArgumentException("Battery is required for Smartwatch"),
                turnedOn: dto.TurnedOn,
                id: Utils.GenerateDeviceId("SW", await deviceManager.GetAllDevices()),
                name: dto.Name
            ),
            "p" => new PersonalComputer(
                operatingSystem: dto.OperatingSystem ?? throw new ArgumentException("OperatingSystem is required for PersonalComputer"),
                turnedOn: dto.TurnedOn,
                id: Utils.GenerateDeviceId("P", await deviceManager.GetAllDevices()),
                name: dto.Name
            ),
            "ed" => new EmbeddedDevice(
                networkName: dto.NetworkName ?? throw new ArgumentException("NetworkName is required for EmbeddedDevice"),
                ipAddress: dto.IpAddress ?? throw new ArgumentException("IpAddress is required for EmbeddedDevice"),
                id: Utils.GenerateDeviceId("ED", await deviceManager.GetAllDevices()),
                name: dto.Name
            ),
            _ => throw new ArgumentException($"Invalid device type: {dto.Type}")
        };

        device.Validate();
        await deviceManager.AddDevice(device);
        return Results.Created($"/api/devices/{device.Id}", device);
    }
    catch (Exception ex)
    {
        return ex switch
        {
            KeyNotFoundException => Results.NotFound(ex.Message),
            ArgumentException or EmptyBatteryException or ArgumentOutOfRangeException or EmptySystemException or ConnectionException => Results.BadRequest(ex.Message),
            InvalidOperationException => Results.Conflict(ex.Message),
            _ => Results.Problem($"An unexpected error occurred: {ex.Message}")
        };
    }
});

app.MapPut("/api/devices", async ([FromBody] DeviceDTO dto, DeviceManager deviceManager) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(dto.Type))
            return Results.BadRequest("Device type is required.");

        var device = await deviceManager.GetDeviceById(dto.Id);
        
        device.Name = dto.Name;
        device.TurnedOn = dto.TurnedOn; 
        switch (device)
        {
            case Smartwatch sw:
                sw.Battery = dto.Battery!.Value;
                break;
            case PersonalComputer pc:
                pc.OperatingSystem = dto.OperatingSystem!;
                break;
            case EmbeddedDevice ed:
                ed.IpAddress   = dto.IpAddress!;
                ed.NetworkName = dto.NetworkName!;
                break;
        }

        device.Validate();
        deviceManager.UpdateDevice(device);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return ex switch
        {
            KeyNotFoundException => Results.NotFound(ex.Message),
            ArgumentException or EmptyBatteryException or ArgumentOutOfRangeException or EmptySystemException or ConnectionException => Results.BadRequest(ex.Message),
            InvalidOperationException => Results.Conflict(ex.Message),
            _ => Results.Problem("An unexpected error occurred.")
        };
    }
});

app.MapDelete("api/device/{deviceId}", async (string deviceId, DeviceManager deviceManager) =>
{
    try
    {
        await deviceManager.DeleteDevice(deviceId);
        return Results.Ok();
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.UseHttpsRedirection();

app.Run();