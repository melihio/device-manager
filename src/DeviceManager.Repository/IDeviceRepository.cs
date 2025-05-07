using device_manager.models;

public interface IDeviceRepository
{
    Task<List<Device>> GetAllAsync();
    Task<Device?> GetByIdAsync(string id);
    Task AddAsync(Device device);
    Task UpdateAsync(Device device);
    Task<int> DeleteAsync(string id, string type);
}