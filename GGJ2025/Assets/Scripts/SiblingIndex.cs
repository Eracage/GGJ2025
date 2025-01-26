using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiblingIndex : MonoBehaviour
{
    public int DesiredPosition = 0;
    // Update is called once per frame
    void Update()
    {
        transform.SetSiblingIndex(DesiredPosition);
    }
}
