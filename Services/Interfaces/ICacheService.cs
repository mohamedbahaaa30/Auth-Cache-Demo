namespace AuthDemo.Services.Interfaces
{
    public interface ICacheService
    {
        public Task<T> GetDataAsync<T>(string key);
        public Task<bool> SetDataAsync<T>(string key, T value, DateTimeOffset expiration);
        public Task<bool> RemoveDataAsync(string key);
    }
}
