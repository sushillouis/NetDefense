using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class StreamURLAtRuntime : MonoBehaviour
{

    public string path;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<VideoPlayer>().url = System.IO.Path.Combine(Application.streamingAssetsPath,"/"+ path);

        GetComponent<VideoPlayer>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
