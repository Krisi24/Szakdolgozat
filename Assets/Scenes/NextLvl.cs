using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLvl : MonoBehaviour
{
    [SerializeField] private int sceneNumber = 1;

    private void OnTriggerEnter(Collider collision)
    {
        SceneManager.LoadSceneAsync("Level " + sceneNumber.ToString());
    }
}
