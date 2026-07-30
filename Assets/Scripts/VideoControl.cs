using UnityEngine;
using UnityEngine.Video;

public class VideoControl : MonoBehaviour
{
    public VideoPlayer ryunn_Video;

    // 按钮调用播放
    public void VideoPlay()
    {
        if (ryunn_Video != null)
            ryunn_Video.Play();
    }

    // 按钮调用暂停
    public void VideoPause()
    {
        if (ryunn_Video != null)
            ryunn_Video.Pause();
    }
}
