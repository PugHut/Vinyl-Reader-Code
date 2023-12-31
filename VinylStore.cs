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

        // Shuffle the albums list randomly
        ShuffleArray(albums);

        // Shuffle the spawnPoints list randomly
        ShuffleList(spawnPoints);

        int albumIndex = 0; // Index to cycle through the albums

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
            string backPath = Path.Combine(albumPath, "BACK.png"); // Path to BACK.png file

            if (File.Exists(coverPath))
            {
                // Load the cover image as a Texture
                byte[] coverData = File.ReadAllBytes(coverPath);
                Texture2D coverTexture = new Texture2D(2, 2);
                coverTexture.LoadImage(coverData);

                // Create a new Material for the front cover and assign the URP LIT shader to it
                Material frontMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit")); // Using URP LIT shader
                frontMaterial.mainTexture = coverTexture;

                Material backMaterial = null;

                if (File.Exists(backPath))
                {
                    // Load the back image as a Texture
                    byte[] backData = File.ReadAllBytes(backPath);
                    Texture2D backTexture = new Texture2D(2, 2);
                    backTexture.LoadImage(backData);

                    // Create a new Material for the back cover and assign the URP LIT shader to it
                    backMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit")); // Using URP LIT shader
                    backMaterial.mainTexture = backTexture;
                }

                // Get the next spawn point GameObject from the list
                GameObject spawnPoint = spawnPoints[albumIndex];

                // Create a vinyl prefab and assign the materials to it at the specified spawn point
                GameObject vinylPrefab = Instantiate(planePrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                vinylPrefab.GetComponent<Renderer>().materials = new Material[] { frontMaterial, backMaterial };

                // Set the scale for visibility and arrange the albums
                vinylPrefab.transform.localScale = new Vector3(albumScale, albumScale, albumScale);

                // Increment counters
                albumsSpawned++;
                albumIndex++;
            }
        }
    }

    // Function to shuffle the array randomly
    private void ShuffleArray<T>(T[] array)
    {
        System.Random random = new System.Random();
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = array[k];
            array[k] = array[n];
            array[n] = value;
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
