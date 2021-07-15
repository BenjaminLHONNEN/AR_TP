using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Behavior de gestion du tracking des images.
/// </summary>
[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracking : MonoBehaviour
{
    /// <summary>
    /// Obtient ou d�finit la liste des prefabs � placer.
    /// </summary>
    [SerializeField]
    private GameObject[] placeablePrefabs;

    /// <summary>
    /// Obtinet ou d�finit la liste des prefab plac�.
    /// </summary>
    private Dictionary<string, GameObjectAnimatorModel> spawnedPrefabs = new Dictionary<string, GameObjectAnimatorModel>();

    /// <summary>
    /// Obtient ou d�finit le module de tracking de la scene.
    /// </summary>
    private ARTrackedImageManager trackedImageManager;

    /// <summary>
    /// Obtient ou d�finit la source audio de la scene.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Obtient ou d�finit si la musique est en train de jouer.
    /// </summary>
    private bool _isMusicPlaying = false;

    /// <summary>
    /// Fonction appel� lors du chargement du Behavior.
    /// </summary>
    private void Awake()
    {
        // R�cup�ration des elements attach�s
        trackedImageManager = FindObjectOfType<ARTrackedImageManager>();
        audioSource = FindObjectOfType<AudioSource>();

        // Initialisation des objets
        foreach (var placeablePrefab in placeablePrefabs)
        {
            GameObject newGameObject = Instantiate(placeablePrefab, Vector3.zero, Quaternion.identity);
            newGameObject.name = placeablePrefab.name;
            spawnedPrefabs.Add(placeablePrefab.name, new GameObjectAnimatorModel { GameObject = newGameObject, Animator = newGameObject?.GetComponent<Animator>() });
            newGameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Fonction appel� lors de l'activation du Behavior.
    /// </summary>
    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += TrackedImageManager_OntrackedImagesChanged;
    }

    /// <summary>
    /// Fonction appel� lors de la d�sactivation du Behavior.
    /// </summary>
    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= TrackedImageManager_OntrackedImagesChanged;
    }

    /// <summary>
    /// Evenement lev� lors de la mise a jour d'une image track�.
    /// </summary>
    /// <param name="obj">Information sur l'evenement.</param>
    private void TrackedImageManager_OntrackedImagesChanged(ARTrackedImagesChangedEventArgs obj)
    {
        // Parcour les elements a ajouter.
        foreach (var arTrackedImage in obj.added)
        {
            UpdateImage(arTrackedImage);
        }

        // parcour les elements a mettre � jour.
        foreach (var arTrackedImage in obj.updated)
        {
            UpdateImage(arTrackedImage);
        }

        // Parcour les elements a supprimer.
        foreach (var arTrackedImage in obj.removed)
        {
            spawnedPrefabs[arTrackedImage.name].GameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Fonction appel� lors de la mise � jour d'une Image track�.
    /// </summary>
    /// <param name="arTrackedImage">Image track�.</param>
    private void UpdateImage(ARTrackedImage arTrackedImage)
    {
        // r�cup�ration du nom du prefab.
        string name = arTrackedImage.referenceImage.name;

        // Mise � jour de la position du prefab
        Vector3 pos = arTrackedImage.transform.position;
        GameObject prefab = spawnedPrefabs[name].GameObject;
        prefab.transform.position = pos;
        prefab.SetActive(true);
        
        // On parcour les prefabs instanci�
        foreach (var spawnedPrefab in spawnedPrefabs.Values)
        {
            // On r�cup�re un prefab s'il est proche de notre prefab.
            var target = spawnedPrefabs.Values.FirstOrDefault(v => !v.Equals(spawnedPrefab) && v.GameObject.activeSelf && Vector3.Distance(v.GameObject.transform.position, spawnedPrefab.GameObject.transform.position) < 0.35f);
            if (spawnedPrefab.GameObject.activeSelf && target != null)
            {
                // s'il y un prefab proche on active les animations et on lance la musique.
                spawnedPrefab.Animator.SetBool("IsNear", true);
                spawnedPrefab.GameObject.transform.LookAt(target.GameObject.transform);
                if (!_isMusicPlaying)
                {
                    audioSource.Play();
                    _isMusicPlaying = true;
                }
            }
            else
            {
                // s'il n'y a aucun prefab proche on desactive les animations et on arrete la musique.
                spawnedPrefab.Animator.SetBool("IsNear", false);
                if (_isMusicPlaying)
                {
                    audioSource.Pause();
                    _isMusicPlaying = false;
                }
            }
        }
    }
}