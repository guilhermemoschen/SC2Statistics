using Microsoft.Practices.Unity;

namespace SC2LiquipediaStatistics.Utilities.Unity
{
    public class Container
    {
        public static IUnityContainer Instance { get; private set; }

        public static void Configure()
        {
            Instance = new UnityContainer();
        }

        public static T Resolve<T>()
        {
            return Instance.Resolve<T>();
        }

        public static T Resolve<T>(params ResolverOverride[] overrides)
        {
            return Instance.Resolve<T>(overrides);
        }
    }
}
