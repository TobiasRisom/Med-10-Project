using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
public class JsonServerCommunicator : MonoBehaviour
{
    private string baseUrl = "http://localhost:3000/games";
    // Method to fetch all games
    public IEnumerator GetGames()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(baseUrl))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log(jsonResponse);
            }
        }
        Debug.Log("it ran");
    }
    // Method to create a new game
    public IEnumerator CreateGame(string title, string genre, int releaseYear)
    {
        string jsonData = "{\"title\": \"" + title + "\", \"genre\": \"" + genre + "\", \"releaseYear\": " + releaseYear + "}";
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(jsonData);
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(baseUrl, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(postData);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            else
            {
                Debug.Log("Game created successfully!");
            }
        }
    }
    // More methods for other CRUD operations...
    
}