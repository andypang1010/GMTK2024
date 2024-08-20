using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class EndingCGManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public CGType cgType;

    void Start()
    {
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "EndingCutScene.mp4");
        videoPlayer.loopPointReached += Skip;
    }

    private void Skip(VideoPlayer source)
    {
        RootManager.Instance.GoMenu();
    }

    void Update()
    {
    }

    public enum CGType
    {
        OPENING,
        CLOSING
    }
}
