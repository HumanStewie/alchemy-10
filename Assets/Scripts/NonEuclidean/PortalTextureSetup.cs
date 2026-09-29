using UnityEngine;
using UnityEngine.Rendering;

public class PortalTextureSetup : MonoBehaviour
{
    public Camera camera_in;
    public Camera camera_out;
    public Material cameraMat_in;
    public Material cameraMat_out;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (camera_out.targetTexture != null)
        {
            camera_out.targetTexture.Release();
        }
        camera_out.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraMat_out.mainTexture = camera_out.targetTexture;

        if (camera_in.targetTexture != null)
        {
            camera_in.targetTexture.Release();
        }
        camera_in.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraMat_in.mainTexture = camera_in.targetTexture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
