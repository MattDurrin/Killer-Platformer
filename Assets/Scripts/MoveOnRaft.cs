using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnRaft : MonoBehaviour {

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Raft")
        {
            transform.parent = collision.transform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        collision.collider.transform.SetParent(null);
    }
}
