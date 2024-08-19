using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flamingo : MonoBehaviour
{
    public Transform bodySprite;
    public Transform feetSprite;

    public Transform bodyTarget;
    public Transform feetTarget;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bodySprite.transform.position = bodyTarget.transform.position;
        feetSprite.transform.position = feetTarget.transform.position;
    }
}
