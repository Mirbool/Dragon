using UnityEngine;

public class AudioMgr : MonoBehaviour
{
    private AudioClip CrtAudioClip;


    private void Awake()
    {

    }

    public void Play(string audioName, float volume)
    {
        CrtAudioClip = Resources.Load<AudioClip>("audios/" + audioName);
        AudioSource.PlayClipAtPoint(CrtAudioClip, this.transform.position, volume);
    }
}

