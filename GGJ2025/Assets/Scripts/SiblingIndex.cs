using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiblingIndex : MonoBehaviour
{
    public int DesiredPosition = 0;
    // Update is called once per frame
    void Update()
    {
        if (DesiredPosition < 0)
        {
            int pos = transform.parent.childCount + DesiredPosition;
            Debug.Log(pos + " from " + DesiredPosition);
            transform.SetSiblingIndex(pos);
            return;
        }
        transform.SetSiblingIndex(DesiredPosition);
    }
}
