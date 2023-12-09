using System;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.Universal; // Import the Universal Render Pipeline namespace

public class VinylStore : MonoBehaviour
{
    public GameObject planePrefab; // Drag and drop a plane prefab in the Inspector
    public Vector3 vinylScale;

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

        foreach (string albumPath in albums)
        {
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

                // Create a plane and assign the material to it
                GameObject albumPlane = Instantiate(planePrefab, Vector3.zero, Quaternion.identity);
                albumPlane.GetComponent<Renderer>().material = newMaterial;

                // Set the scale of the albums
                albumPlane.transform.localScale = vinylScale;
            }
        }
    }
}
