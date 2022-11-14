using System;
using System.Linq;

namespace Nop.Data
{
    /// <summary>
    /// Represents temporary storage
    /// </summary>
    /// <typeparam name="T">Storage record mapping class</typeparam>
    public interface ITempDataStorage<out T> : IQueryable<T>, IDisposable, IAsyncDisposable where T : class //promenjeno
    {
    }
}