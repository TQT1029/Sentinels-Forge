using System;
using System.Collections.Generic;

namespace SentinelForge.Core.Persistence
{
    [Serializable]
    public class InventoryItemSaveData
    {
        public string itemID;
        public int quantity;
    }

    [Serializable]
    public class BaseUpgradeSaveData
    {
        public string upgradeID;
        public int level;
    }

    /// <summary>
    /// Dữ liệu lưu trữ tổng thể của người chơi (Meta-progression, vàng, túi đồ, nâng cấp tháp).
    /// </summary>
    [Serializable]
    public class SaveData
    {
        public int currentStage = 1;
        public int currentWave = 1;
        public int gold = 0;
        public List<string> unlockedBlueprintIDs = new();
        public List<InventoryItemSaveData> inventory = new();
        public List<BaseUpgradeSaveData> baseUpgrades = new();
        public long lastSaveTimestamp;
    }
}
