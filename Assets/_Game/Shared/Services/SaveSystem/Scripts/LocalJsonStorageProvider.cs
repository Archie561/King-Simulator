using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Shared.Services.SaveSystem
{
    /// <summary>
    /// Local implementation of IStorageProvider.
    /// Uses Application.persistentDataPath and implements strict Atomic Saving 
    /// to prevent file corruption during crashes or interrupted writes.
    /// </summary>
    public class LocalJsonStorageProvider : IStorageProvider
    {
        private const string SaveFileName = "save.json";
        private const string TempFileName = "save.tmp";

        private string FinalPath => Path.Combine(Application.persistentDataPath, SaveFileName);
        private string TempPath => Path.Combine(Application.persistentDataPath, TempFileName);

        public async Task WriteDataAsync(string data)
        {
            try
            {
                // 1. Write the new data to a temporary file (Atomic Save pattern)
                await File.WriteAllTextAsync(TempPath, data);

                // 2. Safely replace the old save file with the temp file
                if (File.Exists(FinalPath))
                {
                    // True enables overwriting the existing file
                    File.Move(TempPath, FinalPath);
                }
                else
                {
                    File.Move(TempPath, FinalPath);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LocalJsonStorageProvider] Failed to write save data: {ex.Message}");
                // If writing to TempPath failed, the original FinalPath is completely unharmed.
            }
        }

        public async Task<string> ReadDataAsync()
        {
            if (!File.Exists(FinalPath))
            {
                return string.Empty;
            }

            try
            {
                return await File.ReadAllTextAsync(FinalPath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LocalJsonStorageProvider] Failed to read save data: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
