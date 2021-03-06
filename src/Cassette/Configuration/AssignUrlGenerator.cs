using Cassette.Configuration;

namespace Cassette.Configuration
{
    public class AssignUrlGenerator : ICassetteConfiguration
    {
        public void Configure(BundleCollection bundles, CassetteSettings settings)
        {
            if (settings.UrlGenerator == null)
            {
                settings.UrlGenerator = new UrlGenerator(settings.UrlModifier, UrlGenerator.RoutePrefix);
            }
        }
    }
}