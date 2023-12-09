using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class VinylStore : MonoBehaviour
{
    public GameObject planePrefab; // Drag and drop a plane prefab in the Inspector

    // Lists/variables to store predefined spawn points as GameObjects
    public List<GameObject> spawnPoints;
    public float albumScale = 2.0f; // Default scale factor
    public int maxAlbumsToSpawn = 10; // Maximum number of albums to spawn

    private int albumsSpawned = 0; // Counter for the number of albums spawned

    void Start()
    {
        string albumsPath = Application.dataPath + "/../Albums"; // Path to the "Albums" folder

        if (!Directory.Exists(albumsPath))
        {
            Directory.CreateDirectory(albumsPath); // Create the "Albums" folder if it doesn't exist
            Debug.Log("Created 'Albums' folder.");
            return; // Exit as there are no albums yet
        }

        string[] albums = Directory.GetDirectories(albumsPath); // Get all directories inside "Albums"

        if (albums.Length == 0)
        {
            Debug.Log("No albums found in 'Albums' folder.");
            return; // Exit if there are no albums inside "Albums"
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogError("Please define spawn points in the Inspector.");
            return; // Exit if the spawn points list is not set
        }

        // Shuffle the spawnPoints list randomly
        ShuffleList(spawnPoints);

        int albumIndex = 0; // Index to cycle through the spawn points

        foreach (string albumPath in albums)
        {
            if (albumsSpawned >= maxAlbumsToSpawn || albumIndex >= spawnPoints.Count)
            {
                Debug.Log("Reached the maximum number of albums to spawn or no more spawn points available.");
                break; // Exit the loop if the maximum albums are spawned or no more spawn points
            }

            string albumName = new DirectoryInfo(albumPath).Name; // Extract the album name from the path
            Debug.Log("Found Album: " + albumName);

            string coverPath = Path.Combine(albumPath, "COVER.png"); // Path to COVER.png file

            if (File.Exists(coverPath))
            {
                // Load the image as a Texture
                byte[] fileData = File.ReadAllBytes(coverPath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(fileData);

                // Create a new Material and assign the URP LIT shader to it
                Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit")); // Using URP LIT shader
                newMaterial.mainTexture = texture;

                // Get the next spawn point GameObject from the list
                GameObject spawnPoint = spawnPoints[albumIndex];

                // Create a plane and assign the material to it at the specified spawn point
                GameObject albumPlane = Instantiate(planePrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                albumPlane.GetComponent<Renderer>().material = newMaterial;

                // Set the scale for visibility and arrange the albums
                albumPlane.transform.localScale = new Vector3(albumScale, albumScale, albumScale);

                // Increment counters
                albumsSpawned++;
                albumIndex++;
            }
        }
    }

    // Function to shuffle the list randomly
    private void ShuffleList<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
