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
    /// Obtient ou définit la liste des prefabs à placer.
    /// </summary>
    [SerializeField]
    private GameObject[] placeablePrefabs;

    /// <summary>
    /// Obtinet ou définit la liste des prefab placé.
    /// </summary>
    private Dictionary<string, GameObjectAnimatorModel> spawnedPrefabs = new Dictionary<string, GameObjectAnimatorModel>();

    /// <summary>
    /// Obtient ou définit le module de tracking de la scene.
    /// </summary>
    private ARTrackedImageManager trackedImageManager;

    /// <summary>
    /// Obtient ou définit la source audio de la scene.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Obtient ou définit si la musique est en train de jouer.
    /// </summary>
    private bool _isMusicPlaying = false;

    /// <summary>
    /// Fonction appelé lors du chargement du Behavior.
    /// </summary>
    private void Awake()
    {
        // Récupération des elements attachés
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
    /// Fonction appelé lors de l'activation du Behavior.
    /// </summary>
    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += TrackedImageManager_OntrackedImagesChanged;
    }

    /// <summary>
    /// Fonction appelé lors de la désactivation du Behavior.
    /// </summary>
    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= TrackedImageManager_OntrackedImagesChanged;
    }

    /// <summary>
    /// Evenement levé lors de la mise a jour d'une image tracké.
    /// </summary>
    /// <param name="obj">Information sur l'evenement.</param>
    private void TrackedImageManager_OntrackedImagesChanged(ARTrackedImagesChangedEventArgs obj)
    {
        // Parcour les elements a ajouter.
        foreach (var arTrackedImage in obj.added)
        {
            UpdateImage(arTrackedImage);
        }

        // parcour les elements a mettre à jour.
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
    /// Fonction appelé lors de la mise à jour d'une Image tracké.
    /// </summary>
    /// <param name="arTrackedImage">Image tracké.</param>
    private void UpdateImage(ARTrackedImage arTrackedImage)
    {
        // récupération du nom du prefab.
        string name = arTrackedImage.referenceImage.name;

        // Mise à jour de la position du prefab
        Vector3 pos = arTrackedImage.transform.position;
        GameObject prefab = spawnedPrefabs[name].GameObject;
        prefab.transform.position = pos;
        prefab.SetActive(true);
        
        // On parcour les prefabs instancié
        foreach (var spawnedPrefab in spawnedPrefabs.Values)
        {
            // On récupère un prefab s'il est proche de notre prefab.
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