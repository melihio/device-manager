using Microsoft.Data.SqlClient;

namespace device_manager.managers;

public class DatabaseManager
{
    private readonly string _connectionString;

    public DatabaseManager(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Initialize(string filePath)
    {
        EnsureDatabaseCreated();

        var lines = File.ReadAllLines(filePath);

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        foreach (var line in lines)
        {
            var parts = line.Split(',');

            string id = parts[0];
            string type = id.Split('-')[0].ToUpper();

            var command = new SqlCommand("INSERT INTO Devices (Id, Name, IPAddress, WifiName, TurnedOn, BatteryLevel, DeviceType) " +
                                         "VALUES (@Id, @Name, @IP, @Wifi, @TurnedOn, @Battery, @Type)", connection);

            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Type", type);

            switch (type)
            {
                case "P":
                    command.Parameters.AddWithValue("@Name", parts[1]);
                    command.Parameters.AddWithValue("@IP", DBNull.Value);
                    command.Parameters.AddWithValue("@Wifi", DBNull.Value);
                    command.Parameters.AddWithValue("@TurnedOn", bool.Parse(parts[2]));
                    command.Parameters.AddWithValue("@Battery", DBNull.Value);
                    break;

                case "ED":
                    command.Parameters.AddWithValue("@Name", parts[1]);
                    command.Parameters.AddWithValue("@IP", parts[2]);
                    command.Parameters.AddWithValue("@Wifi", parts[3]);
                    command.Parameters.AddWithValue("@TurnedOn", DBNull.Value);
                    command.Parameters.AddWithValue("@Battery", DBNull.Value);
                    break;

                case "SW":
                    command.Parameters.AddWithValue("@Name", parts[1]);
                    command.Parameters.AddWithValue("@IP", DBNull.Value);
                    command.Parameters.AddWithValue("@Wifi", DBNull.Value);
                    command.Parameters.AddWithValue("@TurnedOn", bool.Parse(parts[2]));
                    command.Parameters.AddWithValue("@Battery", parts[3].Replace('%',' '));
                    break;

                default:
                    Console.WriteLine($"Unrecognized type: {type} for line: {line}");
                    continue;
            }

            command.ExecuteNonQuery();
        }

        Console.WriteLine("Database initialized successfully.");
    }

    private void EnsureDatabaseCreated()
    {
        string createTableSql = @"
            IF NOT EXISTS (
                SELECT * FROM sysobjects WHERE name='Devices' AND xtype='U'
            )
            BEGIN
                CREATE TABLE Devices (
                    Id NVARCHAR(20) PRIMARY KEY,
                    Name NVARCHAR(100) NOT NULL,
                    IPAddress NVARCHAR(50) NULL,
                    WifiName NVARCHAR(100) NULL,
                    TurnedOn BIT NULL,
                    BatteryLevel NVARCHAR(10) NULL,
                    DeviceType NVARCHAR(20) NOT NULL
                );
            END";

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var command = new SqlCommand(createTableSql, connection);
        command.ExecuteNonQuery();
    }
}
