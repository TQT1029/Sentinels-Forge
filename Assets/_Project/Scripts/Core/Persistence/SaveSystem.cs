using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SentinelForge.Core.Interfaces;

namespace SentinelForge.Core.Persistence
{
    /// <summary>
    /// Quản lý việc đọc/ghi dữ liệu người chơi theo định dạng JSON tại Application.persistentDataPath.
    /// Hỗ trợ cơ chế sao lưu .bak để chống hỏng file khi crash bất ngờ.
    /// </summary>
    public class SaveSystem
    {
        private const string SAVE_FILE_NAME = "savegame.json";
        private const string BACKUP_FILE_NAME = "savegame.json.bak";

        private readonly string _saveFilePath;
        private readonly string _backupFilePath;
        private readonly List<ISaveable> _registeredSaveables = new();

        public SaveData CurrentSaveData { get; private set; }

        public SaveSystem()
        {
            _saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            _backupFilePath = Path.Combine(Application.persistentDataPath, BACKUP_FILE_NAME);
            CurrentSaveData = new SaveData();
        }

        public void RegisterSaveable(ISaveable saveable)
        {
            if (saveable != null && !_registeredSaveables.Contains(saveable))
            {
                _registeredSaveables.Add(saveable);
            }
        }

        public void UnregisterSaveable(ISaveable saveable)
        {
            if (saveable != null)
            {
                _registeredSaveables.Remove(saveable);
            }
        }

        /// <summary>
        /// Thu thập dữ liệu từ tất cả ISaveable và ghi ra đĩa.
        /// </summary>
        public bool Save()
        {
            try
            {
                CurrentSaveData.lastSaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                // Chuyển đổi dữ liệu sang định dạng JSON
                string json = JsonUtility.ToJson(CurrentSaveData, prettyPrint: true);

                // Cơ chế ghi an toàn: sao lưu file cũ trước khi ghi file mới
                if (File.Exists(_saveFilePath))
                {
                    File.Copy(_saveFilePath, _backupFilePath, overwrite: true);
                }

                File.WriteAllText(_saveFilePath, json);
                Debug.Log($"[SaveSystem] Đã lưu game thành công vào: {_saveFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Lỗi khi lưu game: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Đọc file JSON từ đĩa và khôi phục vào CurrentSaveData.
        /// </summary>
        public bool Load()
        {
            try
            {
                string targetPath = _saveFilePath;

                if (!File.Exists(targetPath))
                {
                    if (File.Exists(_backupFilePath))
                    {
                        Debug.LogWarning("[SaveSystem] File save chính bị mất, phục hồi từ file backup .bak");
                        targetPath = _backupFilePath;
                    }
                    else
                    {
                        Debug.Log("[SaveSystem] Chưa có file save. Khởi tạo dữ liệu mới.");
                        CurrentSaveData = new SaveData();
                        return false;
                    }
                }

                string json = File.ReadAllText(targetPath);
                CurrentSaveData = JsonUtility.FromJson<SaveData>(json);

                if (CurrentSaveData == null)
                {
                    CurrentSaveData = new SaveData();
                }

                Debug.Log($"[SaveSystem] Đã nạp thành công file save từ: {targetPath}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Lỗi khi nạp file save: {ex.Message}");
                CurrentSaveData = new SaveData();
                return false;
            }
        }

        /// <summary>
        /// Xóa file save để chơi lại từ đầu.
        /// </summary>
        public void DeleteSave()
        {
            if (File.Exists(_saveFilePath)) File.Delete(_saveFilePath);
            if (File.Exists(_backupFilePath)) File.Delete(_backupFilePath);
            CurrentSaveData = new SaveData();
            Debug.Log("[SaveSystem] Đã xóa toàn bộ file save.");
        }
    }
}
