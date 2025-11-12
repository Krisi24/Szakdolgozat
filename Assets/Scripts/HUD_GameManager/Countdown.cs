using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private TextMeshProUGUI loseText;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject mainMenuButton;
    [SerializeField] float remainingTime = 60f;

    [SerializeField] private TMP_Text collectableCounterText;
    [SerializeField] private TMP_Text collectableCounterText2;
    private int collectableCounter = 0;
    private int collectableCounter2 = 0;
    private int collectableCollectedCounter = 0;
    private int collectableCollectedCounter2 = 0;
    [SerializeField] private string collectableName;

    private void Awake()
    {
        if (collectableCounterText == null)
        {
            Debug.LogWarning("CollectableCounter UI element is not set in the HUD inspector!");
        }
        if (collectableCounterText2 == null)
        {
            Debug.LogWarning("CollectableCounter 2 UI element is not set in the HUD inspector!");
        }

        Player.PlayerHasDied += ShowTextLoseMessage;
        OpenDoor.ShowToolTip += ShowInteractionText;
        OpenDoor.HideToolTip += HideInteractionText;
    }

    private void ShowTextLoseMessage()
    {
        loseText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        mainMenuButton.gameObject.SetActive(true);
        timerText.color = Color.red;
        Time.timeScale = 0;
    }

    private void ShowInteractionText()
    {
        interactionText.gameObject.SetActive(true);
    }

    private void HideInteractionText()
    {
        interactionText.gameObject.SetActive(false);
    }

    private void IncrementCollectableCounter()
    {
        collectableCounter++;
        if(collectableCounterText == null)
        {
            Debug.LogWarning("CollectableCounter UI element is not set in the HUD inspector!");
            return;
        }
        collectableCounterText.text = collectableName + " " + collectableCounter.ToString() + " / 0";
    }

    private void IncrementCollectableCounterCollected()
    {
        collectableCollectedCounter++;
        if (collectableCounterText == null)
        {
            Debug.LogWarning("CollectableCounter UI element is not set in the HUD inspector!");
            return;
        }
        collectableCounterText.text = collectableName + " " + collectableCounter.ToString() + " / " + collectableCollectedCounter.ToString();
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (remainingTime >= 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else {
            if (remainingTime != -1000f)
            {
                remainingTime = -1000f;
                timerText.text = string.Format("{0:00}:{1:00}", 0, 0);
                ShowTextLoseMessage();
                return;
            }
            else{return;}
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void OnDestroy()
    {
        Player.PlayerHasDied -= ShowTextLoseMessage;
        OpenDoor.ShowToolTip -= ShowInteractionText;
        OpenDoor.HideToolTip -= HideInteractionText;
    }
}
