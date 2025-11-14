using System;
using TMPro;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Hud : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private TextMeshProUGUI loseText;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject mainMenuButton;
    [SerializeField] private GameObject payOffMenu;
    [SerializeField] float remainingTime = 600f;

    [SerializeField] private TMP_Text boneCounterText;
    [SerializeField] private TMP_Text coinCounterText;

    private Action playerHasDied = null;
    private Enemy chosenEnemy;

    public static Hud instance;
    private void Awake()
    {
        // singelton pattern implementation
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        Player.PlayerHasDied += ShowTextLoseMessage;
    }
    private void Start()
    {
        GameManager.instance.SetHud(this);
    }
    private void Update()
    {
        UpdateTimer();
    }

    private void OnDestroy()
    {
        Player.PlayerHasDied -= ShowTextLoseMessage;
    }

    private void UpdateTimer()
    {
        if (remainingTime >= 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else
        {
            if (remainingTime != -1000f)
            {
                remainingTime = -1000f;
                timerText.text = string.Format("{0:00}:{1:00}", 0, 0);
                ShowTextLoseMessage();
                return;
            }
            else { return; }
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void SetRemainingTime(float time)
    {
        remainingTime = time;
    }

    public float GetRemainingTime()
    {
        return remainingTime;
    }
    public void UpdateAllCollectable(int boneCount, int coinCount, int boneCountAll, int coinCountAll)
    {
        boneCounterText.text = "Bone " + boneCount.ToString() + " / " + boneCountAll.ToString();
        coinCounterText.text = "Coin " + coinCount.ToString() + " / " + coinCountAll.ToString();
    }

    // Hide/Show Texts
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

    public void HidePayOffMenu()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().InputLvlZero();
        payOffMenu.gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void ShowPayOffIntecractionMenu(Enemy enemy)
    {
        chosenEnemy = enemy;
        payOffMenu.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void PayOffGuard()
    {
        GameManager.instance.UseCollectable(CollectableType.Coin);
        chosenEnemy.GetPaidOff();
        Debug.Log("pay off interaction");
    }

    public void TurnOffEnemyVision()
    {
        chosenEnemy.GetComponent<EnemyVisionCheck>().EndSearch();
    }

    public void TurnOnEnemyVision()
    {
        chosenEnemy.GetComponent<EnemyVisionCheck>().StartSearch();
    }
}
