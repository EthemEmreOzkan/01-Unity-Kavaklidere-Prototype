using System.Buffers.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Player_Data_SO", menuName = "Scriptable Objects/Data/Player")]
public class Player_Data_SO : ScriptableObject
{
    //*-------------------------------------------------------------------------------------------------------*\\
    #region Variables

    [Header("Movement_Stats -----------------------------------------------------------------------------------")]
    public float Player_Dash_Duration = 5;
    public float Player_Movement_Speed = 5;
    public float Player_Health = 100;

    #endregion
    //*-------------------------------------------------------------------------------------------------------*\\
}
