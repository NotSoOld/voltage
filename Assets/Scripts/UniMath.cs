using UnityEngine;
using System.Collections;

public class UniMath  {

	public static bool ApproximatelyEqual(float a, float b, float precision = 0.01f)  {
		if(Mathf.Abs(a - b) < precision)
			return true;
		else return false;
	}

	public static bool ApproximatelyEqual(Vector3 a, Vector3 b, float precision = 0.01f)  {
		if((Mathf.Abs(a.x - b.x) < precision) && (Mathf.Abs(a.y - b.y) < precision) && (Mathf.Abs(a.z - b.z) < precision))
			return true;
		else return false;
	}

}