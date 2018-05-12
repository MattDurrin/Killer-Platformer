using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorLava : MonoBehaviour {
    AudioSource myAudio;
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
        myAudio.Stop();
        //GetComponent<AudioSource>().clip = saw;
    }

    void OnCollisionEnter()  //Plays Sound Whenever collision detected
    {
        myAudio.Play();
    }

    private void OnCollisionExit(Collision collision)
    {
        myAudio.Stop();
    }
}
