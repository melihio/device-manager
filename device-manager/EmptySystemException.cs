namespace device_manager;

public class EmptySystemException: Exception
{
    public EmptySystemException()
        : base("The PC has no operating system.")
    {
    }
}