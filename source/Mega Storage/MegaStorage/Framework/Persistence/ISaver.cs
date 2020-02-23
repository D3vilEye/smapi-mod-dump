﻿namespace MegaStorage.Framework.Persistence
{
    public interface ISaver
    {
        string SaveDataKey { get; }

        void LoadCustomChests();
        void ReAddCustomChests();
        void HideAndSaveCustomChests();
    }
}
