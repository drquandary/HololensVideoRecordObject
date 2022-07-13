using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows.WebCam;
using System.Numerics;
using Microsoft.MixedReality.Toolkit;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

#if WINDOWS_UWP
using Windows.Storage;
#endif


public class Recordvideohandler : MonoBehaviour
{

    VideoCapture m_VideoCapture = null;

    public void StartRecordingVideo()
    {
        VideoCapture.CreateAsync(false, OnVideoCaptureCreated);
    }

    public void StopRecordingVideo()
    {
        m_VideoCapture.StopRecordingAsync(OnStoppedRecordingVideo);
    }

    void OnVideoCaptureCreated(VideoCapture videoCapture)
    {
        if (videoCapture != null)
        {
            m_VideoCapture = videoCapture;

            Resolution cameraResolution = VideoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            float cameraFramerate = VideoCapture.GetSupportedFrameRatesForResolution(cameraResolution).OrderByDescending((fps) => fps).First();

            CameraParameters cameraParameters = new CameraParameters();
            cameraParameters.hologramOpacity = 0.0f;
            cameraParameters.frameRate = cameraFramerate;
            cameraParameters.cameraResolutionWidth = cameraResolution.width;
            cameraParameters.cameraResolutionHeight = cameraResolution.height;
            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

            m_VideoCapture.StartVideoModeAsync(cameraParameters,
                                                VideoCapture.AudioState.None,
                                                OnStartedVideoCaptureMode);
        }
        else
        {
            Debug.LogError("Failed to create VideoCapture Instance!");
        }
    }

    void OnStartedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        if (result.success)
        {
            string filename = string.Format("MyVideo_{0}.mp4", Time.time);
            string filepath = System.IO.Path.Combine(Application.persistentDataPath, filename);

            m_VideoCapture.StartRecordingAsync(filepath, OnStartedRecordingVideo);
        }
    }

    void OnStartedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Started Recording Video!");
    }

    void OnStoppedRecordingVideo(VideoCapture.VideoCaptureResult result)
    {
        Debug.Log("Stopped Recording Video!");
        m_VideoCapture.StopVideoModeAsync(OnStoppedVideoCaptureMode);
    }

    void OnStoppedVideoCaptureMode(VideoCapture.VideoCaptureResult result)
    {
        m_VideoCapture.Dispose();
        m_VideoCapture = null;
    }
}