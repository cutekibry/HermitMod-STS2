using System;
using System.Collections.Generic;
using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves;

namespace HermitMod.Utility;

public static class ModAudio
{
    private static readonly Dictionary<string, AudioStream> CachedStreams = new();

    private static AudioStreamPlayer? _musicPlayer;
    private static string? _currentMusicPath;
    private static float _currentVolumeOffset;
    private static Tween? _fadeTween;
    private static AudioStreamPlayer? _outgoingPlayer;
    private static Tween? _outgoingFadeTween;
    private static AudioStreamPlayer? _ambiencePlayer;
    private static string? _currentAmbiencePath;
    private static Tween? _ambienceFadeTween;

    private const float MusicVolumeOffset = -6f;
    private const float AmbienceVolumeOffset = -6f;
    private const float SfxVolumeOffset = -3f;
    private const string ModPath = "res://HermitMod";

    public static void Play(string folder, string soundName, float volume = 0f, float pitchVariation = 0f, float basePitch = 1f)
    {
        var stream = GetOrLoadStream(folder, soundName);
        if (stream == null) return;

        var player = new AudioStreamPlayer
        {
            Stream = stream,
            VolumeDb = volume + SfxVolumeOffset,
            Bus = new StringName("SFX"),
            PitchScale = pitchVariation > 0f
                ? basePitch + (float)Rng.Chaotic.NextDouble() * 2f * pitchVariation - pitchVariation
                : basePitch
        };

        var combatRoom = NCombatRoom.Instance;
        if (combatRoom != null)
        {
            ((Node)combatRoom).AddChild(player);
            player.Play();
            player.Finished += () => player.QueueFree();
        }
    }

    public static void Play(Creature creature, string folder, string soundName, float volume = 0f)
    {
        Play(folder, soundName, volume);
    }

    public static void PlaySfx(string soundName, float volume = 0f, float pitchVariation = 0f)
    {
        Play("", soundName, volume, pitchVariation);
    }

    public static void PlayGlobalSfx(string soundName, float volume = 0f)
    {
        var stream = GetOrLoadStream("", soundName);
        if (stream == null) return;

        var player = new AudioStreamPlayer
        {
            Stream = stream,
            VolumeDb = volume + SfxVolumeOffset,
            Bus = new StringName("SFX")
        };

        var mainLoop = Engine.GetMainLoop();
        var sceneTree = mainLoop as SceneTree;
        var root = sceneTree?.Root;
        if (root != null)
        {
            ((Node)root).AddChild(player);
            player.Play();
            player.Finished += () => player.QueueFree();
        }
    }

    private static AudioStream? GetOrLoadStream(string folder, string soundName)
    {
        string key = string.IsNullOrEmpty(folder) ? soundName : $"{folder}/{soundName}";
        if (CachedStreams.TryGetValue(key, out var cached))
            return cached;

        string path = string.IsNullOrEmpty(folder)
            ? $"{ModPath}/audio/sfx/{soundName}.ogg"
            : $"{ModPath}/audio/sfx/{folder}/{soundName}.ogg";

        var stream = GD.Load<AudioStream>(path);
        if (stream != null)
            CachedStreams[key] = stream;

        return stream;
    }

    public static void PlayMusic(string[] musicOptions, float volumeDbOffset = 0f)
    {
        if (musicOptions == null || musicOptions.Length == 0) return;

        string chosen = musicOptions[GD.RandRange(0, musicOptions.Length - 1)];
        string path = $"{ModPath}/audio/bgm/{chosen}.ogg";

        if (_currentMusicPath == path && _musicPlayer is { Playing: true })
            return;

        StopMusic();
        var stream = GD.Load<AudioStream>(path);
        if (stream == null) return;

        if (stream is AudioStreamOggVorbis ogg)
            ogg.Loop = true;

        _musicPlayer = new AudioStreamPlayer
        {
            Stream = stream,
            Bus = new StringName("Master")
        };
        _currentVolumeOffset = volumeDbOffset;
        float volumeBgm = SaveManager.Instance.SettingsSave.VolumeBgm;
        _musicPlayer.VolumeDb = Mathf.LinearToDb(Mathf.Pow(volumeBgm, 2f)) + _currentVolumeOffset + MusicVolumeOffset;

        var run = NRun.Instance;
        if (run != null)
        {
            ((Node)run).AddChild(_musicPlayer);
            _musicPlayer.Play();
            _currentMusicPath = path;
        }
    }

    public static void SetMusicVolume(float volume)
    {
        if (_musicPlayer != null && GodotObject.IsInstanceValid(_musicPlayer))
            _musicPlayer.VolumeDb = Mathf.LinearToDb(Mathf.Pow(volume, 2f)) + _currentVolumeOffset + MusicVolumeOffset;
    }

    public static void FadeIn(string[] musicOptions, float duration = 1f, float volumeDbOffset = 0f)
    {
        if (musicOptions == null || musicOptions.Length == 0) return;

        string chosen = musicOptions[GD.RandRange(0, musicOptions.Length - 1)];
        string path = $"{ModPath}/audio/bgm/{chosen}.ogg";

        if (_currentMusicPath == path && _musicPlayer is { Playing: true })
            return;

        // Crossfade: move current player to outgoing
        if (_musicPlayer != null && GodotObject.IsInstanceValid(_musicPlayer))
        {
            _outgoingFadeTween?.Kill();
            _outgoingPlayer?.QueueFree();

            _outgoingPlayer = _musicPlayer;
            _outgoingFadeTween = ((Node)_outgoingPlayer).CreateTween();
            _outgoingFadeTween.TweenProperty(_outgoingPlayer, new NodePath("volume_db"), Variant.From(-80f), duration)
                .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
            _outgoingFadeTween.TweenCallback(Callable.From(() =>
            {
                _outgoingPlayer?.QueueFree();
                _outgoingPlayer = null;
            }));
        }

        _fadeTween?.Kill();
        _musicPlayer = null;
        _currentMusicPath = null;

        var stream = GD.Load<AudioStream>(path);
        if (stream == null) return;

        if (stream is AudioStreamOggVorbis ogg)
            ogg.Loop = true;

        _musicPlayer = new AudioStreamPlayer
        {
            Stream = stream,
            Bus = new StringName("Master"),
            VolumeDb = -80f
        };
        _currentVolumeOffset = volumeDbOffset;

        var run = NRun.Instance;
        if (run != null)
        {
            ((Node)run).AddChild(_musicPlayer);
            _musicPlayer.Play();
            _currentMusicPath = path;

            float targetDb = Mathf.LinearToDb(Mathf.Pow(SaveManager.Instance.SettingsSave.VolumeBgm, 2f))
                + _currentVolumeOffset + MusicVolumeOffset;
            _fadeTween = ((Node)_musicPlayer).CreateTween();
            _fadeTween.TweenProperty(_musicPlayer, new NodePath("volume_db"), Variant.From(targetDb), duration)
                .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
        }
    }

    public static void FadeOut(float duration = 1f)
    {
        if (_musicPlayer == null || !GodotObject.IsInstanceValid(_musicPlayer)) return;

        _fadeTween?.Kill();
        _fadeTween = ((Node)_musicPlayer).CreateTween();
        _fadeTween.TweenProperty(_musicPlayer, new NodePath("volume_db"), Variant.From(-80f), duration)
            .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
        _fadeTween.TweenCallback(Callable.From(StopMusicImmediate));
    }

    private static void StopMusicImmediate()
    {
        _fadeTween?.Kill();
        _fadeTween = null;
        _outgoingFadeTween?.Kill();
        _outgoingFadeTween = null;

        if (_musicPlayer != null && GodotObject.IsInstanceValid(_musicPlayer))
        {
            _musicPlayer.Stop();
            _musicPlayer.QueueFree();
        }
        _musicPlayer = null;
        _currentMusicPath = null;

        if (_outgoingPlayer != null && GodotObject.IsInstanceValid(_outgoingPlayer))
        {
            _outgoingPlayer.Stop();
            _outgoingPlayer.QueueFree();
        }
        _outgoingPlayer = null;
    }

    public static void StopMusic() => StopMusicImmediate();

    public static bool IsPlayingLegacyMusic() => _musicPlayer is { Playing: true };

    public static void PlayAmbience(string ambienceName, float volumeDbOffset = 0f)
    {
        string path = $"{ModPath}/audio/bgm/{ambienceName}.ogg";

        if (_currentAmbiencePath == path && _ambiencePlayer is { Playing: true })
            return;

        StopAmbience();
        var stream = GD.Load<AudioStream>(path);
        if (stream == null) return;

        if (stream is AudioStreamOggVorbis ogg)
            ogg.Loop = true;

        _ambiencePlayer = new AudioStreamPlayer
        {
            Stream = stream,
            Bus = new StringName("Master")
        };
        float volumeAmbience = SaveManager.Instance.SettingsSave.VolumeAmbience;
        _ambiencePlayer.VolumeDb = Mathf.LinearToDb(Mathf.Pow(volumeAmbience, 2f)) + volumeDbOffset + AmbienceVolumeOffset;

        var run = NRun.Instance;
        if (run != null)
        {
            ((Node)run).AddChild(_ambiencePlayer);
            _ambiencePlayer.Play();
            _currentAmbiencePath = path;
        }
    }

    public static void FadeInAmbience(string ambienceName, float duration = 1f, float volumeDbOffset = 0f)
    {
        string path = $"{ModPath}/audio/bgm/{ambienceName}.ogg";

        if (_currentAmbiencePath == path && _ambiencePlayer is { Playing: true })
            return;

        StopAmbience();
        var stream = GD.Load<AudioStream>(path);
        if (stream == null) return;

        if (stream is AudioStreamOggVorbis ogg)
            ogg.Loop = true;

        _ambiencePlayer = new AudioStreamPlayer
        {
            Stream = stream,
            Bus = new StringName("Master"),
            VolumeDb = -80f
        };

        var run = NRun.Instance;
        if (run != null)
        {
            ((Node)run).AddChild(_ambiencePlayer);
            _ambiencePlayer.Play();
            _currentAmbiencePath = path;

            float targetDb = Mathf.LinearToDb(Mathf.Pow(SaveManager.Instance.SettingsSave.VolumeAmbience, 2f))
                + volumeDbOffset + AmbienceVolumeOffset;
            _ambienceFadeTween = ((Node)_ambiencePlayer).CreateTween();
            _ambienceFadeTween.TweenProperty(_ambiencePlayer, new NodePath("volume_db"), Variant.From(targetDb), duration)
                .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
        }
    }

    public static void FadeOutAmbience(float duration = 1f)
    {
        if (_ambiencePlayer == null || !GodotObject.IsInstanceValid(_ambiencePlayer)) return;

        _ambienceFadeTween?.Kill();
        _ambienceFadeTween = ((Node)_ambiencePlayer).CreateTween();
        _ambienceFadeTween.TweenProperty(_ambiencePlayer, new NodePath("volume_db"), Variant.From(-80f), duration)
            .SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.In);
        _ambienceFadeTween.TweenCallback(Callable.From(StopAmbience));
    }

    public static void StopAmbience()
    {
        _ambienceFadeTween?.Kill();
        _ambienceFadeTween = null;

        if (_ambiencePlayer != null && GodotObject.IsInstanceValid(_ambiencePlayer))
        {
            _ambiencePlayer.Stop();
            _ambiencePlayer.QueueFree();
        }
        _ambiencePlayer = null;
        _currentAmbiencePath = null;
    }

    public static void SetAmbienceVolume(float volume)
    {
        if (_ambiencePlayer != null && GodotObject.IsInstanceValid(_ambiencePlayer))
            _ambiencePlayer.VolumeDb = Mathf.LinearToDb(Mathf.Pow(volume, 2f)) + AmbienceVolumeOffset;
    }
}
