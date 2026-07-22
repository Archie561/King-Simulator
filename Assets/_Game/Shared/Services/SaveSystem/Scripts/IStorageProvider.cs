using System.Threading.Tasks;

namespace Game.Shared.Services.SaveSystem
{
    /// <summary>
    /// Abstracts the raw I/O storage mechanism (e.g., Local File, Firebase).
    /// All methods must be asynchronous to support future cloud implementations.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Asynchronously writes the master JSON payload to storage.
        /// </summary>
        Task WriteDataAsync(string data);

        /// <summary>
        /// Asynchronously reads the master JSON payload from storage.
        /// Returns null or an empty string if no save exists.
        /// </summary>
        Task<string> ReadDataAsync();
    }
}
