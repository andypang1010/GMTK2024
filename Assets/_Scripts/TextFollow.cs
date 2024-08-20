using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

public class TextFollow : MonoBehaviour
{
    public Transform followTarget;
    public CinemachineVirtualCamera virtualCam;

    private TMP_Text tmPro;

    // Start is called before the first frame update
    void Start()
    {
        tmPro = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(followTarget.position);
        //tmPro.fontSize = 200 / virtualCam.m_Lens.OrthographicSize;
    }
}
