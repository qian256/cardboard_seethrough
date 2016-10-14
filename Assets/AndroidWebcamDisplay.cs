// Author: Long Qian
// Email: lqian8@jhu.edu

using UnityEngine;
using System.Collections;

public class AndroidWebcamDisplay : MonoBehaviour {
    
    private WebCamTexture webCameraTexture;

    // save the webcam texture
    private Material camTextureHolder;

    // shader of implementing fake AR effect
    public Material shaderMaterial;

    // backup image
    public Texture backupImage;

    // a null RenderTexture object to hold place in Graphics.Blit function
    private RenderTexture nullRenderTexture = null;

    // width and height of actual webcam texture
    private int webcamWidth, webcamHeight;

    // fps counter
    const float fpsMeasurePeriod = 0.5f;
    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    private int m_CurrentFps;
    private int fontSize = 40;


    // Use this for initialization
    void Start() {
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;

        Debug.Log("Screen width = " + Screen.width + ", height = " + Screen.height);

        // camTextureHolder needs to be initialized before using
        camTextureHolder = new Material(shaderMaterial);

        // Checks how many and which cameras are available on the device
        for (int cameraIndex = 0; cameraIndex < WebCamTexture.devices.Length; cameraIndex++) {
            
#if UNITY_EDITOR
            // In editor mode, the camera is usually front facing
            // grab any camera is fine for testing
            webCameraTexture = new WebCamTexture(cameraIndex, Screen.width * 10, Screen.height * 10);
#endif
            // grab the back facing camera on Android device
            if (!WebCamTexture.devices[cameraIndex].isFrontFacing) {
                webCameraTexture = new WebCamTexture(cameraIndex, Screen.width, Screen.height);                
            }

        }

        camTextureHolder.mainTexture = webCameraTexture;
        webCameraTexture.Play();

        webcamWidth = webCameraTexture.width;
        webcamHeight = webCameraTexture.height;

        Debug.Log("WebcamTexture width = " + webcamWidth + ", height = " + webcamHeight);

        float Alpha = (float)webcamHeight / (float)Screen.height * (float)Screen.width * 0.5f / (float)webcamWidth;
        shaderMaterial.SetFloat("_Alpha", Alpha);

        Debug.Log("Alpha value of Shader set to: " + Alpha);
    }


    public void ShowCamera() {
        //myCameraTexture.GetComponent<GUITexture>().enabled = true;
        webCameraTexture.Play();
    }

    public void HideCamera() {

        // myCameraTexture.GetComponent<GUITexture>().enabled = false;
        webCameraTexture.Stop();
    }

    void OnGUI() {
        GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = fontSize;
        // A button to demonstrate how to turn the camera on and off, in case you need it
        if (GUI.Button(new Rect(0, 0, 200, 60), "ON/OFF")) {
            if (webCameraTexture.isPlaying)
                this.HideCamera();
            else
                this.ShowCamera();
        }
        GUI.Label(new Rect(20, Screen.height - 60, 400, 50), webcamWidth + " x " + webcamHeight + "  " + m_CurrentFps + "fps");
    }

    // Update is called once per frame
    void Update() {
        ;

    }

    void OnRenderImage(RenderTexture src, RenderTexture dest) {
        Graphics.Blit(camTextureHolder.mainTexture, nullRenderTexture, shaderMaterial);

        // measure average frames per second
        m_FpsAccumulator++;
        if (Time.realtimeSinceStartup > m_FpsNextPeriod) {
            m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
            m_FpsAccumulator = 0;
            m_FpsNextPeriod += fpsMeasurePeriod;
        }
    }

}
