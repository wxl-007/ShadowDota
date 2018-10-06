using System.Collections;
using System.Collections.Generic;

public class SafeEnumerable<T> : IEnumerable<T>
{
    private readonly IEnumerable<T> m_Inner;
    private readonly object m_Lock;

    public SafeEnumerable(IEnumerable<T> inner, object @lock)
    {
        m_Lock = @lock;
        m_Inner = inner;
    }

    #region Implementation of IEnumerable

    public IEnumerator<T> GetEnumerator()
    {
        return new SafeEnumerator<T>(m_Inner.GetEnumerator(), m_Lock);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion
}