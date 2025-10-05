using UnityEngine;

[CreateAssetMenu(fileName = "Enemy_Meele_Data_SO", menuName = "ScriptableObjects/Data/Enemy/Enemy_Meele_Data_SO")]
public class Enemy_Meele_Data_SO : ScriptableObject
{
    //*-------------------------------------------------------------------------------------------------------*\\

    #region Enemy_Meele_Movement_Settings

    [Header("Enemy_Meele_Movement_Settings ----------------------------------------------------------------------")]
    [Space]
    public float Enemy_Meele_Movement_Speed = 3f;
    public float Enemy_Meele_Max_Speed = 3f;
    public float Enemy_Meele_Acceleration_Force = 15f;

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Enemy_Meele_Distance_Settings

    [Space]
    [Header("Enemy_Meele_Distance_Settings ----------------------------------------------------------------------")]
    [Space]
    public float Enemy_Meele_Detection_Range = 10f;
    public float Enemy_Meele_Stop_Distance = 1.5f;
    public float Enemy_Meele_Attack_Range = 2f;

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\

    #region Enemy_Meele_Attack_Settings

    [Space]
    [Header("Enemy_Meele_Attack_Settings -----------------------------------------------------------------------")]
    [Space]
    public float Enemy_Meele_Attack_Damage = 10f;
    public float Enemy_Meele_Attack_Cooldown = 1.5f;
    public float Enemy_Meele_Attack_Duration = 0.5f;

    #endregion

    //*-------------------------------------------------------------------------------------------------------*\\
}