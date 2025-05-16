using System.IO;
using UnityEngine;
using Unity.MLAgents;

public class CaptureScreenShot : MonoBehaviour
{
        private int takes = 0;
        public bool recordEnable;
        private string recordingFolderName;
        private MainConfig mainConfig;
        private EnvironmentParameters m_ResetParams;
        private string mediaOutputFolder;

        public void Awake()
        {
                Academy.Instance.OnEnvironmentReset += SetParameters;
        }

        public void Initialize()
        {
                m_ResetParams = Academy.Instance.EnvironmentParameters;

                ConfigLoader configLoader = FindObjectOfType<ConfigLoader>();
                if (configLoader != null)
                {
                        mainConfig = configLoader.mainConfig;
                        if (mainConfig != null)
                        {
                                if (!string.IsNullOrEmpty(mainConfig.recordingScreen.recordingFolderName))
                                {
                                        recordingFolderName = mainConfig.recordingScreen.recordingFolderName;
                                        Debug.Log("mainConfig.recordingScreen.recordingFolderName: " + mainConfig.recordingScreen.recordingFolderName);
                                }
                                else
                                {
                                        recordingFolderName = "SampleRecordings";
                                        Debug.Log("Use default recordingFolderName: " + recordingFolderName);
                                }

                                recordEnable = mainConfig.recordingScreen.recordEnable;
                                Debug.Log("Use mainConfig.recordingScreen.recordEnable: " + recordEnable);
                        }
                        else
                        {
                                Debug.LogError("mainConfig is null.");
                        }
                }
                else
                {
                        Debug.LogError("ConfigLoader not found.");
                }

                if (recordEnable)
                {
                        CreateRecordDirectory();
                }
        }

        void SetParameters()
        {
        }
        public void CreateRecordDirectory()
        {
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
                string appRoot = Directory.GetParent(Application.dataPath).Parent.FullName;
                mediaOutputFolder = Path.Combine(appRoot, "Recordings", recordingFolderName);
#else
                mediaOutputFolder = Path.Combine(Application.dataPath, "..", "Recordings", recordingFolderName);
#endif
                Debug.Log("mediaOutputFolder: " + mediaOutputFolder);

                DirectoryInfo directoryInfo = new DirectoryInfo(mediaOutputFolder);
                if (!directoryInfo.Exists)
                {
                        directoryInfo.Create();
                }
        }

        // This will be used in the InteroceptiveAgent.OnActionReceived() method
        public void CaptureImage()
        {
                takes = takes + 1;

                string s_takes = takes.ToString();
                ScreenCapture.CaptureScreenshot(Path.Combine(mediaOutputFolder, "record_") + s_takes.PadLeft(5, '0') + ".png");
        }

}
