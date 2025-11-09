using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.InputSystem;

public class ScreenshotUtility : MonoBehaviour
{
    public static ScreenshotUtility screenShotUtility;

    #region Public Variables
    public bool runOnlyInEditor = true;
    public string m_ScreenshotKey = "k";
    public int m_ScaleFactor = 1;
    public bool includeImageSizeInFilename = true;
    #endregion

    [SerializeField] private int m_ImageCount = 0;

    private const string ImageCntKey = "IMAGE_CNT";

    void Awake()
    {
        if (screenShotUtility != null)
        {
            Destroy(this.gameObject);
        }
        else if (runOnlyInEditor && !Application.isEditor)
        {
            Destroy(this.gameObject);
        }
        else
        {
            screenShotUtility = this.GetComponent<ScreenshotUtility>();

            DontDestroyOnLoad(gameObject);

            m_ImageCount = PlayerPrefs.GetInt(ImageCntKey);

            if (!Directory.Exists("Screenshots"))
            {
                Directory.CreateDirectory("Screenshots");
            }
        }
    }

    void Update()
    {
        if (Keyboard.current.FindKeyOnCurrentKeyboardLayout(m_ScreenshotKey).wasPressedThisFrame)
        {
            TakeScreenshot();
        }
    }

    public void ResetCounter()
    {
        m_ImageCount = 0;
        PlayerPrefs.SetInt(ImageCntKey, m_ImageCount);
    }

    public void TakeScreenshot()
    {
        PlayerPrefs.SetInt(ImageCntKey, ++m_ImageCount);

        int width = Screen.width * m_ScaleFactor;
        int height = Screen.height * m_ScaleFactor;

        string pathname = "Screenshots/Screenshot_";
        if (includeImageSizeInFilename)
        {
            pathname += width + "x" + height + "_";
        }
        pathname += m_ImageCount + ".png";

        ScreenCapture.CaptureScreenshot(pathname, m_ScaleFactor);
        Debug.Log("Screenshot captured at " + pathname);
    }
}
