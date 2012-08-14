﻿namespace Cassette
{
    [ProtoBuf.ProtoContract]
    public enum AssetReferenceType
    {
        /// <summary>
        /// A reference to an asset in the same bundle as the referencing asset.
        /// </summary>
        [ProtoBuf.ProtoEnum]
        SameBundle,
        /// <summary>
        /// A reference to an asset in another bundle, or to an entire other bundle itself.
        /// </summary>
        [ProtoBuf.ProtoEnum]
        DifferentBundle,
        /// <summary>
        /// For example, a reference to an image from a CSS file.
        /// </summary>
        [ProtoBuf.ProtoEnum]
        RawFilename,
        /// <summary>
        /// A direct reference to a URL.
        /// </summary>
        [ProtoBuf.ProtoEnum]
        Url
    }
}
