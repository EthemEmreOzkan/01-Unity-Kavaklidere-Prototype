using UnityEngine;
using System.Collections.Generic;

public class Player_Attack_System : MonoBehaviour
{
    //*-------------------------------------------------------------------------------------------------------*\\
   
    #region Inspector Tab

    [Header("References ---------------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private Player_Data_SO Player_Data_SO;
    [SerializeField] private Transform Player_Wand_Transform;
    [SerializeField] private Transform Projectile_Spawn_Point;
    [SerializeField] private Transform Projectile_Parent;
    [SerializeField] private Player_Projectile Player_Projectile;
    [Space]
    [Header("Wand_Settings ------------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private Vector2 Wand_Offset_From_Player = new Vector2(0.5f, 0f);
    [SerializeField] private float Wand_Length = 1f;
    [SerializeField] private float Wand_Rotation_Speed = 20f;
    [Space]
    [Header("Attack_Settings ----------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private float Player_Fire_Rate = 0.3f;
    [SerializeField] private float Player_Projectile_Speed = 15f;
    [SerializeField] private float Player_Projectile_Lifetime = 3f;
    [SerializeField] private int Player_Initial_Pool_Size = 10;
    [Space]
    [Header("Debug --------------------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private bool Show_Debug_Gizmos = true;

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Private Variables

    private Camera Main_Camera;
    private Player_Movement Player_Movement;
    private Vector2 Mouse_World_Position;
    private float Last_Fire_Time;
    private List<Player_Projectile> Projectile_Pool;

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Unity Lifecycle

    void Awake()
    {
        Main_Camera = Camera.main;
        Player_Movement = GetComponent<Player_Movement>();
        Projectile_Pool = new List<Player_Projectile>(Player_Initial_Pool_Size);

        Initialize_Projectile_Pool();
    }

    void Update()
    {
        Update_Mouse_Position();
        Update_Wand_Position_And_Rotation();
        Handle_Attack_Input();
        Update_SO_Runtime_Values();
    }

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Private Methods

    private void Initialize_Projectile_Pool()
    {
        for(int i = 0; i < Player_Initial_Pool_Size; i++)
        {
            Create_New_Projectile();
        }
    }

    private Player_Projectile Create_New_Projectile()
    {
        Player_Projectile projectile = Instantiate(Player_Projectile, Projectile_Parent);
        projectile.gameObject.SetActive(false);
        projectile.Initialize(this, Player_Data_SO.Projectile_Damage);
        Projectile_Pool.Add(projectile);
        return projectile;
    }

    private Player_Projectile Get_Projectile_From_Pool()
    {
        foreach(Player_Projectile projectile in Projectile_Pool)
        {
            if(!projectile.gameObject.activeInHierarchy) return projectile;
        }
        return Create_New_Projectile();
    }

    public void Return_Projectile_To_Pool(Player_Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    private void Update_Mouse_Position()
    {
        if(Main_Camera == null) return;

        Vector3 mouse_Screen_Pos = Input.mousePosition;

        // Ekran sınırları içinde mi kontrol et
        if(mouse_Screen_Pos.x < 0 || mouse_Screen_Pos.x > Screen.width || 
           mouse_Screen_Pos.y < 0 || mouse_Screen_Pos.y > Screen.height)
        {
            return; // Ekran dışındaysa önceki pozisyonu koru
        }

        // Z değerini kamera tipine göre ayarla
        mouse_Screen_Pos.z = Main_Camera.orthographic ? 
            Main_Camera.nearClipPlane : 
            Mathf.Abs(Main_Camera.transform.position.z - transform.position.z);

        Vector2 world_Pos = Main_Camera.ScreenToWorldPoint(mouse_Screen_Pos);

        // Geçerli pozisyon kontrolü
        if(!float.IsInfinity(world_Pos.x) && !float.IsInfinity(world_Pos.y) && 
           !float.IsNaN(world_Pos.x) && !float.IsNaN(world_Pos.y))
        {
            Mouse_World_Position = world_Pos;
        }
    }

    private void Update_Wand_Position_And_Rotation()
    {
        if(Player_Wand_Transform == null) return;

        Vector2 player_Position = transform.position;
        Vector2 direction_To_Mouse = (Mouse_World_Position - player_Position).normalized;

        // Geçersiz yön kontrolü
        if(float.IsNaN(direction_To_Mouse.x) || float.IsNaN(direction_To_Mouse.y))
        {
            direction_To_Mouse = Vector2.right;
        }

        // Pozisyonu hesapla
        Player_Wand_Transform.position = player_Position + (direction_To_Mouse * Wand_Offset_From_Player.magnitude);

        // Rotasyon hesapla (90 derece offset ile)
        float angle = Mathf.Atan2(direction_To_Mouse.y, direction_To_Mouse.x) * Mathf.Rad2Deg + 90f;

        Player_Wand_Transform.rotation = Quaternion.Lerp(
            Player_Wand_Transform.rotation,
            Quaternion.Euler(0, 0, angle),
            Wand_Rotation_Speed * Time.deltaTime
        );
    }

    private void Handle_Attack_Input()
    {
        if(Input.GetMouseButton(0) && Time.time >= Last_Fire_Time + Player_Fire_Rate)
        {
            Fire_Projectile();
            Last_Fire_Time = Time.time;
        }
    }

    private void Fire_Projectile()
    {
        if(Projectile_Spawn_Point == null || Player_Projectile == null) return;

        Player_Projectile projectile = Get_Projectile_From_Pool();
        Vector2 spawn_Position = Projectile_Spawn_Point.position;
        Vector2 direction = (Mouse_World_Position - spawn_Position).normalized;

        // Geçersiz yön kontrolü
        if(float.IsNaN(direction.x) || float.IsNaN(direction.y))
        {
            direction = Vector2.right;
        }

        float rotation_Angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.SetPositionAndRotation(spawn_Position, Quaternion.Euler(0, 0, rotation_Angle));
        projectile.gameObject.SetActive(true);
        projectile.Launch(direction, Player_Projectile_Speed, Player_Projectile_Lifetime);

        if(Player_Movement != null && Player_Data_SO != null)
        {
            Player_Movement.Apply_Recoil(-direction * Player_Data_SO.Projectile_Recoil_Force);
        }
    }

    private void Update_SO_Runtime_Values()
    {
        if(Player_Data_SO == null) return;
        //TODO Buraya f5 so save ekle
    }

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Debug

    void OnDrawGizmos()
    {
        if(!Show_Debug_Gizmos || !Application.isPlaying || Main_Camera == null) return;

        Vector2 player_Position = transform.position;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(player_Position, Mouse_World_Position);

        if(Player_Wand_Transform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(player_Position, Player_Wand_Transform.position);
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay(Player_Wand_Transform.position, ((Mouse_World_Position - (Vector2)Player_Wand_Transform.position).normalized * Wand_Length));
        }

        if(Projectile_Spawn_Point != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Projectile_Spawn_Point.position, 0.1f);
        }
    }

    #endregion
    
    //*-------------------------------------------------------------------------------------------------------*\\
}