using UnityEngine;

public class Dog : MonoBehaviour
{
    private Enemy enemy;
    void Start()
    {
        this.enemy = GetComponent<Enemy>();
    }

    public void Distract(GameObject distrtactionObject)
    {
        gameObject.GetComponent<EnemyVisionCheck>().enabled = false;
        enemy.GetDistracted(distrtactionObject); 
    }
}
