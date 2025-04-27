using System.Text.Json;
using device_manager.exceptions;
using device_manager.managers;
using device_manager.models;
using HTTPApi.dto;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DeviceManagerDB");

if (connectionString == null)
{
    Console.WriteLine("Connection string not defined");
    Environment.Exit(-1);
}

builder.Services.AddOpenApi();
builder.Services.AddSingleton<DeviceManager>(sp => new DeviceManager(connectionString!));

var app = builder.Build();


app.MapGet("api/devices", (DeviceManager deviceManager) =>
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

app.MapGet("api/devices/{deviceId}", (string deviceId,DeviceManager deviceManager) =>
{
    try
    {
        var devices = deviceManager.GetDeviceById(deviceId);
        return Results.Ok(devices);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.MapPost("/api/devices", async (HttpRequest request, DeviceManager deviceManager) =>
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
            string body = await new StreamReader(request.Body).ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(body))
                return Results.BadRequest("Request body is empty.");
            dto = JsonSerializer.Deserialize<DeviceDTO>(body);
        }

        if (dto == null || string.IsNullOrWhiteSpace(dto.Type))
            return Results.BadRequest("Device type is required.");

        Device device = dto.Type.ToUpper() switch
        {
            "SW" => dto.Smartwatch != null && int.TryParse(dto.Smartwatch.Battery.ToString(), out _)
                ? new Smartwatch(dto.Smartwatch.Battery, dto.Smartwatch.TurnedOn, dto.Smartwatch.Id, dto.Smartwatch.Name)
                : throw new ArgumentException("Missing or invalid parameters"),
            "PC" => dto.PersonalComputer != null
                ? new PersonalComputer(dto.PersonalComputer.OperatingSystem, dto.PersonalComputer.TurnedOn, dto.PersonalComputer.Id, dto.PersonalComputer.Name)
                : throw new ArgumentException("Missing or invalid parameters"),
            "ED" => dto.EmbeddedDevice != null
                ? new EmbeddedDevice(dto.EmbeddedDevice.Id, dto.EmbeddedDevice.Name, dto.EmbeddedDevice.NetworkName, dto.EmbeddedDevice.IpAddress)
                : throw new ArgumentException("Missing or invalid parameters"),
            _ => throw new ArgumentException("Invalid device type"),
        };

        if (!bool.TryParse(device.TurnedOn.ToString(), out _))
        {
            throw new ArgumentException("Missing or invalid parameters: TurnedOn");
        }

        device.Validate();
        deviceManager.AddDevice(device);
        return Results.Created();
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


app.MapPut("/api/devices/", ([FromBody] DeviceDTO dto,DeviceManager deviceManager) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(dto.Type))
            return Results.BadRequest("Device type is required.");

        Device device = dto.Type.ToUpper() switch
        {
            "SW" => dto.Smartwatch != null && int.TryParse(dto.Smartwatch.Battery.ToString(), out _)
                ? new Smartwatch(dto.Smartwatch.Battery, dto.Smartwatch.TurnedOn, dto.Smartwatch.Id, dto.Smartwatch.Name)
                : throw new ArgumentException("Missing or invalid parameters"),
            "PC" => dto.PersonalComputer != null
                ? new PersonalComputer(dto.PersonalComputer.OperatingSystem, dto.PersonalComputer.TurnedOn,dto.PersonalComputer.Id, dto.PersonalComputer.Name)
                : throw new ArgumentException("Missing or invalid parameters"),
            "ED" => dto.EmbeddedDevice != null
                ? new EmbeddedDevice(dto.EmbeddedDevice.Id, dto.EmbeddedDevice.Name, dto.EmbeddedDevice.NetworkName, dto.EmbeddedDevice.IpAddress)
                : throw new ArgumentException("Missing or invalid parameters"),
            _ => throw new ArgumentException("Invalid device type"),
        };
        
        if (!bool.TryParse(device.TurnedOn.ToString(), out _))
        {
            throw new ArgumentException("Missing or invalid parameters: TurnedOn");
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

app.MapDelete("api/device/{deviceId}", (string deviceId,DeviceManager deviceManager) =>
{
    try
    {
        deviceManager.DeleteDevice( deviceId);
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