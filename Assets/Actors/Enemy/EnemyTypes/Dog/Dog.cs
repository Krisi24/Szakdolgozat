using UnityEngine;

public class Dog : MonoBehaviour
{
    private Enemy enemy;
    void Start()
    {
        this.enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Distract(GameObject distrtactionObject)
    {
        gameObject.GetComponent<EnemyVisionCheck>().enabled = false;
        enemy.GetDistracted(distrtactionObject); 
    }


}
