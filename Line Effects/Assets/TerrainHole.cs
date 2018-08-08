using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHole : MonoBehaviour
{

    // Assign the collider you want to tunnel through.
    public Collider terrain;

    // This lets us keep track of objects 
    // in one of our (possibly many) colliders.
    Dictionary<Rigidbody, int> _containment
                      = new Dictionary<Rigidbody, int>();

    // When a new body enters one of our triggers,
    // make it start ignoring the terrain's collision.
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TerrainHole::OnTriggerEnter()");
        Rigidbody body = other.GetComponent<Rigidbody>();

        float l_Height = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position).y;
        
        if (l_Height > 0)
        {
            Debug.Log(l_Height);
            Physics.IgnoreCollision(other, terrain, false);
            return;
        }

        if (_containment.ContainsKey(body))
        {
            // The body is in the intersection of more than one
            // of our triggers. Keep a count so we know 
            // when it's exited completely.
            _containment[body]++;
        }
        else
        {
            _containment.Add(body, 1);
            Physics.IgnoreCollision(other, terrain);
        }
    }

    // Once a body has exited all of our triggers,
    // tell it to pay attention to the terrain again.
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("TerrainHole::OnTriggerExit()");
        Rigidbody body = other.GetComponent<Rigidbody>();

        int depth;
        if (_containment.TryGetValue(body, out depth))
        {
            depth--;
            if (depth <= 0)
            {
                // The body has left the hole's vicinity.
                // Re-enable collisions and forget about it.
                Physics.IgnoreCollision(other, terrain, false);
                _containment.Remove(body);
            }
            else
            {
                _containment[body] = depth;
            }
        }
    }
}