using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLvl : MonoBehaviour
{   

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider collision)
    {
        SceneManager.LoadSceneAsync("Level 2");
    }
}
