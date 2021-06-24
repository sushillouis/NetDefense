using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities {

	// Function which determines if a point <p> lies on a line segement (is colinear with the line and between the two end points <a> and <b>)
	public static bool isBetweenAndColinear(Vector3 a, Vector3 b, Vector3 p){
		// Direction vectors used throughout the calculation
		Vector3 bMinA = b - a, pMinA = p - a;

		Vector3 cross = Vector3.Cross(bMinA, pMinA);
		if(cross.magnitude > Mathf.Epsilon) return false; // If the cross product isn't 0 then the vectors aren't colinear

		float dot = Vector3.Dot(bMinA, pMinA);
		// If the dot product isn't positive and less than the square magnitude of b-a then the point doesn't fall between the two endpoints
		if(dot < 0 || dot > bMinA.sqrMagnitude) return false;

		// If none of the cases fail then the point is between the two end points
		return true;
	}
}
