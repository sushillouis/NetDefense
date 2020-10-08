using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneypotPath : MonoBehaviour
{
    private List<Transform> pathPieces;

    private void Start()
    {
        pathPieces = new List<Transform>();
        for(int i = 0; i < transform.childCount; i++)
        {
            pathPieces.Add(transform.GetChild(i));
        }
    }

    public IEnumerator RaisePath()
    {
        foreach(Transform t in pathPieces)
        {
            Vector3 newPos = t.position;
            newPos.y = 0.0f;
            t.position = newPos;
            yield return new WaitForSeconds(0.025f);
        }
    }

    public IEnumerator LowerPath()
    {
        foreach (Transform t in pathPieces)
        {
            Vector3 newPos = t.position;
            newPos.y = -1.0f;
            t.position = newPos;
            yield return new WaitForSeconds(0.025f);
        }
    }
}
