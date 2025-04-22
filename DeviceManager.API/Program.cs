using device_manager.managers;
using device_manager.models;
using HTTPApi.dto;
using Microsoft.AspNetCore.Mvc;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DeviceManagerDB");

builder.Services.AddOpenApi();
builder.Services.AddSingleton<DatabaseManager>(sp => new DatabaseManager(connectionString!));
builder.Services.AddSingleton<DeviceManager>(sp => new DeviceManager(connectionString!));

var app = builder.Build();

// try
// {
//     var dbManager = app.Services.GetRequiredService<DatabaseManager>();
//     dbManager.Initialize("../DeviceManager.Logic/input.txt");
// }
// catch (Exception ex)
// {
//     Console.WriteLine("An error occured during initialization" + ex.Message);
// }

var deviceManager = app.Services.GetRequiredService<DeviceManager>();

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

app.MapGet("api/devices/{deviceId}", (string deviceId) =>
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

app.MapPost("/api/devices", ([FromBody] DeviceDTO dto) =>
{
    try
    {
        if (string.IsNullOrWhiteSpace(dto.Type))
            return Results.BadRequest("Device type is required.");

        Device? device = dto.Type.ToUpper() switch
        {
            "SW" => dto.Smartwatch != null
                ? new Smartwatch(dto.Smartwatch.Battery, dto.Smartwatch.TurnedOn, dto.Smartwatch.Id, dto.Smartwatch.Name)
                : null,
            "PC" => dto.PersonalComputer != null
                ? new PersonalComputer(dto.PersonalComputer.OperatingSystem, dto.PersonalComputer.TurnedOn,dto.PersonalComputer.Id, dto.PersonalComputer.Name)
                : null,
            "ED" => dto.EmbeddedDevice != null
                ? new EmbeddedDevice(dto.EmbeddedDevice.Id, dto.EmbeddedDevice.Name, dto.EmbeddedDevice.NetworkName, dto.EmbeddedDevice.IpAddress)
                : null,
            _ => null
        };

        if (device == null)
            return Results.BadRequest("Invalid device type or missing device data.");

        deviceManager.AddDevice(device);
        return Results.Created();
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(ex.Message); 
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message); 
    }
    catch (Exception)
    {
        return Results.Problem("An unexpected error occurred.");
    }
});

app.MapPut("/api/devices/", ([FromBody] DeviceDTO dto) =>
{
    try
    {
        Device? device = dto.Type.ToUpper() switch
        {
            "SW" => dto.Smartwatch != null
                ? new Smartwatch(dto.Smartwatch.Battery, dto.Smartwatch.TurnedOn, dto.Smartwatch.Id, dto.Smartwatch.Name)
                : null,
            "PC" => dto.PersonalComputer != null
                ? new PersonalComputer(dto.PersonalComputer.OperatingSystem, dto.PersonalComputer.TurnedOn,dto.PersonalComputer.Id, dto.PersonalComputer.Name)
                : null,
            "ED" => dto.EmbeddedDevice != null
                ? new EmbeddedDevice(dto.EmbeddedDevice.Id, dto.EmbeddedDevice.Name, dto.EmbeddedDevice.NetworkName, dto.EmbeddedDevice.IpAddress)
                : null,
            _ => null
        };

        if (device == null)
            throw new ArgumentException("Invalid device type or missing device data.");

        deviceManager.UpdateDevice(device);
        return Results.Ok();
    }
    catch (InvalidOperationException ex)
    {
        return Results.Conflict(ex.Message);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.Problem("An unexpected error occurred.");
    }
});

app.MapDelete("api/device/{deviceId}", (string deviceId) =>
{
    try
    {
        deviceManager.DeleteDevice( deviceId);
        return Results.Ok("Device successfully deleted");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.UseHttpsRedirection();

app.Run();