using UnityEngine;

public class SetActiveOnTrigger : MonoBehaviour
{
    public GameObject activateAble;

    private void OnTriggerEnter(Collider collision)
    {
        if (activateAble != null)
        {
            activateAble.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
