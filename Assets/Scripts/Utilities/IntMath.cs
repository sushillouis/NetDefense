using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntMath
{
	public static int Clamp(int value, int min, int max){
		if(value > max) return max;
		if(value < min) return min;
		return value;
	}
}
