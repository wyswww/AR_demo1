using UnityEngine;
using UnityEngine.Video;

public class control_button : MonoBehaviour
{
    [Header("VideoPlayer")]
    public VideoPlayer ryunn_Video;


    public void VideoPlayPause()
    {
        if (ryunn_Video != null)
        {
            if (ryunn_Video.isPlaying)
            {
                ryunn_Video.Pause();
                Debug.Log("视频暂停");
            }
            else
            {
                ryunn_Video.Play();
                Debug.Log("视频播放");
            }
        }
    }
}