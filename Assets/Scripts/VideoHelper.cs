using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;

public class VideoHelper : MonoBehaviour
{
    public void PlayVideo()
    {
        // #if !UNITY_EDITOR
        var video = WX.CreateVideo(new WXCreateVideoParam()
        {
            src = "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4",
            // src="https://6d61-mathrunruncsv-0gppeloj57600df4-1317479052.tcb.qcloud.la/Video/Part1/VID_20231001_134543.mp4?sign=e74b96ae2b004c3553619bc43e02c228&t=1697178703",
            objectFit = "contain",
            controls=true
        });
        WX.GetSystemInfoAsync(new GetSystemInfoAsyncOption()
        {
            success = (res) =>
            {
                video.width = res.windowWidth;
                video.height = res.windowHeight;
            }
        });
        video.RequestFullScreen(0);
        video.OnEnded(()=>
        {
            video.ExitFullScreen();
            video.Destroy();
        });
        // #endif
    }
}
