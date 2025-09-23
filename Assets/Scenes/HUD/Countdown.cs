using UnityEngine;
using TMPro;

public class Countdown : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI interactionText;
    [SerializeField] TextMeshProUGUI loseText;
    [SerializeField] GameObject restartButton;
    [SerializeField] GameObject mainMenuButton;
    [SerializeField] float remainingTime = 60f;

    private void Awake()
    {
        Player.PlayerHasDied += ShowTextLoseMessage;
        OpenDoor.ShowToolTip += ShowInteractionText;
        OpenDoor.HideToolTip += HideInteractionText;
    }

    private void ShowTextLoseMessage()
    {
        loseText.gameObject.SetActive(true);
    }

    private void ShowInteractionText()
    {
        interactionText.gameObject.SetActive(true);
    }

    private void HideInteractionText()
    {
        interactionText.gameObject.SetActive(false);
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
                timerText.color = Color.red;
                timerText.text = string.Format("{0:00}:{1:00}", 0, 0);
                loseText.gameObject.SetActive(true);
                restartButton.gameObject.SetActive(true);
                mainMenuButton.gameObject.SetActive(true);
                Time.timeScale = 0;
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
