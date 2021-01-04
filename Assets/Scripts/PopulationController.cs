using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DebugGUI.LogPersistent("fps", "FPS: " + (1.0f / Time.deltaTime).ToString("F0"));
    }
}
