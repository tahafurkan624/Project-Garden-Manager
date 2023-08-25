#if UNITY_EDITOR
using System.IO;
using _Main._Scripts.Utilities;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.Events;
using zz_HelmetMaster.Recorder.Resources;
using System.Collections;
#endif

namespace HelmetMaster.Recorder.Runtime
{
#if UNITY_EDITOR
    public class ScreenRecorder : Singleton<ScreenRecorder>
    {
        [Header("Settings")] [SerializeField] private RecorderData recorderData;
        [SerializeField] private bool captureUI = true;
        [SerializeField] private bool shouldCreateAHand = true;
        [SerializeField] private bool captureAudio;

        [Header("Image Settings")]
        [SerializeField]
        private ImageRecorderSettings.ImageRecorderOutputFormat targetImageOutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.JPEG;
        
        [Header("Video Settings")] [SerializeField, Tooltip("Select MOV format for higher video quality")]
        private VideoOutputFormat targetVideoOutputFormat = VideoOutputFormat.MP4;

        [SerializeField] private VideoQuality targetVideoQuality = VideoQuality.Low;
        [SerializeField] private string targetVideoSuffix;
        

        [SerializeField]
        private VideoOutputResolutions targetVideoResolution = VideoOutputResolutions._1280x1600_Creative;

        public bool IsCustomResolution => targetVideoResolution == VideoOutputResolutions.Custom;

        private ResolutionData _resolutionData;
        public ResolutionData ResolutionData
        {
            get
            {
                if (UnityEngine.Resources.Load<ResolutionData>("ResolutionData") != null)
                {
                    _resolutionData = UnityEngine.Resources.Load<ResolutionData>("ResolutionData");
                }
                else
                {
                    Debug.LogError($"{GetType().Name} -> ResolutionData.asset file not found in Resources folder!");
                }

                return _resolutionData;
            }
        }
        
        [SerializeField, Tooltip("Start video recording as soon as game starts.")]
        private bool autoStartVideoRecording = false;

        private enum VideoQuality
        {
            Low,
            Medium,
            High
        }

        private enum VideoOutputFormat
        {
            MP4,
            MOV
        }

        private enum VideoOutputResolutions
        {
            _886x1920_6_5,
            _1080x1920_5_5,
            _1200x1600_12_9,
            _1280x1600_Creative,
            Custom,
        }

        private RecorderController recorderController;
        private RecorderControllerSettings recorderControllerSettings;
        private ImageRecorderSettings imageRecorderSettings;
        private MovieRecorderSettings movieRecorderSettings;
        private string imageOutputFolder;
        private string videoOutputFolder;
        private string lastRecordedImageName = "null_image";
        private GameObject hand;

        private bool isImageRecording;
        private bool isVideoRecording;
        public bool IsRecording { get; private set; }
        public bool IsDeviceSimulatorRunning { get; private set; }

        public UnityAction OnRecordingStateChanged;

        private void Reset()
        {
            if (UnityEngine.Resources.Load<RecorderData>("RecorderData") != null)
            {
                recorderData = UnityEngine.Resources.Load<RecorderData>("RecorderData");
            }
            else
            {
                Debug.LogError($"{GetType().Name} -> RecorderData.asset file not found in Resources folder!");
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            IsRecording = false;
        }

        private void OnEnable()
        {
            OnRecordingStateChanged += CheckRecordingState;
        }

        private void Start()
        {
            CheckDeviceSimulatorState();
            
            if (autoStartVideoRecording)
            {
                if (!IsDeviceSimulatorRunning)
                {
                    TakeVideoCapture();
                }
                else
                {
                    Debug.LogError(
                        $"{GetType().Name} -> Unable autostart video recording. Please switch to GameView. Recorder can not work with Simulator.");
                }
            }
        }

        private void Update()
        {
            CheckDeviceSimulatorState(); //TODO: might not be required to check every frame

            if (!IsDeviceSimulatorRunning)
            {
                if ((Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) &&
                    !isImageRecording && !isVideoRecording)
                {
                    StartCoroutine(TakeAllScreenShotsAsync());
                }
                else if ((Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) &&
                         isImageRecording)
                {
                    Debug.LogError(
                        $"{GetType().Name} -> Recorder is busy! Please wait until recording process completed.");
                }

                if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
                {
                    TakeVideoCapture();
                }
                if((Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Alpha8)) && isVideoRecording)
                {
                    hand.SetActive(!hand.activeSelf);
                }
                
                if (isVideoRecording && hand.activeSelf) FollowHand();
            }
            else
            {
                Debug.LogError($"{GetType().Name} -> Please switch to GameView. Recorder can not work with Simulator.");
            }
        }

        private void OnDisable()
        {
            OnRecordingStateChanged -= CheckRecordingState;
        }

        [MenuItem("Master/Screen Recorder", false, 29)]
        public static void CreateNestScreenRecorder()
        {
            if (FindObjectOfType<ScreenRecorder>() == null)
            {
                GameObject nestRecorder = new GameObject("ScreenRecorder", typeof(ScreenRecorder));
            }
            else
            {
                Debug.LogError($"ScreenRecorder -> Recorder is already exist in hierarchy!");
            }
        }

        #region Video Recording

        private void InitializeVideoRecorderController()
        {
            recorderControllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            recorderController = new RecorderController(recorderControllerSettings);

            videoOutputFolder = Path.Combine(Application.dataPath, "..", "Recordings/Videos");

            movieRecorderSettings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            movieRecorderSettings.name = "Video Recorder";
            movieRecorderSettings.Enabled = true;
            //movieRecorderSettings.Take = GetVideoFileCount();
            movieRecorderSettings.Take = recorderData.videoTakeCount;

            // This performs a MP4 recording
            if (targetVideoOutputFormat == VideoOutputFormat.MP4)
            {
                movieRecorderSettings.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;
            }
            else if (targetVideoOutputFormat == VideoOutputFormat.MOV)
            {
                movieRecorderSettings.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MOV;
            }

            movieRecorderSettings.VideoBitRateMode = GetTargetVideoBitrateMode(movieRecorderSettings);

            if (targetVideoResolution == VideoOutputResolutions._886x1920_6_5)
            {
                movieRecorderSettings.ImageInputSettings = new GameViewInputSettings
                {
                    OutputWidth = 886,
                    OutputHeight = 1920
                };
            }
            else if (targetVideoResolution == VideoOutputResolutions._1080x1920_5_5)
            {
                movieRecorderSettings.ImageInputSettings = new GameViewInputSettings
                {
                    OutputWidth = 1080,
                    OutputHeight = 1920
                };
            }
            else if (targetVideoResolution == VideoOutputResolutions._1200x1600_12_9)
            {
                movieRecorderSettings.ImageInputSettings = new GameViewInputSettings
                {
                    OutputWidth = 1200,
                    OutputHeight = 1600
                };
            }
            else if (targetVideoResolution == VideoOutputResolutions._1280x1600_Creative)
            {
                movieRecorderSettings.ImageInputSettings = new GameViewInputSettings
                {
                    OutputWidth = 1280,
                    OutputHeight = 1600
                };
            }
            else if (targetVideoResolution == VideoOutputResolutions.Custom)
            {
                movieRecorderSettings.ImageInputSettings = new GameViewInputSettings
                {
                    OutputWidth = ResolutionData.outputWidth,
                    OutputHeight = ResolutionData.outputHeight
                };
            }

            // movieRecorderSettings.ImageInputSettings = new GameViewInputSettings
            // {
            //     OutputWidth = 1280,
            //     OutputHeight = 1600
            // };

            movieRecorderSettings.AudioInputSettings.PreserveAudio = captureAudio;

            movieRecorderSettings.OutputFile = videoOutputFolder + "/" +
                                               $"{DefaultWildcard.Project}_{DefaultWildcard.Resolution}_{DefaultWildcard.Take}_{targetVideoSuffix}";

            // movieRecorderSettings.OutputFile = Path.Combine(videoOutputFolder,
            //     $"{DefaultWildcard.Project}_{DefaultWildcard.Resolution}_{DefaultWildcard.Take}"); ;

            // Setup Recording
            recorderControllerSettings.AddRecorderSettings(movieRecorderSettings);
            recorderControllerSettings.SetRecordModeToManual();
            recorderControllerSettings.FrameRate = 30.0f;
            SetUIVisibility(captureUI); //Decide should be hidden or not

            RecorderOptions.VerboseMode = false;
            recorderController.PrepareRecording();
            recorderController.StartRecording();
            isVideoRecording = true;
            recorderData.videoTakeCount += 1;
            EditorUtility.SetDirty(recorderData);
            OnRecordingStateChanged.Invoke();
            CreateHand();

            Debug.Log($"{GetType().Name} -> Video recording started!");
        }

        private void TakeVideoCapture()
        {
            if (!isImageRecording && !isVideoRecording)
            {
                InitializeVideoRecorderController();
            }
            else
            {
                recorderController.StopRecording();
                Destroy(hand);
                SetUIVisibility(true); //Show UI
                isVideoRecording = false;
                OnRecordingStateChanged.Invoke();

                Debug.Log($"{GetType().Name} -> Video recording finished!");
            }
        }

        private int GetVideoFileCount()
        {
            var directoryInfo = new DirectoryInfo(videoOutputFolder);
            int fileCount;

            if (!Directory.Exists(videoOutputFolder))
            {
                fileCount = 0;
            }
            else
            {
                var files = directoryInfo.GetFiles("*.mp4");
                fileCount = files.Length;
            }

            return fileCount;
        }

        #endregion

        #region Image Recording

        private IEnumerator TakeAllScreenShotsAsync()
        {
            isImageRecording = true;
            OnRecordingStateChanged.Invoke();

            SetUIVisibility(captureUI); //Decide should be hidden or not

            SetImageRecorderSettings(1242, 2208);
            yield return StartCoroutine(TakeScreenshot());

            SetImageRecorderSettings(1242, 2688);
            yield return StartCoroutine(TakeScreenshot());

            SetImageRecorderSettings(2048, 2732);
            yield return StartCoroutine(TakeScreenshot());

            SetUIVisibility(true); //make sure ui is visible after screenshots taken

            recorderData.screenshotTakeCount += 1;
            EditorUtility.SetDirty(recorderData);


            isImageRecording = false;
            OnRecordingStateChanged.Invoke();
        }

        private void InitializeRecorderController()
        {
            recorderControllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            recorderController = new RecorderController(recorderControllerSettings);
            
            imageOutputFolder = Path.Combine(Application.dataPath, "..", "Recordings/Screenshots");
        }

        private void SetImageRecorderSettings(int imageWidth, int imageHeight)
        {
            InitializeRecorderController();

            imageRecorderSettings = ScriptableObject.CreateInstance<ImageRecorderSettings>();
            imageRecorderSettings.name = "Image Recorder";
            imageRecorderSettings.Enabled = true;
            imageRecorderSettings.OutputFormat = targetImageOutputFormat;
            imageRecorderSettings.CaptureAlpha = false;
            //imageRecorderSettings.Take = GetImageFileCount() / 3;
            imageRecorderSettings.Take = recorderData.screenshotTakeCount;

            imageRecorderSettings.OutputFile = imageOutputFolder + "/" + DefaultWildcard.Resolution + "/" + 
                                               $"{DefaultWildcard.Project}_{DefaultWildcard.Resolution}_{DefaultWildcard.Take}";

            // imageRecorderSettings.OutputFile = Path.Combine(imageOutputFolder,
            //     $"{DefaultWildcard.Project}_{DefaultWildcard.Resolution}_{DefaultWildcard.Take}");

            if (imageRecorderSettings.OutputFormat == ImageRecorderSettings.ImageRecorderOutputFormat.JPEG)
            {
                imageRecorderSettings.imageInputSettings = new GameViewInputSettings
                {
                    OutputWidth = imageWidth,
                    OutputHeight = imageHeight,
                };  
            }
            else
            {
                imageRecorderSettings.CaptureAlpha = true;
                imageRecorderSettings.imageInputSettings = new CameraInputSettings();
                imageRecorderSettings.imageInputSettings.OutputHeight = imageHeight;
                imageRecorderSettings.imageInputSettings.OutputWidth = imageWidth;
                imageRecorderSettings.imageInputSettings.RecordTransparency = true;
            }
            

            lastRecordedImageName =
                $"{imageWidth}x{imageHeight}_{imageRecorderSettings.Take}.{imageRecorderSettings.OutputFormat}";
        }
        
        private IEnumerator TakeScreenshot()
        {
            recorderControllerSettings.RemoveRecorder(imageRecorderSettings); //null?
            recorderControllerSettings.AddRecorderSettings(imageRecorderSettings);
            recorderControllerSettings.SetRecordModeToSingleFrame(0);

            recorderController.PrepareRecording();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            recorderController.StartRecording();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();


            Debug.Log($"{GetType().Name} -> {lastRecordedImageName}");
        }

        private int GetImageFileCount()
        {
            var directoryInfo = new DirectoryInfo(imageOutputFolder);
            int fileCount;

            if (!Directory.Exists(imageOutputFolder))
            {
                fileCount = 0;
            }
            else
            {
                var files = directoryInfo.GetFiles("*.jpg");
                fileCount = files.Length;
            }

            return fileCount;
        }

        #endregion

        private void SetUIVisibility(bool isEnabled)
        {
            var canvasList = FindObjectsOfType<Canvas>();

            for(int i = 0; i < canvasList.Length; i++)
            {
                var canvas = canvasList[i];
                //canvas.enabled = isEnabled;

                if(canvas.gameObject.GetComponent<CanvasGroup>() == null) canvas.gameObject.AddComponent<CanvasGroup>();

                canvas.gameObject.GetComponent<CanvasGroup>().alpha = isEnabled ? 1 : 0;
            }
        }

        private VideoBitrateMode GetTargetVideoBitrateMode(MovieRecorderSettings movieRecorderSettings)
        {
            switch(targetVideoQuality)
            {
                case VideoQuality.Low: return VideoBitrateMode.Low;
                case VideoQuality.Medium: return VideoBitrateMode.Medium;
                case VideoQuality.High: return VideoBitrateMode.High;
                default:
                    Debug.Log($"{GetType().Name} -> Unable to detect target video quality, 'Low' settled as default.");
                    return VideoBitrateMode.Low;
            }
        }

        private void CheckRecordingState()
        {
            if (isImageRecording || isVideoRecording)
            {
                IsRecording = true;
                //Debug.Log($"{GetType().Name} -> IsRecording {IsRecording} ");
            }
            else if (!isImageRecording && !isVideoRecording)
            {
                IsRecording = false;
                //Debug.Log($"{GetType().Name} -> IsRecording {IsRecording} ");
            }
        }

        private void CheckDeviceSimulatorState()
        {
            if(!Application.isPlaying) return;
            //IsDeviceSimulatorRunning = Application.isMobilePlatform ? true : false;
            IsDeviceSimulatorRunning = Application.isMobilePlatform 
                                       && EditorWindow.focusedWindow != null 
                                       && EditorWindow.focusedWindow.ToString() == " (Unity.DeviceSimulator.SimulatorWindow)";
        }

        private void CreateHand()
        {
            //hand = PrefabUtility.LoadPrefabContents("Assets/zz_HelmetMaster/Recorder/Prefabs/HandCreative.prefab");
            //hand = Instantiate(hand, UIManager.Instance.InGameUI.transform);
            //hand.SetActive(shouldCreateAHand);
        }

        private void FollowHand()
        {
            hand.transform.position = Input.mousePosition;
        }
    }
#endif
}