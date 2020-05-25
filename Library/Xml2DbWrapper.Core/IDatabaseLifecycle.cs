using System;

namespace Xml2DbMapper.Core
{
    public interface IDatabaseLifecycle : IDisposable
    {
        bool Created { get; }

        IDatabaseLifecycle Scope();
    }
}
