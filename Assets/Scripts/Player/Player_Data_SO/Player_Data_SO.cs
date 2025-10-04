using System.Buffers.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_Data_SO", menuName = "Scriptable Objects/Data/Player")]
public class Player_Data_SO : ScriptableObject
{
    //*-------------------------------------------------------------------------------------------------------*\\
    #region Variables
    [Header("Movement_Stats -----------------------------------------------------------------------------------")]
    [Space]
    public float Player_Movement_Speed = 5;
    public float Player_Acceleration_Rate = 10f;
    public float Player_Deceleration_Rate = 15f;
    [Space]
    [Header("Dash_Stats --------------------------------------------------------------------------------------")]
    [Space]
    public float Player_Dash_Speed = 20f;
    public float Player_Dash_Distance = 5f;
    public float Player_Dash_Duration = 5;
    public  float Player_Dash_Cooldown = 1f;
    [Space]
    [Header("Health_Stats -------------------------------------------------------------------------------------")]
    [Space]
    public float Player_Health = 100;

    #endregion
    //*-------------------------------------------------------------------------------------------------------*\\
}
