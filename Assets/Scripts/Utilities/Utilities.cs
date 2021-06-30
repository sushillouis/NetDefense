using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities {

	// Function which removes unnecessary parts from the prefab path that can be copied from unity. If these parts are not removed then the resource loader will fail to find the prefab!
	public static void PreparePrefabPath(ref string path) { path = path.Replace("Assets/Resources/", "").Replace(".prefab", ""); }
	public static void PreparePrefabPaths(ref string[] paths){
		for(int i = 0; i < paths.Length; i++)
			PreparePrefabPath(ref paths[i]);
	}

	// Function which determines if a point <p> lies on a line segment (is colinear with the line and between the two end points <a> and <b>)
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

	// Creates a Vector3 where every axis is the same (value)
	public static Vector3 toVec(float value){
		return new Vector3(value, value, value);
	}

	// Returns a new Vector3 with the given <y> value applied to the given <position>
	public static Vector3 positionSetY(Vector3 position, float y){
		return new Vector3(position.x, y, position.z);
	}

	// Returns a new Vector3 where the y value of the given <position> is 0
	public static Vector3 positionNoY(Vector3 position) {
		return positionSetY(position, 0);
	}

	// Returns a random enum value
	public static T randomEnum <T>() where T : Enum {
    	var v = Enum.GetValues (typeof (T));
    	return (T) v.GetValue (UnityEngine.Random.Range(0, v.Length));
	}

	// Gets a list of components that implement the specified interface on the provided game object
	// Code from: https://answers.unity.com/questions/523409/strategy-pattern-with-monobehaviours.html
	public static void GetInterfaces<T>(out List<T> resultList, GameObject objectToSearch) where T: class {
         MonoBehaviour[] list = objectToSearch.GetComponents<MonoBehaviour>();
         resultList = new List<T>();
         foreach(MonoBehaviour mb in list){
             if(mb is T){
                 //found one
                 resultList.Add((T)((System.Object)mb));
             }
         }
     }

}
