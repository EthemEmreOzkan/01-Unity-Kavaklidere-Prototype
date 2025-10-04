using UnityEngine;

public class Player_Animator_Manager : MonoBehaviour
{
    [Header("Animator References --------------------------------------------------------------")]
    [Space]
    [SerializeField] private Animator Player_Animator;

    //*-----------------------------------------------------------------------------------------//
    #region Public Methods ----------------------------------------------------------------------

    //TODO Animasyonalrı Güncelle

    public void SetBool(string param_name, bool state)
        => Player_Animator.SetBool(Animator.StringToHash(param_name), state);

    #endregion
    //*-----------------------------------------------------------------------------------------//
}
