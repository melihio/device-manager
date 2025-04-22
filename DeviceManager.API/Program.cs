using device_manager.managers;


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

// app.MapPost("api/device", ([FromBody] DeviceDTO dto) =>
// {
//     try {
//         Device concreteDevice = (dto.Type switch
//         {
//             "SW" => JsonSerializer.Deserialize<Smartwatch>(JsonSerializer.Serialize(dto.Device)),
//             "P"  => JsonSerializer.Deserialize<PersonalComputer>(JsonSerializer.Serialize(dto.Device)),
//             "ED" => JsonSerializer.Deserialize<EmbeddedDevice>(JsonSerializer.Serialize(dto.Device)),
//             _ => throw new ArgumentException("Invalid device type")
//         })!;
//
//         deviceManager.AddDevice(dto.Type, concreteDevice);
//         return Results.Created();
//     }
//     catch (Exception ex)
//     {
//         return Results.Problem(ex.Message);
//     }
// });
//
// app.MapPut("api/device", ([FromBody] DeviceDTO dto) =>
// {
//     try {
//         Device concreteDevice = (dto.Type switch
//         {
//             "SW" => JsonSerializer.Deserialize<Smartwatch>(JsonSerializer.Serialize(dto.Device)),
//             "P"  => JsonSerializer.Deserialize<PersonalComputer>(JsonSerializer.Serialize(dto.Device)),
//             "ED" => JsonSerializer.Deserialize<EmbeddedDevice>(JsonSerializer.Serialize(dto.Device)),
//             _ => throw new ArgumentException("Invalid device type")
//         })!;
//
//         deviceManager.UpdateDevice(dto.Type, concreteDevice);
//         return Results.Ok("Device successfully updated");
//     }
//     catch (Exception ex)
//     {
//         return Results.Problem(ex.Message);
//     }
// });
//
// app.MapDelete("api/device/{deviceType}-{deviceId}", (string deviceType, string deviceId) =>
// {
//     try
//     {
//         deviceManager.DeleteDevice(deviceType, deviceId);
//         return Results.Ok("Device successfully deleted");
//     }
//     catch (Exception ex)
//     {
//         return Results.Problem(ex.Message);
//     }
// });

app.UseHttpsRedirection();

app.Run();