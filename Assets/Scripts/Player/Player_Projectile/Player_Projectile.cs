using UnityEngine;

public class Player_Projectile : MonoBehaviour
{
    //*-------------------------------------------------------------------------------------------------------*\\
   
    #region Inspector Tab

    [Header("References ---------------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private Rigidbody2D Projectile_Rigidbody;
    [SerializeField] private Collider2D Projectile_Collider;
    [Space]
    [Header("Projectile_Settings ------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private LayerMask Enemy_Layer_Mask;
    [Space]
    [Header("Debug --------------------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private bool Show_Debug_Gizmos = false;

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Private Variables

    private Player_Attack_System Player_Attack_System;
    private float Current_Lifetime;
    private float Max_Lifetime;
    private float Damage;
    private bool Is_Active;

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Unity Lifecycle

    void Awake()
    {
        Projectile_Rigidbody ??= GetComponent<Rigidbody2D>();
        Projectile_Collider ??= GetComponent<Collider2D>();
    }

    void Update()
    {
        if(!Is_Active) return;

        Current_Lifetime -= Time.deltaTime;
        if(Current_Lifetime <= 0) Deactivate_Projectile();
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(!Is_Active) return;
        if(((1 << collision.gameObject.layer) & Enemy_Layer_Mask) == 0) return;

        Handle_Enemy_Hit(collision.collider);
        Deactivate_Projectile();
    }


    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Public Methods

    public void Initialize(Player_Attack_System attack_System, float damage)
    {
        Player_Attack_System = attack_System;
        Damage = damage;
    }

    public void Launch(Vector2 direction, float speed, float lifetime)
    {
        Is_Active = true;
        Current_Lifetime = Max_Lifetime = lifetime;

        Projectile_Rigidbody.linearVelocity = direction * speed;
        Projectile_Collider.enabled = true;
    }

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Private Methods

    private void Handle_Enemy_Hit(Collider2D enemy)
    {
        Debug.Log("Enemy_Vuruldu");
        //enemy.GetComponent<Enemy_Health>()?.Take_Damage(Damage);
    }

    private void Deactivate_Projectile()
    {
        Is_Active = false;
        Projectile_Rigidbody.linearVelocity = Vector2.zero;
        Player_Attack_System?.Return_Projectile_To_Pool(this);
    }

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Debug

    void OnDrawGizmos()
    {
        if(!Show_Debug_Gizmos || !Is_Active) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Projectile_Rigidbody.linearVelocity.normalized * 0.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.2f * (Current_Lifetime / Max_Lifetime));
    }

    #endregion
    
    //*-------------------------------------------------------------------------------------------------------*\\
}