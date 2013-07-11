using Cassette;
using Cassette.Configuration;
using Cassette.DependencyGraphInteration;
using Cassette.DependencyGraphInteration.InMemory;

namespace CassetteHostingEnvironment.Hosting
{
    public class HostedDependencyGraphInteractionFactory : IDependencyGraphInteractionFactory
    {
        ICassetteApplication _application;
        public HostedDependencyGraphInteractionFactory(ICassetteApplication application = null)
        {
            _application = application;
        }

        public IInteractWithDependencyGraph GetDependencyGraphInteration(CassetteSettings settings)
        {
            return new InMemoryDependencyGraphInteraction(_application);
        }

        public void SetCassetteApplication(ICassetteApplication app)
        {
            _application = app;
        }
    }
}
