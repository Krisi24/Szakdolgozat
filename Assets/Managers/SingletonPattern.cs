using UnityEngine;

public abstract class SingletonPattern : MonoBehaviour
{

    public static SingletonPattern instance;
    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        } else
        {
            instance = this;
        }
    }
}
