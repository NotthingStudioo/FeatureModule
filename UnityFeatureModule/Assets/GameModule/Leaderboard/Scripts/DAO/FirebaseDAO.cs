using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FeatureTemplate.Scripts.Services;
using Firebase.Firestore;
using GameModule.Leaderboard.Scripts.DAO;
using GameModule.Leaderboard.Scripts.DAO.QueryBuilder;

public class FirebaseDAO : BaseDAO
{
    FirebaseFirestore db;

    protected override void InternalInitialize() { this.db = FirebaseFirestore.DefaultInstance; }

    private async UniTask AddData(string documentId, Dictionary<string, object> userData)
    {
        DocumentReference docRef = db.Collection("leaderboard").Document(documentId);
        await docRef.SetAsync(userData).AsUniTask();
        this.LogMessage($"Added data to the {documentId} document in the users collection.");
    }

    // CREATE
    public override async UniTask<bool> Create(string documentId, Dictionary<string, object> userData)
    {
        try
        {
            await AddData(documentId, userData);

            return true; // Successfully created
        }
        catch (Exception e)
        {
            this.LogMessage($"Error creating document: {e.Message}");

            return false; // Failed to create
        }
    }

    // READ
    public override async UniTask<Dictionary<string, object>> Read(string documentId)
    {
        try
        {
            var docRef   = db.Collection("leaderboard").Document(documentId);
            var  snapshot = await docRef.GetSnapshotAsync().AsUniTask();

            if (snapshot.Exists)
            {
                return snapshot.ToDictionary(); // Return document data
            }
            else
            {
                this.LogMessage($"Document {documentId} does not exist.");

                return null; // Document does not exist
            }
        }
        catch (Exception e)
        {
            this.LogMessage($"Error reading document: {e.Message}");

            return null; // Failed to read
        }
    }

    // UPDATE
    public override async UniTask<bool> Update(string documentId, Dictionary<string, object> updates)
    {
        try
        {
            var docRef = this.db.Collection("leaderboard").Document(documentId);
            await docRef.UpdateAsync(updates).AsUniTask();
            this.LogMessage($"Updated document {documentId} in the users collection.");

            return true; // Successfully updated
        }
        catch (Exception e)
        {
            this.LogMessage($"Error updating document: {e.Message}");

            return false; // Failed to update
        }
    }

    // DELETE
    public override async UniTask<bool> Delete(string documentId)
    {
        try
        {
            var docRef = db.Collection("leaderboard").Document(documentId);
            await docRef.DeleteAsync().AsUniTask();
            this.LogMessage($"Deleted document {documentId} from the users collection.");

            return true; // Successfully deleted
        }
        catch (Exception e)
        {
            this.LogMessage($"Error deleting document: {e.Message}");

            return false; // Failed to delete
        }
    }

    public override IQueryBuilder<Dictionary<string,string>> GenerateQueryBuilder()
    {
        var colRef = db.Collection("leaderboard");
        return new FirestoreQueryBuilder<Dictionary<string,string>>(colRef);
    }
}