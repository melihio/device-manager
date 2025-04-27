namespace device_manager.managers;

public static class ManagerFactory
{
    private static DeviceManager _deviceManagerInstance;
    
    public static DeviceManager GetDeviceManager(string filePath)
    {
        if (_deviceManagerInstance == null)
        {
            _deviceManagerInstance = new DeviceManager(filePath);
        }
        
        return _deviceManagerInstance;
    }
}