using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public Material CapturedMat;
    Renderer rend;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    public void Capture()
    {
        rend.material = CapturedMat;
    }

}
