using System.Collections;
using System.Collections.Generic;

public class Thread_Safe_Linkedlist<T> : IEnumerable<T> {
    private LinkedList<T> m_Inner;

    // this is the object we shall lock on. 
    private readonly object m_Lock = new object();

    public IEnumerator<T> GetEnumerator()
    {
        // instead of returning an usafe enumerator,
        // we wrap it into our thread-safe class
        return new SafeEnumerator<T>(m_Inner.GetEnumerator(), m_Lock);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // To be actually thread-safe, our collection
    // must be locked on all other operations
    // For example, this is how Add() method should look
    public void Add(T item)
    {
        lock (m_Lock)
            m_Inner.AddLast(item);
    }

    // To make thread-safe, our collection must be locked on remove
    public void Remove(T item)
    {
        lock (m_Lock)
            m_Inner.Remove(item);
    }

    public void Remove(List<T> items) {
        lock (m_Lock) {
            foreach(T t in items) {
                m_Inner.Remove(t);
            }
        }
    }


    public void Clear()
    {
        lock (m_Lock)
            m_Inner.Clear();
    }

	public int Count {
		get {
			lock(m_Lock)
				return m_Inner.Count;
		}
	}

	public Thread_Safe_Linkedlist () {
		m_Inner = new LinkedList<T> ();
	}

}

/*
 * 
 * ******  Following is another way of creating thread-safe ie ******
 * 
 */ 
public static class EnumerableExtension
{
    public static IEnumerable<T> AsLocked<T>(this IEnumerable<T> ie, object @lock)
    {
        return new SafeEnumerable<T>(ie, @lock);
    }
}


// in a class...
public class MyThreadSafeEnumerable<T>
{
    // come collection of items..
	private IEnumerable<T> m_Items = null;
    private readonly object m_Lock = new object();
    // and thread-safe getter for them!
	public IEnumerable<T> Items {
		get {
            return m_Items.AsLocked(m_Lock);
        }
    }
}


//
// .. or simply in loop
//foreach(var item in someList.AsLocked(someLock)){
    // ...
//}
//
