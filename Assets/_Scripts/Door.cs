using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Transform moveToTransform;

    private Vector2 moveToPos;
    private Vector2 originalPos;

    // Start is called before the first frame update
    void Start()
    {
        moveToPos = moveToTransform.position;
        originalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Open()
    {
        transform.position = moveToPos;
    }

    public void Close()
    {
        transform.position = originalPos;
    }
}
