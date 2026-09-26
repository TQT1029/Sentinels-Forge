namespace SentinelForge.Core.Interfaces
{
    /// <summary>
    /// Interface cho các Manager hoặc thành phần có dữ liệu cần được lưu và phục hồi.
    /// </summary>
    public interface ISaveable
    {
        /// <summary>
        /// Key định danh duy nhất cho dữ liệu của module này trong file Save.
        /// </summary>
        string SaveKey { get; }

        /// <summary>
        /// Thu thập trạng thái hiện tại dưới dạng object để lưu trữ.
        /// </summary>
        object CaptureState();

        /// <summary>
        /// Khôi phục trạng thái từ dữ liệu đã load.
        /// </summary>
        /// <param name="state">Dữ liệu thô đọc từ SaveData.</param>
        void RestoreState(object state);
    }
}
