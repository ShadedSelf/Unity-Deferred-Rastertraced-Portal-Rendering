using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumIndexer<T> where T : Enum
{
	private T[] values;

	public int count			=> values.Length;
	public T this[int index]	=> values[index];
	public int this[T e]		=> Array.IndexOf(values, e);
	
	public EnumIndexer() => values = (T[])Enum.GetValues(typeof(T));
}