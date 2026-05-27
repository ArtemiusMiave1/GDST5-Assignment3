using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

namespace PostSeamBlending
{
    public class SeamBlendingScript : MonoBehaviour
    {
        private Camera _objectIdCamera;
        private RenderTexture _objectIdTexture;
        private RenderTexture _sceneColorTexture;


        [Header("Materials")] [Tooltip("Material with the PostSeamBlendingMaterial shader")]
        public Material PostBlendMaterial;

        [Tooltip("The Object ID Shader | This should be in the package folder")]
        public Shader ObjectIdShader;

        [Header("Settings")] public LayerMask LayersToBlend;


        private void Start()
        {
            if (!Camera.main)
            {
                Debug.LogWarning("Seam blending script requires a main camera", this);
                Destroy(this);
                return;
            }

            Camera cam;
            if (!(cam = GetComponent<Camera>()))
            {
                if (cam != Camera.main)
                {
                    Debug.LogWarning("Please put Seam Blending script on the GameObject with the Main Camera", this);
                    Destroy(this);
                    return;
                }
            }


            //Make sure depth is enabled
            Camera.main.depthTextureMode |= DepthTextureMode.Depth;

            //Add command that copies scene color
            _sceneColorTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
            _sceneColorTexture.Create();

            CommandBuffer cb = new();
            cb.name = "Capture Scene Color";
            cb.Blit(BuiltinRenderTextureType.CameraTarget, _sceneColorTexture);
            Camera.main.AddCommandBuffer(CameraEvent.AfterForwardAlpha, cb);
            Shader.SetGlobalTexture("_CameraOpaqueTexture", _sceneColorTexture);

            //Add camera for use with the ID Pass
            GameObject camObj = new("OverrideCamera");
            camObj.transform.SetParent(transform);
            _objectIdCamera = camObj.AddComponent<Camera>();
            UpdateCamera();
        }

        //Updates the settings of the seamblend camera from the main camera
        public void UpdateCamera()
        {
            _objectIdCamera.CopyFrom(Camera.main);
            _objectIdCamera.cullingMask = LayersToBlend;
            _objectIdCamera.targetTexture = _objectIdTexture;
            _objectIdCamera.enabled = false;
        }

        //Use OnRenderImage in Built in RP happens after transparency and UI, in URP we do this before transparency to make sure its handled correctly. However, as far as I know there is no equivalent in BiRP.
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            int width = Screen.width;
            int height = Screen.height;
            //First object ID Pass
            // Create/Update renderTextures if necessary
            if (_objectIdTexture == null || _objectIdTexture.width != width)
            {
                if (_objectIdTexture != null)
                    _objectIdTexture.Release();

                RenderTextureDescriptor desc = new(width, height, RenderTextureFormat.ARGB32, 32)
                {
                    autoGenerateMips = false,
                    useMipMap = false,
                    sRGB = true,
                    msaaSamples = 1,
                    depthBufferBits = 24, // Possibly not needed
                    colorFormat = RenderTextureFormat.ARGB32,
                    enableRandomWrite = true // Also maybe not needed
                };
                _objectIdTexture = new RenderTexture(desc)
                {
                    name = "Object ID Texture"
                };
                _objectIdTexture.Create();
                _objectIdCamera.targetTexture = _objectIdTexture;
            }

            //Render from the id camera using the id+depth shader
            _objectIdCamera.RenderWithShader(ObjectIdShader, "RenderType");

            //Full screen pass
            Shader.SetGlobalTexture("_ObjectIDTexture", _objectIdTexture);
            Graphics.Blit(source, destination, PostBlendMaterial);
        }
    }
}