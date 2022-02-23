using System;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] Song[] playlist = null;

    AudioSource audioSource = null;

    Song currentSong = null;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        //Swap to timeline or coroutine
        SetSong(MusicPhase.Phase0);
    }

    public void SetSong(MusicPhase phase)
    {
        Song songToSet = GetSong(phase);

        audioSource.clip = songToSet.GetSongClip();
        audioSource.volume = songToSet.GetVolume();

        currentSong = songToSet;

        audioSource.Play();
    }

    private Song GetSong(MusicPhase phase)
    {
        Song songToGet = null;

        foreach(Song song in playlist)
        {
            if(song.GetPhase() == phase)
            {
                songToGet = song;
            }
        }

        return songToGet;
    }
}

public enum MusicPhase { Phase0, Phase1, Phase2, Phase3}

[Serializable]
public class Song
{
    [SerializeField] MusicPhase phase = MusicPhase.Phase0;
    [SerializeField] AudioClip songClip = null;
    [Range(0, 1)] [SerializeField] float volume = 1;

    public MusicPhase GetPhase()
    {
        return phase;
    }

    public AudioClip GetSongClip()
    {
        return songClip;
    }

    public float GetVolume()
    {
        return volume;
    }
}
