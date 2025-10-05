using UnityEngine;

public class Enemy_Distance_Calculator : MonoBehaviour
{
    //*-------------------------------------------------------------------------------------------------------*\\

    #region Inspector Tab

    [Header("References ---------------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private Transform Player_Transform;
    [Space]
    [Header("Distance Settings --------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private float Detection_Range = 10f;
    [SerializeField] private float Stop_Distance = 1.5f;
    [SerializeField] private float Attack_Range = 2f;
    [Space]
    [Header("Advanced Settings --------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private bool Auto_Find_Player = true;
    [Space]
    [Header("Debug --------------------------------------------------------------------------------------------")]
    [Space]
    [SerializeField] private bool Show_Debug_Gizmos = true;

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Public Properties

    public float Current_Distance { get; private set; }
    public Vector2 Direction_To_Target { get; private set; }
    public Transform Current_Target => Player_Transform;
    public float Detection_Range_Value => Detection_Range;
    public float Stop_Distance_Value => Stop_Distance;
    public float Attack_Range_Value => Attack_Range;

    public bool Is_Target_In_Detection_Range => Current_Distance <= Detection_Range;
    public bool Is_Target_In_Stop_Range => Current_Distance <= Stop_Distance;
    public bool Is_Target_In_Attack_Range => Current_Distance <= Attack_Range;
    public bool Is_Target_Beyond_Stop_Range => Current_Distance > Stop_Distance;
    public bool Has_Valid_Target => Player_Transform != null;

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Private Variables

    private Vector2 position_2D;
    private Vector2 target_Position_2D;

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Unity Lifecycle

    void Awake()
    {
        Initialize_Target();
    }

    void Update()
    {
        Update_Distance_Calculations();
    }

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Public Methods

    public void Set_Target(Transform new_Target)
    {
        Player_Transform = new_Target;
        Update_Distance_Calculations();
    }

    public void Set_Detection_Range(float new_Range)
    {
        Detection_Range = Mathf.Max(0f, new_Range);
    }

    public void Set_Stop_Distance(float new_Distance)
    {
        Stop_Distance = Mathf.Max(0f, new_Distance);
    }

    public void Set_Attack_Range(float new_Range)
    {
        Attack_Range = Mathf.Max(0f, new_Range);
    }

    public bool Is_Target_Within_Range(float custom_Range)
    {
        return Current_Distance <= custom_Range;
    }

    public bool Is_Target_Beyond_Range(float custom_Range)
    {
        return Current_Distance > custom_Range;
    }

    public void Reset_Calculator()
    {
        Current_Distance = float.MaxValue;
        Direction_To_Target = Vector2.zero;
        Initialize_Target();
    }

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Private Methods

    private void Initialize_Target()
    {
        if (Auto_Find_Player && Player_Transform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Player_Transform = player.transform;
            }
        }
    }

    private void Update_Distance_Calculations()
    {
        if (Player_Transform == null)
        {
            Current_Distance = float.MaxValue;
            Direction_To_Target = Vector2.zero;
            return;
        }

        position_2D = transform.position;
        target_Position_2D = Player_Transform.position;

        Current_Distance = Vector2.Distance(position_2D, target_Position_2D);
        Direction_To_Target = (target_Position_2D - position_2D).normalized;
    }

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Debug

    void OnDrawGizmos()
    {
        if (!Show_Debug_Gizmos) return;

        Vector3 pos = transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, Detection_Range);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, Attack_Range);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pos, Stop_Distance);

        if (Application.isPlaying && Player_Transform != null)
        {
            Gizmos.color = Is_Target_In_Attack_Range ? Color.red : 
                          Is_Target_In_Detection_Range ? Color.magenta : Color.white;
            Gizmos.DrawLine(pos, Player_Transform.position);
        }
    }

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\
}