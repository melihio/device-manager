using device_manager.managers;

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

app.MapPut("api/device/{device}", (string device) =>
{
    try
    {
        var d = DeviceManager.GetDeviceByString(device);
        deviceManager.UpdateDevice(DeviceManager.GetDeviceType(d),d);
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