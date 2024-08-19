using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRepeater : MonoBehaviour
{
    public int numberOfRepetition;
    public Transform repeatPosition;
    public GameObject repeatObject;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 startPos = repeatObject.transform.position;
        Vector2 posOffset = repeatPosition.position - repeatObject.transform.position;
        for (int i = 0; i < numberOfRepetition; i++)
        {
            Instantiate(repeatObject, startPos + (i+1) * posOffset, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
