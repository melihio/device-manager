namespace device_manager.Repository;

using System.Data;
using device_manager.models;
using Microsoft.Data.SqlClient;

public class DeviceRepository : IDeviceRepository
{
    private readonly string _connectionString;

    public DeviceRepository(string connectionString)
    {
        _connectionString = connectionString; 
    }

    public static string GetTypeById(string id)
    {
        return id.Split('-')[0];
    }

    public async Task AddAsync(Device device)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var command = new SqlCommand("AddDevice", conn);
        await conn.OpenAsync();
        
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@Id", device.Id);
        command.Parameters.AddWithValue("@Name", device.Name);
        command.Parameters.AddWithValue("@IsEnabled", device.TurnedOn);
        command.Parameters.AddWithValue("@Type", GetTypeById(device.Id));

        command.Parameters.AddWithValue("@BatteryPercentage", DBNull.Value);
        command.Parameters.AddWithValue("@OperationSystem", DBNull.Value);
        command.Parameters.AddWithValue("@IpAddress", DBNull.Value);
        command.Parameters.AddWithValue("@NetworkName", DBNull.Value);

        switch (device)
        {
            case Smartwatch sw:
                command.Parameters["@BatteryPercentage"].Value = sw.Battery;
                break;
            case PersonalComputer pc:
                command.Parameters["@OperationSystem"].Value = pc.OperatingSystem;
                break;
            case EmbeddedDevice ed:
                command.Parameters["@IpAddress"].Value = ed.IpAddress;
                command.Parameters["@NetworkName"].Value = ed.NetworkName;
                break;
            default:
                throw new NotSupportedException($"Unknown device type");
        }
        await command.ExecuteNonQueryAsync();
    }

    public async Task<Device?> GetByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException("Device ID cannot be null or empty.", nameof(id));
        }

        await using var conn = new SqlConnection(_connectionString);
        await using var command = new SqlCommand("GetDeviceById", conn);
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@Id", id);

        await conn.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var deviceType = reader.GetString(3);
            Device device = deviceType switch
            {
                "ED" => new EmbeddedDevice(
                    reader.IsDBNull(5) ? throw new InvalidOperationException($"IpAddress is null for EmbeddedDevice with ID: {id}") : reader.GetString(5),
                    reader.IsDBNull(6) ? throw new InvalidOperationException($"NetworkName is null for EmbeddedDevice with ID: {id}") : reader.GetString(6),
                    reader.GetString(0),
                    reader.GetString(1)
                ),
                "P" => new PersonalComputer(
                    reader.IsDBNull(7) ? null : reader.GetString(7),
                    reader.GetBoolean(2),
                    reader.GetString(0),
                    reader.GetString(1)
                ),
                "SW" => new Smartwatch(
                    reader.IsDBNull(4) ? throw new InvalidOperationException($"BatteryPercentage is null for Smartwatch with ID: {id}") : reader.GetInt32(4),
                    reader.GetBoolean(2),
                    reader.GetString(0),
                    reader.GetString(1)
                ),
                _ => throw new NotSupportedException($"Unknown device type: {deviceType} for ID: {id}")
            };
            return device;
        }

        return null;
    }
    
    public async Task UpdateAsync(Device device)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var command = new SqlCommand("UpdateDevice", conn);
        await conn.OpenAsync();

        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@Id", device.Id);
        command.Parameters.AddWithValue("@Name", device.Name);
        command.Parameters.AddWithValue("@IsEnabled", device.TurnedOn);
        command.Parameters.AddWithValue("@BatteryPercentage", DBNull.Value);
        command.Parameters.AddWithValue("@OperationSystem", DBNull.Value);
        command.Parameters.AddWithValue("@IpAddress", DBNull.Value);
        command.Parameters.AddWithValue("@NetworkName", DBNull.Value);
        
        switch (device)
        {
            case Smartwatch sw:
                command.Parameters["@BatteryPercentage"].Value = sw.Battery;
                break;
            case PersonalComputer pc:
                command.Parameters["@OperationSystem"].Value = pc.OperatingSystem;
                break;
            case EmbeddedDevice ed:
                command.Parameters["@IpAddress"].Value = ed.IpAddress;
                command.Parameters["@NetworkName"].Value = ed.NetworkName;
                break;
        }
        await command.ExecuteNonQueryAsync();
    }

    public async Task<int> DeleteAsync(string id, string type)
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var command = new SqlCommand("DeleteDevice", conn);
        await conn.OpenAsync();
        
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Type", type);
        
        return await command.ExecuteNonQueryAsync();
    }
    
    public async Task<List<Device>> GetAllAsync()
    {
        await using var conn = new SqlConnection(_connectionString);
        await using var command = new SqlCommand("GetAllDevices", conn);
        command.CommandType = CommandType.StoredProcedure;

        await conn.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        var list = new List<Device>();

        while (await reader.ReadAsync())
        {
            var deviceType = reader.GetString(3);
            Device device = deviceType switch
            {
                "ED" => new EmbeddedDevice(
                    reader.GetString(5),
                     reader.GetString(6),
                    reader.GetString(0),
                    reader.GetString(1)
                ),
                "P" => new PersonalComputer(
                    reader.IsDBNull(7) ? null : reader.GetString(7),
                    reader.GetBoolean(2),
                    reader.GetString(0),
                    reader.GetString(1)
                ),
                "SW" => new Smartwatch(
                    reader.IsDBNull(4) ? throw new InvalidOperationException("BatteryPercentage is null") : reader.GetInt32(4),
                    reader.GetBoolean(2),
                    reader.GetString(0),
                    reader.GetString(1)
                ),
                _ => throw new NotSupportedException($"Unknown device type: {deviceType}")
            };
            list.Add(device);
        }
        return list;
    }
}