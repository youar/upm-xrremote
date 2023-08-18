// using System.Text;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.XR.ARFoundation;
// using UnityEngine.XR.ARSubsystems;

// namespace XRRemote
// {
//     /// <summary>
//     /// This component displays a picture-in-picture view of the environment depth texture, the human depth texture, or
//     /// the human stencil texture.
//     /// </summary>
//     public class DisplayDepthImage : MonoBehaviour
//     {
//         /// <summary>
//         /// The display mode for the texture widget. Values must match the UI dropdown.
//         /// </summary>
//         enum DisplayMode
//         {
//             EnvironmentDepthRaw = 0,
//             EnvironmentDepthSmooth = 1,
//             HumanDepth = 2,
//             HumanStencil = 3,
//         }

//         /// <summary>
//         /// Name of the max distance property in the shader.
//         /// </summary>
//         const string k_MaxDistanceName = "_MaxDistance";

//         /// <summary>
//         /// Name of the display rotation matrix in the shader.
//         /// </summary>
//         const string k_DisplayRotationPerFrameName = "_DisplayRotationPerFrame";

//         /// <summary>
//         /// The default texture aspect ratio.
//         /// </summary>
//         const float k_DefaultTextureAspectRadio = 1.0f;

//         /// <summary>
//         /// ID of the max distance  property in the shader.
//         /// </summary>
//         static readonly int k_MaxDistanceId = Shader.PropertyToID(k_MaxDistanceName);

//         /// <summary>
//         /// ID of the display rotation matrix in the shader.
//         /// </summary>
//         static readonly int k_DisplayRotationPerFrameId = Shader.PropertyToID(k_DisplayRotationPerFrameName);

//         /// <summary>
//         /// A string builder for construction of strings.
//         /// </summary>
//         readonly StringBuilder m_StringBuilder = new StringBuilder();

//         /// <summary>
//         /// The current screen orientation remembered so that we are only updating the raw image layout when it changes.
//         /// </summary>
//         ScreenOrientation m_CurrentScreenOrientation;

//         /// <summary>
//         /// The current texture aspect ratio remembered so that we can resize the raw image layout when it changes.
//         /// </summary>
//         float m_TextureAspectRatio = k_DefaultTextureAspectRadio;

//         /// <summary>
//         /// The mode indicating which texture to display.
//         /// </summary>
//         DisplayMode m_DisplayMode = DisplayMode.EnvironmentDepthRaw;

//         /// <summary>
//         /// The display rotation matrix for the shader.
//         /// </summary.
//         Matrix4x4 m_DisplayRotationMatrix = Matrix4x4.identity;

// #if UNITY_ANDROID
//         /// <summary>
//         /// A matrix to flip the Y coordinate for the Android platform.
//         /// </summary>
//         Matrix4x4 k_AndroidFlipYMatrix = Matrix4x4.identity;
// #endif // UNITY_ANDROID

//         /// <summary>
//         /// Get or set the <c>AROcclusionManager</c>.
//         /// </summary>
//         public AROcclusionManager occlusionManager
//         {
//             get => m_OcclusionManager;
//             set => m_OcclusionManager = value;
//         }

//         [SerializeField]
//         [Tooltip("The AROcclusionManager which will produce depth textures.")]
//         AROcclusionManager m_OcclusionManager;

//         /// <summary>
//         /// Get or set the <c>ARCameraManager</c>.
//         /// </summary>
//         public ARCameraManager cameraManager
//         {
//             get => m_CameraManager;
//             set => m_CameraManager = value;
//         }

//         [SerializeField]
//         [Tooltip("The ARCameraManager which will produce camera frame events.")]
//         ARCameraManager m_CameraManager;

//         /// <summary>
//         /// The UI RawImage used to display the image on screen.
//         /// </summary>
//         public RawImage rawImage
//         {
//             get => m_RawImage;
//             set => m_RawImage = value;
//         }

//         [SerializeField]
//         RawImage m_RawImage;

//         /// <summary>
//         /// The UI Text used to display information about the image on screen.
//         /// </summary>
//         public Text imageInfo
//         {
//             get => m_ImageInfo;
//             set => m_ImageInfo = value;
//         }

//         [SerializeField]
//         Text m_ImageInfo;

//         /// <summary>
//         /// The depth material for rendering depth textures.
//         /// </summary>
//         public Material depthMaterial
//         {
//             get => m_DepthMaterial;
//             set => m_DepthMaterial = value;
//         }

//         [SerializeField]
//         Material m_DepthMaterial;

//         /// <summary>
//         /// The stencil material for rendering stencil textures.
//         /// </summary>
//         public Material stencilMaterial
//         {
//             get => m_StencilMaterial;
//             set => m_StencilMaterial = value;
//         }

//         [SerializeField]
//         Material m_StencilMaterial;

//         /// <summary>
//         /// The max distance value for the shader when showing an environment depth texture.
//         /// </summary>
//         public float maxEnvironmentDistance
//         {
//             get => m_MaxEnvironmentDistance;
//             set => m_MaxEnvironmentDistance = value;
//         }

//         [SerializeField]
//         float m_MaxEnvironmentDistance = 8.0f;

//         /// <summary>
//         /// The max distance value for the shader when showing an human depth texture.
//         /// </summary>
//         public float maxHumanDistance
//         {
//             get => m_MaxHumanDistance;
//             set => m_MaxHumanDistance = value;
//         }

//         [SerializeField]
//         float m_MaxHumanDistance = 3.0f;
//     }
// }