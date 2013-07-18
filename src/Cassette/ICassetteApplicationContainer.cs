using System;
using Cassette.DependencyGraphInteration;

namespace Cassette
{
    interface ICassetteApplicationContainer<out T> : IDisposable
        where T : ICassetteApplication
    {
        T Application { get; }
        void RecycleApplication(IDependencyGraphInteractionFactory factory);
    }
}