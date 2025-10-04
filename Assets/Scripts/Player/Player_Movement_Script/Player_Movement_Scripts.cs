using UnityEngine;
using System.Collections;

public class Player_Movement : MonoBehaviour
{
    //*-------------------------------------------------------------------------------------------------------*\\
   
    #region Inspector Tab

    [Header("References ---------------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private Player_Data_SO Player_Data_SO;
    [SerializeField] private Rigidbody2D Player_Rigidbody;
    [SerializeField] private Collider2D Player_Collider;
    [SerializeField] private Player_Animator_Manager Player_Animator_Manager;
    [Space]
    [Header("Movement_Settings --------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private float Player_Movement_Speed = 5f;
    [SerializeField] private float Player_Acceleration_Rate = 10f;
    [SerializeField] private float Player_Deceleration_Rate = 15f;
    [Space]
    [Header("Dash_Settings ------------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private float Player_Dash_Speed = 20f;
    [SerializeField] private float Player_Dash_Distance = 5f;
    [SerializeField] private float Player_Dash_Duration = 0.2f;
    [SerializeField] private float Player_Dash_Cooldown = 1f;
    [Space]
    [Header("Animation_Settings -------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private float Movement_Threshold = 0.1f;
    [Space]
    [Header("Debug --------------------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private bool Show_Debug_Gizmos = false;
    [Space]
    [SerializeField] private bool Use_SO_Data;
    [Space]
    [Header("SO_Data_Save -------------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private KeyCode Save_Current_Data_To_SO = KeyCode.F5;

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Public Variables

    public float Current_Dash_Cooldown { get; private set; }
    public bool Is_Dashing { get; private set; }
    public bool Is_Walking { get; private set; }

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Private Variables

    private Vector2 Movement_Input;
    private Vector2 Current_Velocity;
    private Vector2 Target_Velocity;
    private Vector2 Dash_Direction;
    private Vector2 Last_Movement_Direction;
    private float Dash_Cooldown_Timer;

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Unity Lifecycle

    void Awake()
    {
        if(Use_SO_Data) Update_SO_Data();
        
        if(Player_Rigidbody == null)
        {
            Player_Rigidbody = GetComponent<Rigidbody2D>();
        }

        if(Player_Collider == null)
        {
            Player_Collider = GetComponent<Collider2D>();
        }

        if(Player_Animator_Manager == null)
        {
            Player_Animator_Manager = GetComponent<Player_Animator_Manager>();
        }

        Last_Movement_Direction = Vector2.right;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        Movement_Input = new Vector2(horizontal, vertical);
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Attempt_Dash();
        }

        if(Input.GetKeyDown(Save_Current_Data_To_SO))
        {
            Save_Runtime_Data_To_SO();
        }
        
        if(Dash_Cooldown_Timer > 0)
        {
            Dash_Cooldown_Timer -= Time.deltaTime;
        }
        
        Current_Dash_Cooldown = Mathf.Max(0, Dash_Cooldown_Timer);

        Update_Animation_States();
        Update_Player_Direction();
    }

    void FixedUpdate()
    {
        if(!Is_Dashing)
        {
            Handle_Movement();
        }
    }

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Private Methods

    private void Update_SO_Data()
    {
        Player_Movement_Speed = Player_Data_SO.Player_Movement_Speed;
        Player_Dash_Speed = Player_Data_SO.Player_Dash_Speed;
        Player_Dash_Distance = Player_Data_SO.Player_Dash_Distance;
        Player_Dash_Duration = Player_Data_SO.Player_Dash_Duration;
        Player_Dash_Cooldown = Player_Data_SO.Player_Dash_Cooldown;
        Player_Acceleration_Rate = Player_Data_SO.Player_Acceleration_Rate;
        Player_Deceleration_Rate = Player_Data_SO.Player_Deceleration_Rate;
    }

    private void Save_Runtime_Data_To_SO()
    {
        #if UNITY_EDITOR
        UnityEditor.Undo.RecordObject(Player_Data_SO, "Save Runtime Player Data");
        #endif

        Player_Data_SO.Player_Movement_Speed = Player_Movement_Speed;
        Player_Data_SO.Player_Dash_Speed = Player_Dash_Speed;
        Player_Data_SO.Player_Dash_Distance = Player_Dash_Distance;
        Player_Data_SO.Player_Dash_Duration = Player_Dash_Duration;
        Player_Data_SO.Player_Dash_Cooldown = Player_Dash_Cooldown;
        Player_Data_SO.Player_Acceleration_Rate = Player_Acceleration_Rate;
        Player_Data_SO.Player_Deceleration_Rate = Player_Deceleration_Rate;

        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(Player_Data_SO);
        UnityEditor.AssetDatabase.SaveAssets();
        #endif

        Debug.Log($"<color=green>Runtime data saved to SO: {Player_Data_SO.name}</color>");
    }

    private void Handle_Movement()
    {
        Target_Velocity = Movement_Input.normalized * Player_Movement_Speed;
        float interpolation_Rate = Movement_Input.magnitude > 0.1f ? Player_Acceleration_Rate : Player_Deceleration_Rate;
        Current_Velocity = Vector2.Lerp(Current_Velocity, Target_Velocity, interpolation_Rate * Time.fixedDeltaTime);
        Player_Rigidbody.linearVelocity = Current_Velocity;
    }

    private void Update_Animation_States()
    {
        if(Player_Animator_Manager == null) return;

        bool was_Walking = Is_Walking;
        Is_Walking = Current_Velocity.magnitude > Movement_Threshold && !Is_Dashing;

        if(was_Walking != Is_Walking)
        {
            Player_Animator_Manager.SetBool("Is_Walking", Is_Walking);
        }
    }

    private void Update_Player_Direction()
    {
        if(Movement_Input.magnitude > 0.1f)
        {
            Last_Movement_Direction = Movement_Input;

            if(Movement_Input.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if(Movement_Input.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
    }

    private void Attempt_Dash()
    {
        if(Is_Dashing || Dash_Cooldown_Timer > 0) return;
        
        if(Movement_Input.magnitude > 0.1f)
        {
            Dash_Direction = Movement_Input.normalized;
        }
        else
        {
            Dash_Direction = Last_Movement_Direction.normalized;
        }
        
        StartCoroutine(Perform_Dash());
    }

    private IEnumerator Perform_Dash()
    {
        Is_Dashing = true;
        Dash_Cooldown_Timer = Player_Dash_Cooldown;
        
        if(Player_Animator_Manager != null)
        {
            Player_Animator_Manager.SetBool("Is_Dashing", true);
            Player_Animator_Manager.SetBool("Is_Walking", false);
        }
        
        if(Player_Collider != null)
        {
            Player_Collider.enabled = false;
        }
        
        float elapsed_Time = 0f;
        Vector2 start_Position = transform.position;
        Vector2 target_Position = start_Position + (Dash_Direction * Player_Dash_Distance);
        
        while(elapsed_Time < Player_Dash_Duration)
        {
            elapsed_Time += Time.deltaTime;
            float progress = elapsed_Time / Player_Dash_Duration;
            
            Vector2 new_Position = Vector2.Lerp(start_Position, target_Position, progress);
            Player_Rigidbody.MovePosition(new_Position);
            
            yield return null;
        }
        
        Player_Rigidbody.linearVelocity = Vector2.zero;
        Current_Velocity = Vector2.zero;
        
        if(Player_Collider != null)
        {
            Player_Collider.enabled = true;
        }
        
        if(Player_Animator_Manager != null)
        {
            Player_Animator_Manager.SetBool("Is_Dashing", false);
        }
        
        Is_Dashing = false;
    }

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Debug 

    void OnDrawGizmos()
    {
        if (!Show_Debug_Gizmos || !Application.isPlaying) return;
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Current_Velocity.normalized * 2f);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, (Vector3)Movement_Input * 1.5f);
        
        if(Is_Dashing)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, (Vector3)(Dash_Direction * Player_Dash_Distance));
        }
    }

    #endregion
    
    //*-----------------------------------------------------------------------------------------//
}