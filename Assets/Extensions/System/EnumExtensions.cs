using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumExtensions
{
	public static int ToIndex(this Enum e)
	{
		var values = Enum.GetValues(e.GetType());
		return Array.IndexOf(values, e);
	}
}