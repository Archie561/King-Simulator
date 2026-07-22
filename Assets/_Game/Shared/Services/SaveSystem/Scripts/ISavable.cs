namespace Game.Shared.Services.SaveSystem
{
    /// <summary>
    /// Implemented by any Domain Manager or Model that needs to persist state.
    /// Exclusively exchanges raw JSON strings to prevent Domain coupling to serialization libraries.
    /// </summary>
    public interface ISavable
    {
        /// <summary>
        /// The unique key used to identify this module in the Master Save object.
        /// </summary>
        string SaveKey { get; }

        /// <summary>
        /// Returns the module's state serialized as a JSON string.
        /// </summary>
        string GetSaveState();

        /// <summary>
        /// Loads the module's state from a JSON string.
        /// </summary>
        void LoadFromState(string jsonState);
    }
}
