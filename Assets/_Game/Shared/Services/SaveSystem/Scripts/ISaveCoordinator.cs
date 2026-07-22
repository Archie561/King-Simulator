using System.Threading.Tasks;

namespace Game.Shared.Services.SaveSystem
{
    /// <summary>
    /// The central orchestrator for the Save System.
    /// Injected where save/load operations need to be triggered (e.g., Bootstrapper, Settings Menu).
    /// </summary>
    public interface ISaveCoordinator
    {
        /// <summary>
        /// Registers a module to be included in future Save/Load operations.
        /// </summary>
        void RegisterSavable(ISavable savable);

        /// <summary>
        /// Unregisters a module.
        /// </summary>
        void UnregisterSavable(ISavable savable);

        /// <summary>
        /// Asynchronously collects state from all ISavable modules and writes them via the IStorageProvider.
        /// </summary>
        Task SaveGameAsync();

        /// <summary>
        /// Asynchronously reads the master save from the IStorageProvider and pushes state to all ISavable modules.
        /// </summary>
        Task LoadGameAsync();
    }
}
