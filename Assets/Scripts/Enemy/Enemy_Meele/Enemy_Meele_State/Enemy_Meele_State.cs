using UnityEngine;

public class Enemy_Meele_State : MonoBehaviour
{
    //*-------------------------------------------------------------------------------------------------------*\\
    #region State Machine

    public enum Enemy_Meele_State_Machine
    {
        Enemy_Meele_Walking,
        Enemy_Meele_Attacking,
        Enemy_Meele_Pooling
    }

    #endregion
    //*-------------------------------------------------------------------------------------------------------*\\
    #region Inspector Tab

    [Header("References ---------------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private Enemy_Meele_Data_SO Enemy_Meele_Data_SO;
    [SerializeField] private Rigidbody2D Enemy_Meele_Rigidbody;
    [SerializeField] private Enemy_Distance_Calculator Enemy_Meele_Distance_Calculator;
    [Space]
    [Header("Enemy_Meele_Movement_Settings ----------------------------------------------------------------------")]
    [Space]
    [SerializeField] private float Enemy_Meele_Movement_Speed = 3f;
    [SerializeField] private float Enemy_Meele_Max_Speed = 3f;
    [SerializeField] private float Enemy_Meele_Acceleration_Rate = 6f;
    [SerializeField] private float Enemy_Meele_Deceleration_Rate = 8f;
    [Space]
    [Header("Enemy_Meele_Distance_Settings ----------------------------------------------------------------------")]
    [Space]
    [SerializeField] private float Enemy_Meele_Detection_Range = 10f;
    [SerializeField] private float Enemy_Meele_Stop_Distance = 1.5f;
    [SerializeField] private float Enemy_Meele_Attack_Range = 2f;
    [Space]
    [Header("Enemy_Meele_Attack_Settings -----------------------------------------------------------------------")]
    [Space]
    [SerializeField] private float Enemy_Meele_Attack_Damage = 10f;
    [SerializeField] private float Enemy_Meele_Attack_Cooldown = 1.5f;
    [SerializeField] private float Enemy_Meele_Attack_Duration = 0.5f;
    [Space]
    [Header("Debug --------------------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private bool Show_Debug_Gizmos = false;
    [Space]
    [SerializeField] private bool Use_SO_Data = true;
    [Space]
    [Header("SO_Data_Save -------------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private KeyCode Save_Current_Data_To_SO = KeyCode.F5;

    #endregion
    //*-------------------------------------------------------------------------------------------------------*\\
    #region Private Variables

    private Enemy_Meele_State_Machine Enemy_Meele_Current_State;
    private Vector2 Enemy_Meele_Movement_Direction;
    private float Enemy_Meele_Current_Distance;
    private float Enemy_Meele_Attack_Timer;

    #endregion
    //*-------------------------------------------------------------------------------------------------------*\\
    #region Unity Lifecycle

    void Awake()
    {
        Enemy_Meele_Rigidbody ??= GetComponent<Rigidbody2D>();
        Enemy_Meele_Distance_Calculator ??= GetComponent<Enemy_Distance_Calculator>();

        if (Enemy_Meele_Rigidbody != null)
        {
            Enemy_Meele_Rigidbody.gravityScale = 0f;
            Enemy_Meele_Rigidbody.freezeRotation = true;
            Enemy_Meele_Rigidbody.linearDamping = 2.5f; // doğal frenleme
            Enemy_Meele_Rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        if (Use_SO_Data) Update_SO_Data();
        Enemy_Meele_Current_State = Enemy_Meele_State_Machine.Enemy_Meele_Walking;
    }

    void Update()
    {
        if (Input.GetKeyDown(Save_Current_Data_To_SO)) Save_Runtime_Data_To_SO();

        Update_Timers();
        Update_State_Machine();
    }

    void FixedUpdate()
    {
        if (Enemy_Meele_Current_State == Enemy_Meele_State_Machine.Enemy_Meele_Walking)
        {
            Update_AI_Logic();
            Apply_Force_Movement();
            Handle_Rotation();
        }
    }

    #endregion
    //*-------------------------------------------------------------------------------------------------------*\\
    #region Private Methods

    private void Update_SO_Data()
    {
        if (Enemy_Meele_Data_SO == null) return;

        Enemy_Meele_Movement_Speed = Enemy_Meele_Data_SO.Enemy_Meele_Movement_Speed;
        Enemy_Meele_Max_Speed = Enemy_Meele_Data_SO.Enemy_Meele_Max_Speed;
        Enemy_Meele_Acceleration_Rate = Enemy_Meele_Data_SO.Enemy_Meele_Acceleration_Force;
        Enemy_Meele_Detection_Range = Enemy_Meele_Data_SO.Enemy_Meele_Detection_Range;
        Enemy_Meele_Stop_Distance = Enemy_Meele_Data_SO.Enemy_Meele_Stop_Distance;
        Enemy_Meele_Attack_Range = Enemy_Meele_Data_SO.Enemy_Meele_Attack_Range;
        Enemy_Meele_Attack_Damage = Enemy_Meele_Data_SO.Enemy_Meele_Attack_Damage;
        Enemy_Meele_Attack_Cooldown = Enemy_Meele_Data_SO.Enemy_Meele_Attack_Cooldown;
        Enemy_Meele_Attack_Duration = Enemy_Meele_Data_SO.Enemy_Meele_Attack_Duration;
    }

    private void Save_Runtime_Data_To_SO()
    {
#if UNITY_EDITOR
        if (Enemy_Meele_Data_SO == null) return;

        UnityEditor.Undo.RecordObject(Enemy_Meele_Data_SO, "Save Runtime Enemy Data");

        Enemy_Meele_Data_SO.Enemy_Meele_Movement_Speed = Enemy_Meele_Movement_Speed;
        Enemy_Meele_Data_SO.Enemy_Meele_Max_Speed = Enemy_Meele_Max_Speed;
        Enemy_Meele_Data_SO.Enemy_Meele_Acceleration_Force = Enemy_Meele_Acceleration_Rate;
        Enemy_Meele_Data_SO.Enemy_Meele_Detection_Range = Enemy_Meele_Detection_Range;
        Enemy_Meele_Data_SO.Enemy_Meele_Stop_Distance = Enemy_Meele_Stop_Distance;
        Enemy_Meele_Data_SO.Enemy_Meele_Attack_Range = Enemy_Meele_Attack_Range;
        Enemy_Meele_Data_SO.Enemy_Meele_Attack_Damage = Enemy_Meele_Attack_Damage;
        Enemy_Meele_Data_SO.Enemy_Meele_Attack_Cooldown = Enemy_Meele_Attack_Cooldown;
        Enemy_Meele_Data_SO.Enemy_Meele_Attack_Duration = Enemy_Meele_Attack_Duration;

        UnityEditor.EditorUtility.SetDirty(Enemy_Meele_Data_SO);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }

    private void Update_Timers()
    {
        Enemy_Meele_Attack_Timer = Mathf.Max(0, Enemy_Meele_Attack_Timer - Time.deltaTime);
    }

    private void Update_State_Machine()
    {
        if (Enemy_Meele_Distance_Calculator == null || !Enemy_Meele_Distance_Calculator.Has_Valid_Target) return;

        Enemy_Meele_Current_Distance = Enemy_Meele_Distance_Calculator.Current_Distance;

        Enemy_Meele_Distance_Calculator.Set_Detection_Range(Enemy_Meele_Detection_Range);
        Enemy_Meele_Distance_Calculator.Set_Stop_Distance(Enemy_Meele_Stop_Distance);
        Enemy_Meele_Distance_Calculator.Set_Attack_Range(Enemy_Meele_Attack_Range);

        switch (Enemy_Meele_Current_State)
        {
            case Enemy_Meele_State_Machine.Enemy_Meele_Walking:
                if (Enemy_Meele_Distance_Calculator.Is_Target_In_Attack_Range && Enemy_Meele_Attack_Timer <= 0)
                    Start_Attack();
                break;

            case Enemy_Meele_State_Machine.Enemy_Meele_Attacking:
                if (Enemy_Meele_Attack_Timer <= 0)
                    End_Attack();
                break;

            case Enemy_Meele_State_Machine.Enemy_Meele_Pooling:
                // TODO: Pooling logic
                break;
        }
    }

    private void Start_Attack()
    {
        Enemy_Meele_Current_State = Enemy_Meele_State_Machine.Enemy_Meele_Attacking;
        Enemy_Meele_Attack_Timer = Enemy_Meele_Attack_Duration;

        if (Enemy_Meele_Rigidbody != null)
            Enemy_Meele_Rigidbody.linearVelocity = Vector2.zero;

        Debug.Log("Attack Started");
    }

    private void End_Attack()
    {
        Enemy_Meele_Current_State = Enemy_Meele_State_Machine.Enemy_Meele_Walking;
        Enemy_Meele_Attack_Timer = Enemy_Meele_Attack_Cooldown;

        Debug.Log("Attack Ended - Cooldown Started");
    }

    private void Update_AI_Logic()
    {
        if (Enemy_Meele_Distance_Calculator == null || !Enemy_Meele_Distance_Calculator.Has_Valid_Target)
        {
            Enemy_Meele_Movement_Direction = Vector2.zero;
            return;
        }

        if (Enemy_Meele_Distance_Calculator.Is_Target_In_Detection_Range &&
            Enemy_Meele_Distance_Calculator.Is_Target_Beyond_Stop_Range)
        {
            Enemy_Meele_Movement_Direction = Enemy_Meele_Distance_Calculator.Direction_To_Target;
        }
        else
        {
            Enemy_Meele_Movement_Direction = Vector2.zero;
        }
    }

    private void Apply_Force_Movement()
    {
        if (Enemy_Meele_Rigidbody == null) return;

        Vector2 desiredVelocity = Enemy_Meele_Movement_Direction * Enemy_Meele_Movement_Speed;
        Vector2 velocityDiff = desiredVelocity - Enemy_Meele_Rigidbody.linearVelocity;

        float accel = (Enemy_Meele_Movement_Direction.magnitude > 0.1f)
            ? Enemy_Meele_Acceleration_Rate
            : Enemy_Meele_Deceleration_Rate;

        Vector2 force = velocityDiff * accel;
        Enemy_Meele_Rigidbody.AddForce(force, ForceMode2D.Force);

        // Hız sınırını koru
        if (Enemy_Meele_Rigidbody.linearVelocity.magnitude > Enemy_Meele_Max_Speed)
            Enemy_Meele_Rigidbody.linearVelocity =
                Enemy_Meele_Rigidbody.linearVelocity.normalized * Enemy_Meele_Max_Speed;

        // Küçük hızlarda tamamen dur
        if (Enemy_Meele_Movement_Direction == Vector2.zero &&
            Enemy_Meele_Rigidbody.linearVelocity.magnitude < 0.05f)
        {
            Enemy_Meele_Rigidbody.linearVelocity = Vector2.zero;
        }
    }

    private void Handle_Rotation()
    {
        if (Enemy_Meele_Rigidbody != null && Mathf.Abs(Enemy_Meele_Rigidbody.linearVelocity.x) > 0.05f)
        {
            transform.localScale = new Vector3(Mathf.Sign(Enemy_Meele_Rigidbody.linearVelocity.x), 1, 1);
        }
    }

    #endregion
    //*-------------------------------------------------------------------------------------------------------*\\
    #region Public Methods

    public void Reset_Enemy()
    {
        Enemy_Meele_Movement_Direction = Vector2.zero;
        Enemy_Meele_Attack_Timer = 0f;
        Enemy_Meele_Current_State = Enemy_Meele_State_Machine.Enemy_Meele_Walking;

        if (Enemy_Meele_Rigidbody != null)
            Enemy_Meele_Rigidbody.linearVelocity = Vector2.zero;
    }

    #endregion
    //*-------------------------------------------------------------------------------------------------------*\\
    #region Debug

    void OnDrawGizmos()
    {
        if (!Show_Debug_Gizmos || !Application.isPlaying) return;

        Vector3 pos = transform.position;

        if (Enemy_Meele_Rigidbody != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(pos, (Vector3)Enemy_Meele_Rigidbody.linearVelocity.normalized * 2f);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(pos, (Vector3)Enemy_Meele_Movement_Direction * 1.5f);

        Gizmos.color = Enemy_Meele_Current_State switch
        {
            Enemy_Meele_State_Machine.Enemy_Meele_Walking => Color.yellow,
            Enemy_Meele_State_Machine.Enemy_Meele_Attacking => Color.red,
            Enemy_Meele_State_Machine.Enemy_Meele_Pooling => Color.black,
            _ => Color.white
        };
        Gizmos.DrawWireSphere(pos + Vector3.up * 2f, 0.3f);
    }

    #endregion
    //*-------------------------------------------------------------------------------------------------------*\\
}
