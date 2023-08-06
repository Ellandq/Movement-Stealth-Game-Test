using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private LayerMask collisionLayers;

    private Dictionary<GameObject, Vector3> displacementDictionary;

    private void Start()
    {
        displacementDictionary = new Dictionary<GameObject, Vector3>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collisionLayers & (1 << collision.gameObject.layer)) == 0) return;
        
        Vector3 displacementVector = transform.position - collision.GetContact(0).point;
        displacementVector.y = 0f;

        displacementDictionary.Add(collision.gameObject, displacementVector.normalized);

        playerMovement.CollisionDeceleration(displacementVector.normalized);
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((collisionLayers & (1 << collision.gameObject.layer)) == 0) return;

        if (displacementDictionary.ContainsKey(collision.gameObject))
        {
            Vector3 displacementVector = displacementDictionary[collision.gameObject];
            displacementDictionary.Remove(collision.gameObject);

            playerMovement.ClearCollisionDeceleration(displacementVector.normalized, displacementDictionary.Count);
        }
    }
}
