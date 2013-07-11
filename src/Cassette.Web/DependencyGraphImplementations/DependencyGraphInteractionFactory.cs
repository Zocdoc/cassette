using Cassette.Configuration;
using Cassette.DependencyGraphInteration.InMemory;
using Cassette.Web.DependencyGraphImplementations;
using CassetteHostingEnvironment.DependencyGraphInteration.Service;

namespace Cassette.DependencyGraphInteration
{
    public class DependencyGraphInteractionFactory : IDependencyGraphInteractionFactory
    {
        ICassetteApplication _application;
        public DependencyGraphInteractionFactory(ICassetteApplication application = null)
        {
            this._application = application;
        }

        public IInteractWithDependencyGraph GetDependencyGraphInteration(CassetteSettings settings)
        {
            if (settings.UseOutOfProcessCassette && settings.IsDebuggingEnabled)
            {
                return new WcfServiceDependencyGraphInteraction(settings,
                                                                new BundleRequestedPerRequestProvider(),
                                                                new InterationServiceUtility());
            }

            return new InMemoryDependencyGraphInteraction(_application);
        }

        public void SetCassetteApplication(ICassetteApplication app)
        {
            _application = app;
        }
    }
}
