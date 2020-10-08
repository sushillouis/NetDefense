using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Displaceable : MonoBehaviour
{
    public Vector3 EditPosition;
    public Vector3 ShowPosition;

    private void Awake() {
        EditPosition = transform.localPosition;
    }

    public bool show = false;

    public bool isValid {
        get { return show; }
        set {
            show = value;
            if (show)
                transform.localPosition = Vector3.zero;
            else
                transform.localPosition = EditPosition;
        }
    }

}
