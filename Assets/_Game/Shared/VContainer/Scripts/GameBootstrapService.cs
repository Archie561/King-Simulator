using System.Threading;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;
using Game.Shared.Services.SaveSystem;
using UnityEngine;
using System.Threading.Tasks;

namespace Game.Root
{
    /// <summary>
    /// Pure C# bootstrapper that runs safely within VContainer's lifecycle.
    /// Replaces MonoBehavior Start() to prevent async void anti-patterns.
    /// </summary>
    public class GameBootstrapService : IAsyncStartable
    {
        private readonly ISaveCoordinator _saveCoordinator;
        private readonly LifetimeScope _rootScope;

        public GameBootstrapService(ISaveCoordinator saveCoordinator, LifetimeScope rootScope)
        {
            _saveCoordinator = saveCoordinator;
            _rootScope = rootScope;
        }

        public async Awaitable StartAsync(CancellationToken cancellation)
        {
            Debug.Log("[GameBootstrapService] Booting Game...");

            // 1. Await the disk I/O to ensure save state is cached in memory
            await _saveCoordinator.LoadGameAsync();

            // 2. Enqueue the root scope as parent for the next loaded scene
            using (LifetimeScope.EnqueueParent(_rootScope))
            {
                // 3. Additively load the main game scene
                var asyncOp = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
                
                // Wait for the scene to finish loading
                while (!asyncOp.isDone && !cancellation.IsCancellationRequested)
                {
                    await Task.Yield();
                }
            }

            Debug.Log("[GameBootstrapService] Main Scene Loaded. Game Boot Complete.");
        }
    }
}
