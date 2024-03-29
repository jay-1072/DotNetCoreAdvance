﻿namespace DotNetCoreAdvance.Cache
{
    public interface ICacheService
    {
        Task<T> GetData<T>(string key);

        Task SetData<T>(string key, T value, DateTimeOffset expirationTime);

        Task RemoveData(string key);
    }
}
