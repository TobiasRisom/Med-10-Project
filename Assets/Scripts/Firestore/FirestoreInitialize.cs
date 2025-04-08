using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class FirestoreInitialize : MonoBehaviour
{
    public FirebaseFirestore firestore;
    
    // Should all fetch functions be in the same script? Find out if we can initialize multiple times
    void Start()
    {
        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            firestore = FirebaseFirestore.GetInstance(app);
        });
    }
    
    // Function to fetch the "Ugeskema" document from "Schedule" collection
    private void GetSchedule()
    {
        // Reference to the "Ugeskema" document in the "Schedule" collection
        DocumentReference docRef = firestore.Collection("Schedule").Document("Skema");

        // Fetch the document
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                DocumentSnapshot snapshot = task.Result;

                // Log the document fields
                LogDocument(snapshot);
            }
            else
            {
                Debug.LogError("Document does not exist or error fetching document.");
            }
        });
    }

    // Helper function to log the document content
    private void LogDocument(DocumentSnapshot snapshot)
    {
        // If the document exists, log its content
        Dictionary<string, object> documentFields = snapshot.ToDictionary();

        // Log all key-value pairs in the document
        foreach (KeyValuePair<string, object> field in documentFields)
        {
            Debug.Log($"{field.Key}: {field.Value}");
        }
        
    }
    
}