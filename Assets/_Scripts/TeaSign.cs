using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeaSign : MonoBehaviour
{
    public GameObject sign1;
    public GameObject sign2;

    // Start is called before the first frame update
    void Start()
    {
        sign2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RotateSign()
    {
        sign1.SetActive(false);
        sign2.SetActive(true);
    }
}
