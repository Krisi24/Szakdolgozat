using UnityEngine;

public class EnemyManager : SingletonPattern
{
    private GameObject[] enemies;


    private void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }


    void Update()
    {
        
    }
}
