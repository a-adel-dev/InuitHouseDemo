using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;



[System.Serializable]
public class TimedSpawnEvent
{
    public GameObject objectToSpawn;
    public float spawnTime;  // Time in seconds when to spawn
    public Vector3 spawnOffset = Vector3.zero;  // Offset relative to video position
    public bool hasSpawned = false;
}

public class VideoSpawner : MonoBehaviour
{
    [Header("Video Settings")]
    [Tooltip("Video clip to play")]
    public VideoClip videoClip;
    
    [Tooltip("Prefab containing a Quad with VideoPlayer (optional)")]
    public GameObject videoPrefab;
    
    [Header("Spawn Settings")]
    [Tooltip("Distance in front of click position to spawn video")]
    public float spawnDistance = 2f;

    public Transform spawnPosition;
    
    [Tooltip("Size of the video display")]
    public Vector2 videoSize = new Vector2(1.6f, 0.9f); // 16:9 aspect ratio
    
    [Header("Timed Object Spawning")]
    [Tooltip("List of objects to spawn at specific times")]
    public List<TimedSpawnEvent> spawnEvents = new List<TimedSpawnEvent>();
    
    [Tooltip("Whether spawned objects should be destroyed when video ends")]
    public bool destroyObjectsOnVideoEnd = true;
    
    private GameObject currentVideoObject;
    private VideoPlayer videoPlayer;
    private Material videoMaterial;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    private void Start()
    {
        // Create the video material
        CreateVideoMaterial();
        
        // If no prefab is assigned, create a default video setup
        if (videoPrefab == null)
        {
            CreateDefaultVideoPrefab();
        }
        // Sort spawn events by time for efficiency
        spawnEvents.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));
    }
    
    private void CreateVideoMaterial()
    {
        // Create an unlit material specifically for video
        videoMaterial = new Material(Shader.Find("Unlit/Texture"));
        videoMaterial.name = "Video Material";
    }

    private void CreateDefaultVideoPrefab()
    {
        // Create a quad
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.localScale = videoSize;
        
        // Set the unlit material
        quad.GetComponent<Renderer>().material = videoMaterial;
        
        // Add video player component
        VideoPlayer videoPlayer = quad.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
        videoPlayer.targetMaterialRenderer = quad.GetComponent<Renderer>();
        videoPlayer.aspectRatio = VideoAspectRatio.FitInside;
        
        // Store as prefab reference
        videoPrefab = quad;
        videoPrefab.SetActive(false);
    }

    private void Update()
    {
        HandleMouseClick();
        CheckSpawnEvents();
    }
    
    private void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
            {
                SpawnVideo(hit.point);
            }
        }
    }
    private void CheckSpawnEvents()
    {
        if (videoPlayer == null || !videoPlayer.isPlaying) return;

        float currentTime = (float)videoPlayer.time;
        
        foreach (var spawnEvent in spawnEvents)
        {
            if (!spawnEvent.hasSpawned && currentTime >= spawnEvent.spawnTime)
            {
                SpawnObject(spawnEvent);
                spawnEvent.hasSpawned = true;
            }
        }

        // Check if video has ended
        if (videoPlayer.frame >= (long)(videoPlayer.frameCount - 1))
        {
            OnVideoComplete();
        }
    }

    private void SpawnObject(TimedSpawnEvent spawnEvent)
    {
        if (spawnEvent.objectToSpawn == null || currentVideoObject == null) return;

        // Calculate spawn position relative to video
        Vector3 spawnPosition = currentVideoObject.transform.position + 
                                currentVideoObject.transform.rotation * spawnEvent.spawnOffset;

        // Instantiate the object
        GameObject spawnedObject = Instantiate(spawnEvent.objectToSpawn, spawnPosition, currentVideoObject.transform.rotation);
        spawnedObjects.Add(spawnedObject);
    }

    private void SpawnVideo(Vector3 hitPoint)
    {
        // Destroy existing video if there is one
        if (currentVideoObject != null)
        {
            Destroy(currentVideoObject);
        }

        // Calculate spawn position
        //Vector3 spawnPosition = hitPoint + Camera.main.transform.forward * spawnDistance;

        // Spawn the video object
        currentVideoObject = Instantiate(videoPrefab, spawnPosition.position, spawnPosition.rotation);
        currentVideoObject.SetActive(true);

        // Make it face the camera
        //currentVideoObject.transform.LookAt(Camera.main.transform);
        //currentVideoObject.transform.Rotate(0, 180, 0); // Flip to face camera

        // Setup video player
        videoPlayer = currentVideoObject.GetComponent<VideoPlayer>();
        videoPlayer.clip = videoClip;
        videoPlayer.Play();
    }

    // Call this method to stop and destroy the video
    public void StopVideo()
    {
        if (currentVideoObject != null)
        {
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
            }
            Destroy(currentVideoObject);
        }
    }
    
    private void OnVideoComplete()
    {
        if (destroyObjectsOnVideoEnd)
        {
            CleanupSpawnedObjects();
        }
    }
    
    private void CleanupCurrentVideo()
    {
        if (currentVideoObject != null)
        {
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
            }
            Destroy(currentVideoObject);
        }
        CleanupSpawnedObjects();
    }
    
    private void CleanupSpawnedObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        spawnedObjects.Clear();
    }
    
    private void OnDestroy()
    {
        if (videoMaterial != null)
        {
            Destroy(videoMaterial);
        }
    }
}