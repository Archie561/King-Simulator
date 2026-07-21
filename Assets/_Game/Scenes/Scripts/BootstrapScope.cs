using VContainer;
using VContainer.Unity;

namespace Game.Root
{
    public class BootstrapScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Register services or presenters specific to the Bootstrap scene here.
            // Global services are already registered in RootLifetimeScope.
        }

        protected override void Awake()
        {
            base.Awake();
            // Start the initial game flow here, e.g., load the Main Menu or first Canvas.
        }
    }
}
