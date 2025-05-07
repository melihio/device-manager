using device_manager.models;
using device_manager.Repository;
using Microsoft.Data.SqlClient;

namespace device_manager.managers;

public class DeviceManager
{
    private readonly string _connectionString;
    
    public DeviceManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    ///  Returns all the devices in database 
    /// </summary>
    public async Task<List<Device>> GetAllDevices()
    {
        var rp = new DeviceRepository(_connectionString);
        return await rp.GetAllAsync();
    }

    /// <summary>
    ///  This method adds given device to database.
    /// </summary>
    public async Task AddDevice(Device device)
    {
        var devices = await GetAllDevices();
        if (devices.Any(d => d.Id == device.Id))
            throw new InvalidOperationException($"Device with given Id already exists");
        try
        { 
            var rp = new DeviceRepository(_connectionString);
            rp.AddAsync(device).Wait();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Failed to add device: {ex.Message}");
        }
    }

    /// <summary>
    ///  This method updates an already existing device in the database
    /// </summary>
    public async Task UpdateDevice(Device device)
    {
        var existingDevice = await GetDeviceById(device.Id);
        device.RowVersion = existingDevice.RowVersion;

        var rp = new DeviceRepository(_connectionString);
        await rp.UpdateAsync(device);
    }


    /// <summary>
    ///  This method deletes an existing device from database.
    /// </summary>

    public async Task DeleteDevice(string deviceId)
    {
        var rp = new DeviceRepository(_connectionString);
        
        if(await GetDeviceById(deviceId) == null)
            throw new KeyNotFoundException();

        var type = DeviceRepository.GetTypeById(deviceId);
        var result = await rp.DeleteAsync(deviceId, type);
        
    }

    /// <summary>
    ///  This method returns a single device with given id.
    /// </summary>

    public async Task<Device> GetDeviceById(string deviceId)
    {
        var rp = new DeviceRepository(_connectionString);
        var device = await rp.GetByIdAsync(deviceId);
        if (device == null)
        {
            throw new KeyNotFoundException("No device found with given Id");
        }

        return device;
    }
}
