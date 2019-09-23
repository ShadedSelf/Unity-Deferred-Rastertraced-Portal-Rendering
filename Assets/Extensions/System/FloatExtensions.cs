using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Unity.Mathematics;
using static Unity.Mathematics.math;

public static class FloatExtensions
{
	public static bool Almost(this float number1, float number2, float precision = 0.0001f)
	{
		return abs(number1 - number2) < precision;
	}

	public static float RelativeTo(this float value, float min, float max)
	{
		return (value - max) / (min - max);
	}
}
