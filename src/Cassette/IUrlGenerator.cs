namespace Cassette
{
    public interface IUrlGenerator
    {
        string CreateBundleUrl(Bundle bundle, bool absoluteUrl = false);
        string CreateAssetUrl(IAsset asset, bool absoluteUrl = false);
        string CreateRawFileUrl(string filename, string hash);
    }
}