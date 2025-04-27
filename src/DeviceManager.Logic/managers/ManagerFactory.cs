namespace device_manager.managers;

/// <summary>
///  Factory of the existing classes in the project.
///  It prevents from creating multiple instances in different blocks and makes it easier to use the manager
/// </summary>
public static class ManagerFactory
{
    private static DeviceManager _deviceManagerInstance;

    /// <summary>
    ///  Returns an instance of device manager.
    /// </summary>
    public static DeviceManager GetDeviceManager(string connectionString)
    {
        if (_deviceManagerInstance == null)
        {
            _deviceManagerInstance = new DeviceManager(connectionString);
        }
        
        return _deviceManagerInstance;
    }
}