using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public delegate void ButtonFunction();
[RequireComponent(typeof(VideoPlayer))]
[RequireComponent(typeof(AudioSource))]
public class VideoPlayerCtrl : MonoBehaviour
{
    public static VideoPlayerCtrl ctrl;
    public VideoRenderMode renderMode = VideoRenderMode.CameraFarPlane;
    public VideoAudioOutputMode audioOutputMode = VideoAudioOutputMode.AudioSource;    
    public bool playOnAwake, runInBackground = true;
    public Scrollbar timeBar;
    public Button playBtn;
    public Sprite PlaySprite;
    public Sprite PauseSprite;
    
    private ButtonFunction btnFunction;
    private VideoPlayer videoPlayer
    {
        get { return GetComponent<VideoPlayer>(); }
    }
    private AudioSource audioSource
    {
        get { return GetComponent<AudioSource>(); }
    }
    public bool isPause = true;

    private void Awake()
    {
        ctrl = this;
    }

    void Start()
    {
        SetMode();//設定初始狀態 
    }

    void SetMode()
    {
        //參數設定
        Application.runInBackground = runInBackground;
        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;
        //videoPlayer.clip = videoClip;
        videoPlayer.audioOutputMode = audioOutputMode;
        //聲音撥放設定
        if (audioOutputMode == VideoAudioOutputMode.AudioSource)
        {
            videoPlayer.EnableAudioTrack(0, true);
            videoPlayer.SetTargetAudioSource(0, audioSource);
        }
        //影像魔是設定
        videoPlayer.renderMode = renderMode;        
        //StartCoroutine("VideoPrepare");//準備影片
    }

    public void SetVideoPath(string path)
    {
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = "file://"+path;        
        StartCoroutine("VideoPrepare");//準備影片
    }

    IEnumerator VideoPrepare()
    {
        videoPlayer.Prepare();//讀取素材影片
        while (!videoPlayer.isPrepared)
        {
            Debug.Log("Preparing Video");
            yield return null;
        }
        Debug.Log("Video is Prepared");

        if (playOnAwake) Play();//是否直接撥放
    }

    //撥放功能鈕
    public void ButtonFN()
    {        
        btnFunction = videoPlayer.isPlaying ? (ButtonFunction)Pause : (ButtonFunction)Play;
        btnFunction();
    }

    public void Play()
    {
        videoPlayer.Play();
        isPause = false;
        playBtn.image.sprite = PauseSprite;
    }

    public void Pause()
    {
        videoPlayer.Pause();
        isPause = true;
        playBtn.image.sprite = PlaySprite;
    }

    //停止
    public void Stop()
    {
        videoPlayer.Stop();
        timeBar.value = 0;
    }


    void Update()
    {

        if (timeBar)
        {
            if (videoPlayer.isPlaying && !isPause)
            {
                //時間條同步
                timeBar.value = (float)(videoPlayer.time / videoPlayer.length);
            }
            else
            {
                //拖曳改變影片時間
                videoPlayer.time = videoPlayer.length * timeBar.value;
            }
        }
    }

}