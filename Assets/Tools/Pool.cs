using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

public class Pool<T> where T : new()
{
	private ConcurrentBag<T> obs = new ConcurrentBag<T>();

	public int count => obs.Count;

	public void Release(T ob)
	{
		obs.Add(ob);
	}

	public T Get()
	{
		if (obs.TryTake(out var ob))
			return ob;
		return new T();
	}
}