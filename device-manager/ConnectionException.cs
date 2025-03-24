namespace device_manager;

public class ConnectionException: Exception
{
    public ConnectionException()
        : base("Invalid network name")
    {
    }
}