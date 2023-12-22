using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Sound
{ 

    public string name;

    public AudioClip clip;
    private AudioSource source;

    public float Volumn;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
    }
    public void Play()
    {
        source.Play();
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField]
    public Sound[] sounds;
    
    void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject soundsource = new GameObject("sound name : " + sounds[i].name);
            sounds[i].SetSource(soundsource.AddComponent<AudioSource>());
        }
    }

    public void Play(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_name == sounds[i].name)
            {
                sounds[i].Play();
                return;
            }
        }
    }

}
