using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behavior de gestion du swipe.
/// </summary>
public class ScaleOnSwipe : MonoBehaviour
{
    /// <summary>
    /// Obtient ou définit la taille par défaut.
    /// </summary>
    public float Scale = 0.1f;

    /// <summary>
    /// Obtient ou définit le vecteur de départ du swipe.
    /// </summary>
    private Vector2 startPosition;

    /// <summary>
    /// Fonction appelé a chaque mise à jour du Behavior.
    /// </summary>
    void Update()
    {
        HandleSwipeChange();
    }

    /// <summary>
    /// Fonction de gestion du swipe.
    /// </summary>
    private void HandleSwipeChange()
    {
        Touch[] touches = Input.touches;
        if (touches.Length >= 1)
        {
            Touch currentTouch = touches[0];
            if (currentTouch.phase == TouchPhase.Began)
            {
                startPosition = currentTouch.position;
            }
            else if (currentTouch.phase == TouchPhase.Ended)
            {
                Vector2 endPosition = currentTouch.position;
                HandleSwipeEnd(startPosition, endPosition);
            }
        }
    }

    /// <summary>
    /// Fonction de gestion de la taille des elements.
    /// </summary>
    /// <param name="startPosition">Position de départ du swipe.</param>
    /// <param name="endPosition">Position de fin de swipe.</param>
    private void HandleSwipeEnd(Vector2 startPosition, Vector2 endPosition)
    {
        bool isUpwardSwipe = startPosition.y < endPosition.y;
        bool isDownwardSwipe = startPosition.y > endPosition.y;

        // Si on swipe vers le haut on aagrandi le GameObject, autrement on le diminue
        if (isUpwardSwipe)
        {
            Scale += 0.01f;
        }
        else if (isDownwardSwipe)
        {
            Scale -= 0.01f;
        }
        transform.localScale = new Vector3(Scale ,Scale ,Scale);
    }
}