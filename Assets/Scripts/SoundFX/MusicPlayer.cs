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

    public void SetSong(PlayerPhase phase)
    {
        audioSource.Stop();

        Song songToSet = GetSong(phase);

        audioSource.clip = songToSet.GetSongClip();
        audioSource.volume = songToSet.GetVolume();

        currentSong = songToSet;

        audioSource.Play();
    }

    private Song GetSong(PlayerPhase phase)
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

    public void Pause()
    {
        audioSource.Pause();
    }
}

[Serializable]
public class Song
{
    [SerializeField] PlayerPhase phase = PlayerPhase.One;
    [SerializeField] AudioClip songClip = null;
    [Range(0, 1)] [SerializeField] float volume = 1;

    public PlayerPhase GetPhase()
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
