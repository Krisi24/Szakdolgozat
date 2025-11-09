using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // time
    public float timeLimit = 600f;
    private float remainingTime;
    private float remainingTimeFromLastLvl;
    private Hud Hud;

    // collectables
    private int boneCountAll = 0;
    private int coinCountAll = 0;
    private int boneCountAvaiable = 0;
    private int coinCountAvaiable = 0;

    private int boneCountAllPreviousLevel = 0;
    private int coinCountAllPreviousLevel = 0;
    private int boneCountAvaiablePreviousLevel = 0;
    private int coinCountAvaiablePreviousLevel = 0;

    // singelton
    public static GameManager instance;
    private void Awake()
    {
        // singelton pattern implementation
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //FindCurrentHud();
        if (Hud != null)
        {
            Hud.SetRemainingTime(remainingTimeFromLastLvl);
            Hud.UpdateAllCollectable(boneCountAvaiable, coinCountAvaiable, boneCountAll, coinCountAll);
        } else
        {
            Debug.Log("Unable to find Hud OnSceneLoaded()!");
        }
    }

    public void ResetCurrentLevel()
    {
        remainingTime = remainingTimeFromLastLvl;

        boneCountAll = boneCountAllPreviousLevel;
        coinCountAll = coinCountAllPreviousLevel;
        boneCountAvaiable = boneCountAvaiablePreviousLevel;
        coinCountAvaiable = coinCountAvaiablePreviousLevel;
    }

    public void SaveLevelData()
    {
        if(Hud != null)
        {
            remainingTimeFromLastLvl = Hud.GetRemainingTime();
        } else
        {
            Debug.Log("Couldn't save remaining time, because Hud reference is null!");
        }
        boneCountAllPreviousLevel = boneCountAll;
        coinCountAllPreviousLevel = coinCountAll;
        boneCountAvaiablePreviousLevel = boneCountAvaiable;
        coinCountAvaiablePreviousLevel = coinCountAvaiable;
    }

    public void ResetGame()
    {
        remainingTimeFromLastLvl = remainingTime = timeLimit;

        boneCountAllPreviousLevel = 0;
        coinCountAllPreviousLevel = 0;
        boneCountAvaiablePreviousLevel = 0;
        coinCountAvaiablePreviousLevel = 0;
        boneCountAll = 0;
        coinCountAll = 0;
        boneCountAvaiable = 0;
        coinCountAvaiable = 0;
    }

    // Collectable counters
    public void RegisterCollectable(CollectableType c)
    {
        //FindCurrentHud();
        switch (c)
        {
            case CollectableType.Bone: boneCountAll++;
                break;
            case CollectableType.Coin: coinCountAll++;
                break;
            default:  Debug.Log("Undefined Collectable Type tried to register!"); break;
        }
        if (Hud != null)
        {
            Hud.UpdateAllCollectable(boneCountAvaiable, coinCountAvaiable, boneCountAll, coinCountAll);
        } else
        {
            Debug.Log("Hud is not set -> unable to update Collectable Counter!");
        }
    }

    public void UnregisterCollectable(CollectableType c)
    {
        switch (c)
        {
            case CollectableType.Bone:
                boneCountAvaiable++;
                break;
            case CollectableType.Coin:
                coinCountAvaiable++;
                break;
            default: Debug.Log("Undefined Collectable Type tried to unregister!"); break;
        }
        if (Hud != null)
        {
            Hud.UpdateAllCollectable(boneCountAvaiable, coinCountAvaiable, boneCountAll, coinCountAll);
        }
        else
        {
            Debug.Log("Hud is not set -> unable to update Collectable Counter!");
        }
    }

    private void FindCurrentHud()
    {
        if (Hud == null)
        {
            Hud = Hud.instance;
        } else
        {
            Debug.Log("Unable to find Hud element in the scene!");
        }
    }

    public void SetHud(Hud newHud)
    {
        Hud = newHud;
        Hud.UpdateAllCollectable(boneCountAvaiable, coinCountAvaiable, boneCountAll, coinCountAll);
    }
}
