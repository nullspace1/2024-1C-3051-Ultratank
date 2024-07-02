using System.Collections.Generic;
using BepuUtilities;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

public enum Audios
{
    MENU_SONG,
    ENEMY_DIED,
    AMBIENT,
    SHOOT,
}

public class AudioManager
{
    private static AudioManager _instance;
    private Dictionary<Audios, SoundEffectInstance> _soundEffects;
    private Dictionary<Audios, Song> _songs;

    private AudioManager()
    {
        _soundEffects = new Dictionary<Audios, SoundEffectInstance>();
        _songs = new Dictionary<Audios, Song>();
    }

    public static AudioManager Instance
    {
        get
        {
            _instance ??= new AudioManager();
            return _instance;
        }
    }

    public void AddSoundEffect(Audios name, SoundEffect soundEffect)
    {
        if (!_soundEffects.ContainsKey(name))
        {
            _soundEffects[name] = soundEffect.CreateInstance();
        }
    }

    public void AddSong(Audios name, Song song)
    {
        if (!_songs.ContainsKey(name))
        {
            _songs[name] = song;
        }
    }

    public void PlaySound(Audios name, bool isLoop = false)
    {
        if (!Constants.PLAY_SOUND)
            return;
        if (_soundEffects.ContainsKey(name))
        {
            _soundEffects[name].IsLooped = isLoop;
            _soundEffects[name].Play();
        }
    }

    public void PlaySong(Audios name)
    {
        if (!Constants.PLAY_SOUND)
            return;

        if (_songs.ContainsKey(name))
        {
            MediaPlayer.Play(_songs[name]);
        }
    }

    public void SetVolume(Audios name, float volume)
    {
        if (_soundEffects.ContainsKey(name))
        {
            _soundEffects[name].Volume = MathHelper.Clamp(volume, 0.0f, 1.0f);
        }
        else if (_songs.ContainsKey(name))
        {
            MediaPlayer.Volume = MathHelper.Clamp(volume, 0.0f, 1.0f);
        }
    }

    public void StopSound(Audios name)
    {
        if (!Constants.PLAY_SOUND)
            return;

        if (_soundEffects.ContainsKey(name))
        {
            _soundEffects[name].Stop();
        }
    }

    public void StopSong()
    {
        MediaPlayer.Stop();
    }

    public void StopAllSounds()
    {
        foreach (var sound in _soundEffects.Values)
            sound.Stop();

        MediaPlayer.Stop();
    }

    public SoundEffectInstance GetSound(Audios name)
    {
        if (_soundEffects.ContainsKey(name))
            return _soundEffects[name];

        return null;
    }

    public Song GetSong(Audios name)
    {
        if (_songs.ContainsKey(name))
            return _songs[name];

        return null;
    }
}