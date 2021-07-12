using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class which acts as the base class for both hat's managers
public class BaseSharedBetweenHats : Core.Utilities.Singleton<BaseSharedBetweenHats> {
	// Error codes used by the error handling system
	public class ErrorCodes {
		public static readonly int Generic = 0;
		public static readonly int WrongPlayer = 1;			// Error code stating that the wrong player tried to interact with the object
		public static readonly int InvalidTarget = 2;		// Error code stating that the selected target is invalid
		public static readonly int NoUpdatesRemaining = 3;	// Error code stating that the selected target doesn't have any updates left

		public int value = Generic;

		// Constructor
		public ErrorCodes(){}
		public ErrorCodes(int _value){
			value = _value;
		}

		// Object equality (Required to override ==)
		public override bool Equals(System.Object obj) {
			if (obj == null)
				return false;
			int? o = obj as int?;
			return Equals(o.Value);
		}

		// Details equality
		public bool Equals(int v){
			return value == v;
		}

		// Required to override Equals
		public override int GetHashCode() { return base.GetHashCode(); }

		// Equality Operator
		public static bool operator ==(ErrorCodes a, ErrorCodes b) => a.Equals(b);
		// Inequality Operator (Required if == is overriden)
		public static bool operator !=(ErrorCodes a, ErrorCodes b) => !a.Equals(b);

		public static implicit operator int(ErrorCodes e) => e.value;
		public static implicit operator ErrorCodes(int value) => new ErrorCodes(value);

	}


	// -- GameState Accessors --


	// Access the selected element
	public static T getSelected<T>() where T : MonoBehaviour, SelectionManager.ISelectable { return SelectionManager.instance.selected?.GetComponent<T>(); }

	public static GameObject[] getFirewallTargets(){ return GameObject.FindGameObjectsWithTag("FirewallTarget"); }
	public static GameObject[] getSwitchTargets(){ return GameObject.FindGameObjectsWithTag("SwitchTarget"); }


	// -- Error Handling --


	protected virtual void ErrorHandler(ErrorCodes errorCode, string error){
		Debug.LogError(error);
	}
	protected virtual void ErrorHandler(string error){ ErrorHandler(ErrorCodes.Generic, error); }
}
