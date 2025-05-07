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
    public async void UpdateDevice(Device device)
    {
        List<Device> _devices = await GetAllDevices();
        if (_devices.All(d => d.Id != device.Id))
            throw new KeyNotFoundException($"No device found with given Id");

        await using var conn = new SqlConnection(_connectionString);
        try
        {
            conn.Open();

            const string deviceSql = "UPDATE Device SET Name = @Name, IsEnabled = @IsEnabled WHERE Id = @Id";
            var deviceCommand = new SqlCommand(deviceSql, conn);
            deviceCommand.Parameters.AddWithValue("@Id", device.Id);
            deviceCommand.Parameters.AddWithValue("@Name", device.Name);
            deviceCommand.Parameters.AddWithValue("@IsEnabled", device.TurnedOn);
            deviceCommand.ExecuteNonQuery();

            switch (device)
            {
                case Smartwatch sw:
                    const string smartwatchSql = "UPDATE Smartwatch SET BatteryPercentage = @BatteryPercentage WHERE DeviceId = @DeviceId";
                    var smartwatchCommand = new SqlCommand(smartwatchSql, conn);
                    smartwatchCommand.Parameters.AddWithValue("@BatteryPercentage", sw.Battery);
                    smartwatchCommand.Parameters.AddWithValue("@DeviceId", device.Id);
                    if (smartwatchCommand.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException($"No Smartwatch found with DeviceId '{device.Id}' in the database.");
                    break;

                case PersonalComputer pc:
                    const string pcSql = "UPDATE PersonalComputer SET OperationSystem = @OperationSystem WHERE DeviceId = @DeviceId";
                    var pcCommand = new SqlCommand(pcSql, conn);
                    pcCommand.Parameters.AddWithValue("@OperationSystem", pc.OperatingSystem ?? (object)DBNull.Value);
                    pcCommand.Parameters.AddWithValue("@DeviceId", device.Id);
                    if (pcCommand.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException($"No PersonalComputer found with DeviceId '{device.Id}' in the database.");
                    break;

                case EmbeddedDevice ed:
                    const string embeddedSql = "UPDATE Embedded SET IpAddress = @IpAddress, NetworkName = @NetworkName WHERE DeviceId = @DeviceId";
                    var embeddedCommand = new SqlCommand(embeddedSql, conn);
                    embeddedCommand.Parameters.AddWithValue("@IpAddress", ed.IpAddress);
                    embeddedCommand.Parameters.AddWithValue("@NetworkName", ed.NetworkName);
                    embeddedCommand.Parameters.AddWithValue("@DeviceId", device.Id);
                    if (embeddedCommand.ExecuteNonQuery() == 0)
                        throw new InvalidOperationException($"No Embedded device found with DeviceId '{device.Id}' in the database.");
                    break;

                default:
                    throw new ArgumentException("Unknown device type");
            }
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException($"Failed to update device: {ex.Message}");
        }
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
