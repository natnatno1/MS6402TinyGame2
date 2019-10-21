using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public AudioClip[] Sounds;
    public Vector3[] SoundCues;
    public AudioSource Source;
    public int SoundClip;
    public bool NewClip;
    public int NewPosition;
    public float AudioLength;
    public bool Play;
    public BoxCollider Col;

    // Start is called before the first frame update
    void Start()
    {
        Col = GetComponent<BoxCollider>();
        SoundClip = 0;
        Source = gameObject.GetComponent<AudioSource>();
        Resources.Load("Assets / SoundMusic / 1st NTS.mp3");
        NewClip = false;
        NewPosition = 0;
        AudioLength = Sounds[SoundClip].length;


    }

    // Update is called once per frame
    void Update()
    {

        Source.clip = Sounds[SoundClip];
        transform.position = SoundCues[NewPosition];
       // AudioLength = Sounds[SoundClip].length;

        if (AudioLength <= 0)
        {
            SoundClip += 1;
            NewPosition += 1;
            AudioLength = Sounds[SoundClip].length;
            Play = false;
            Col.enabled = true;
        }

        else if (AudioLength > 0 && Play == true)
        {
            AudioLength -= Time.deltaTime;
            Col.enabled = false;
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlaySound();
            //NewPosition += 1;
            //NewPosition += 1;
        }
    }

    void PlaySound()
    {
        Source.Play();
       // StartCoroutine(ExecuteAfterTime(AudioLength));
        Play = true;
    }
    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
        SoundClip += 1;

        yield break;
    }
}
