using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    private Animator anim;
    [field: SerializeField] public float cooldownTime = 0.7f;
    [field: SerializeField] private float nextfireTime = 0f;
    [field: SerializeField] private float noOfClicks = 0;
    [field: SerializeField] private float damage = 40f;
    public GameObject EnemyTarget { get; set; }

    public Transform AttackBox;
    public Vector3 attackRange = new Vector3(2f, 1.75f, 1.25f);
    public Quaternion enemyLayers = new Quaternion();

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(nextfireTime < Time.time )
        {
            anim.SetBool("isBasicAttack", false);
            if (Input.GetMouseButtonDown(0))
            {
                OnClick();
            }
        }   
    }


    private void OnDrawGizmosSelected()
    {
        if(AttackBox == null)
        {
            return;
        }
        Gizmos.DrawWireCube(AttackBox.position, attackRange);
    }

    void OnClick()
    {
        if (anim.GetBool("isBasicAttack") != true)
        {
            anim.SetBool("isBasicAttack", true);
            Attack();
            nextfireTime = Time.time + cooldownTime;
        }
         
    }
    void Attack()
    {
        Collider[] hitEnemies = Physics.OverlapBox(AttackBox.position ,attackRange ,enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("hit enemy: " + enemy.name);
            if (!enemy.isTrigger)
            {
                Enemy enemyComponent = enemy.GetComponent<Enemy>();  // Ellenõrizzük, hogy van-e Enemy komponens
                if (enemyComponent != null)
                {
                    enemyComponent.Damage(damage);
                }
            }
        }
    }
}
