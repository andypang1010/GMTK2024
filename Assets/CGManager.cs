using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CGManager : MonoBehaviour
{

    public VideoPlayer videoPlayer;
    public CGType cgType;

    void Start()
    {
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "CutSceneOpening.mp4");
        videoPlayer.loopPointReached += Skip;
    }

    private void Skip(VideoPlayer source)
    {
        RootManager.Instance.GoTutorial();
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            RootManager.Instance.GoTutorial();
        }
    }

    public enum CGType
    {
        OPENING,
        CLOSING
    }
}
