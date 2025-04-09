using device_manager.managers;
using device_manager.models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var deviceManager = DeviceManager.GetInstance("../../../../device-manager/input.txt");

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

app.MapPost("api/device/{device}", (string device) =>
{
    try
    {
        var d = DeviceManager.GetDeviceByString(device);
        var deviceType = DeviceManager.GetDeviceType(d);
        deviceManager.AddDevice(deviceType,device);
        return Results.Ok("Device successfully added");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.UseHttpsRedirection();

app.Run();