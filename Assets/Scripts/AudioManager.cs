using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class AudioManager : GenericSingletonClass<AudioManager>
{
    [SerializeField]
    AudioClip _chompAudioClip, _deathAudioClip, _fruitAudioClip, _blueGhostAudioClip, _gameStartAudioClip;

    AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        //_audioSource.PlayOneShot(_gameStartAudioClip);
    }

    public void Play(AudioClipType audioClipType)
    {
        switch (audioClipType)
        {
            case AudioClipType.chomp:
                _audioSource.PlayOneShot(_chompAudioClip);
                break;
            case AudioClipType.death:
                _audioSource.PlayOneShot(_deathAudioClip);

                break;
            case AudioClipType.fruit:
                _audioSource.PlayOneShot(_fruitAudioClip);

                break;
            case AudioClipType.blueGhost:
                _audioSource.PlayOneShot(_blueGhostAudioClip);

                break;
        }
    }

}

public enum AudioClipType
{
    chomp,
    death,
    fruit,
    blueGhost
}
