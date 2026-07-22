using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Shared.Services.SaveSystem
{
    /// <summary>
    /// Coordinates the Save/Load process.
    /// Isolates Newtonsoft.Json logic from the Domain.
    /// Modules register themselves to support additive scene loading.
    /// </summary>
    public class SaveCoordinator : ISaveCoordinator
    {
        private readonly IStorageProvider _storageProvider;
        private readonly HashSet<ISavable> _savableModules = new HashSet<ISavable>();
        
        // Version integer for future migrations. If we change schemas, we increment this.
        private const int CurrentSaveVersion = 1;

        // Cache the master save data in memory after loading
        private MasterSaveData _cachedMasterSave;

        public SaveCoordinator(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public void RegisterSavable(ISavable savable)
        {
            if (_savableModules.Add(savable))
            {
                // If we already loaded the game from disk, immediately push the state to the newly registered module
                if (_cachedMasterSave != null && _cachedMasterSave.Modules != null)
                {
                    if (_cachedMasterSave.Modules.TryGetValue(savable.SaveKey, out string moduleStateJson))
                    {
                        try
                        {
                            savable.LoadFromState(moduleStateJson);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"[SaveCoordinator] Failed to load state into dynamically registered module {savable.SaveKey}: {ex.Message}");
                        }
                    }
                }
            }
        }

        public void UnregisterSavable(ISavable savable)
        {
            _savableModules.Remove(savable);
        }

        public async Task SaveGameAsync()
        {
            var masterSave = new MasterSaveData
            {
                Version = CurrentSaveVersion,
                Modules = new Dictionary<string, string>()
            };

            // Poll all registered modules for their serialized string state
            foreach (var module in _savableModules)
            {
                try
                {
                    masterSave.Modules[module.SaveKey] = module.GetSaveState();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[SaveCoordinator] Failed to get save state from module {module.SaveKey}: {ex.Message}");
                }
            }

            // Update cache
            _cachedMasterSave = masterSave;

            // Serialize the master payload
            string finalJson = JsonConvert.SerializeObject(masterSave, Formatting.None);
            
            // Push to the asynchronous storage provider
            await _storageProvider.WriteDataAsync(finalJson);
            
            Debug.Log("[SaveCoordinator] Game saved successfully.");
        }

        public async Task LoadGameAsync()
        {
            string rawJson = await _storageProvider.ReadDataAsync();

            if (string.IsNullOrEmpty(rawJson))
            {
                Debug.Log("[SaveCoordinator] No save data found. Proceeding with fresh state.");
                return;
            }

            try
            {
                _cachedMasterSave = JsonConvert.DeserializeObject<MasterSaveData>(rawJson);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveCoordinator] Failed to deserialize master save file. Data may be corrupted: {ex.Message}");
                return; // Optionally implement fallback/backup loading here in the future
            }

            if (_cachedMasterSave == null || _cachedMasterSave.Modules == null) return;

            // Optional: Handle migration logic here based on masterSave.Version before passing to modules

            // Push the specific JSON strings back into the respective modules
            foreach (var module in _savableModules)
            {
                if (_cachedMasterSave.Modules.TryGetValue(module.SaveKey, out string moduleStateJson))
                {
                    try
                    {
                        module.LoadFromState(moduleStateJson);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[SaveCoordinator] Failed to load state into module {module.SaveKey}: {ex.Message}");
                    }
                }
            }

            Debug.Log($"[SaveCoordinator] Game loaded successfully (Version {_cachedMasterSave.Version}).");
        }

        // Private wrapper class for the root JSON structure
        private class MasterSaveData
        {
            public int Version { get; set; }
            public Dictionary<string, string> Modules { get; set; }
        }
    }
}
