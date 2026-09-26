namespace SentinelForge.Core.Interfaces
{
    /// <summary>
    /// Interface cho các GameObject hỗ trợ tái sử dụng từ PoolManager.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// Được gọi ngay khi object được lấy ra từ Pool.
        /// </summary>
        void OnGetFromPool();

        /// <summary>
        /// Được gọi ngay trước khi object được trả ngược về Pool.
        /// </summary>
        void OnReturnToPool();
    }
}
