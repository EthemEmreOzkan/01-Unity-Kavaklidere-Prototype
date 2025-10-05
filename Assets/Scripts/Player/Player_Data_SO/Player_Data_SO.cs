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
    [Header("Attack_Stats -------------------------------------------------------------------------------------")]
    [Space]
    public float Projectile_Damage = 10f;
    public float Projectile_Recoil_Force = 2f;
    public float Projectile_Speed = 15f;
    public float Attack_Cooldown = 0.5f;
    public float Projectile_Lifetime = 3f;
    public float Recoil_Force = 0.5f;
    public float Recoil_Duration = 0.15f;
    public float Recoil_Smooth_Speed = 10f;
    [Space]
    [Header("Health_Stats -------------------------------------------------------------------------------------")]
    [Space]
    public float Player_Health = 100;

    #endregion
    //*-------------------------------------------------------------------------------------------------------*\\
}
