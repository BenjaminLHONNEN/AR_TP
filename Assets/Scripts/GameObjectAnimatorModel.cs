using UnityEngine;

/// <summary>
/// Modèle de GameObject avec son Animator lié.
/// </summary>
public class GameObjectAnimatorModel
{
    /// <summary>
    /// Obtient ou définit le GameObject.
    /// </summary>
    public GameObject GameObject { get; set; }

    /// <summary>
    /// Obtient ou définit l'animator du GameObject.
    /// </summary>
    public Animator Animator { get; set; }
}