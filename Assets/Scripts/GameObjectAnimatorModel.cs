using UnityEngine;

/// <summary>
/// Mod�le de GameObject avec son Animator li�.
/// </summary>
public class GameObjectAnimatorModel
{
    /// <summary>
    /// Obtient ou d�finit le GameObject.
    /// </summary>
    public GameObject GameObject { get; set; }

    /// <summary>
    /// Obtient ou d�finit l'animator du GameObject.
    /// </summary>
    public Animator Animator { get; set; }
}