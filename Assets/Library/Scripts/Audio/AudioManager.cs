using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace Library.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        #region Singleton Custom
        
        private static AudioManager _instance;
        public static AudioManager Instance {
            get {
                if (_instance != null) return _instance;
                _instance = FindAnyObjectByType<AudioManager>();
                if (_instance != null) return _instance;
                
				// change path if needed
                var loaded = Resources.Load("AudioManager") as GameObject;
                var inst = Instantiate(loaded);
                inst.transform.position = Vector3.zero;
                _instance = inst.GetComponent<AudioManager>();
                
                DontDestroyOnLoad(_instance);
                
                return _instance;
            }
        }  
        
        #endregion
        
        public enum AudioType {
            Master,
            BGM,
            FX
        }
        
        [Header("Audio Mixer")]
        public AudioMixer mainMixer;
        
        [Header("Containers")] 
        public Transform bgmContainer;
        public Transform fxContainer;
        
        [Header("Mixer Groups")] 
        public AudioMixerGroup bgmMixer;
        public AudioMixerGroup sfxMixer;

        [Header("Info (Don't Edit)")] 
        public List<AudioSourceController> bgmAudioController = new();
        public List<AudioSourceController> fxAudioController = new();
        
        private AudioSourceController _currentMusicController;
        private AudioSourceController _tempTransitionController;
        
        private Tween _fadeInTween;
        private Tween _fadeOutTween;
        private Sequence _crossfadeSequence;
        
         // Adjust if needed
        private readonly Dictionary<AudioType, string> _volumeParam = new() {
            { AudioType.Master, "VolumeMain" },
            { AudioType.BGM, "VolumeBGM" },
            { AudioType.FX, "VolumeFX" },
        };
        
        #region Music Methods

        /// <summary>
        /// Play music via Audioclip
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="isLoop"></param>
        public void PlayMusic(AudioClip clip, bool isLoop = true) {
            _tempTransitionController?.Stop();
            _tempTransitionController = null;
            
            if (_currentMusicController) {
                _currentMusicController.Stop();
                _currentMusicController = null;
            }

            var availableSource = GetAvailableSource(ref bgmAudioController, AudioType.BGM);
            if (availableSource) {
                _currentMusicController = availableSource;
                _currentMusicController.Play(clip, bgmMixer, isLoop);
                _currentMusicController.SetSourceVolume(1);
            }
            else {
                Debug.LogError("Cannot find a valid AudioSourceController to play music");
            }
        }
        
        /// <summary>
        /// Play music via AudioClip, with fade-in time
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="fadeInTime"></param>
        /// <param name="easing">Ease curve for fade in time</param>
        /// <param name="isLoop"></param>
        public void PlayMusicFadeIn(AudioClip clip, float fadeInTime, Ease easing, bool isLoop = true) {
            _tempTransitionController?.Stop();
            _tempTransitionController = null;
            
            _fadeInTween?.Kill();
            PlayMusic(clip, isLoop);
            _fadeInTween = DOVirtual.Float(0f, 1f, fadeInTime, value => {
                _currentMusicController?.SetSourceVolume(value);
            }).SetEase(easing);
        }

        /// <summary>
        /// Pause music
        /// </summary>
        /// <param name="onPause">Additional action on pause</param>
        public void PauseMusic(Action onPause = null) {
            onPause?.Invoke();
            _crossfadeSequence?.Pause();
            _currentMusicController?.Pause(false);
            _tempTransitionController?.Pause(false);
        }

        /// <summary>
        /// Unpause music
        /// </summary>
        /// <param name="onUnpause">Additional action on unpause</param>
        public void UnpauseMusic(Action onUnpause = null) {
            onUnpause?.Invoke();
            _crossfadeSequence?.Play();
            _currentMusicController?.Pause(true);
            _tempTransitionController?.Pause(true);
        }

        /// <summary>
        /// Stop Music
        /// </summary>
        public void StopMusic() {
            _currentMusicController?.Stop();
            _currentMusicController = null;
            
            _tempTransitionController?.Stop();
            _tempTransitionController = null;
        }
        
        /// <summary>
        /// Stop Music with fade-out time
        /// </summary>
        /// <param name="fadeOutTime"></param>
        /// <param name="easing"></param>
        public void StopMusicFadeOut(float fadeOutTime, Ease easing) {
            if (!_currentMusicController) return;
            if (!_currentMusicController.IsPlaying) return;
            _fadeOutTween?.Kill();
            _fadeOutTween = DOVirtual.Float(1f, 0f, fadeOutTime, value => {
                _currentMusicController?.SetSourceVolume(value);
            }).SetEase(easing);
        }

        /// <summary>
        /// Transition music using cross-fade
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="crossfadeTime"></param>
        /// <param name="easing"></param>
        /// <param name="isLoop"></param>
        public void CrossfadeMusic(AudioClip clip, float crossfadeTime, Ease easing, bool isLoop = true) {
            if (_currentMusicController && _currentMusicController.IsPlaying) {
                var inst = GetAvailableSource(ref bgmAudioController, AudioType.BGM);
                if (inst) {
                    _tempTransitionController = inst;
                    _tempTransitionController.Play(clip, bgmMixer, isLoop);
                    _tempTransitionController.SetSourceVolume(0);
                    
                    _crossfadeSequence?.Kill();
                    _crossfadeSequence = DOTween.Sequence();

                    _crossfadeSequence
                        .Append(DOVirtual.Float(0f, 1f, crossfadeTime, value => {
                            _tempTransitionController.SetSourceVolume(value);
                        }).SetEase(Ease.Linear))
                        .Insert(0, DOVirtual.Float(1f, 0f, crossfadeTime, value => {
                            _currentMusicController.SetSourceVolume(value);
                        }).SetEase(Ease.Linear))
                        .AppendCallback(() => {
                            _currentMusicController = inst;
                            _tempTransitionController = null;
                        });
                }
                else {
                    Debug.LogError("Cannot find a valid AudioSourceController to crossfade"); 
                }
            }
            else {
                PlayMusicFadeIn(clip, crossfadeTime, easing, isLoop);
            }
        }

        #endregion

        #region Sound Effect
        
        /// <summary>
        /// Play sound effect by clip
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        public AudioSourceController PlaySoundEffect(AudioClip clip) {
            var availableSource = GetAvailableSource(ref fxAudioController, AudioType.FX);
            if (availableSource) {
                availableSource.Play(clip, sfxMixer);
            }
            else {
                Debug.LogError("Cannot find a valid AudioSourceController to play sound effect");
            }

            return availableSource;
        }

        /// <summary>
        /// Pause all current sound effect
        /// </summary>
        /// <param name="isUnpause"></param>
        public void PauseAllSoundEffect(bool isUnpause) {
            if (fxAudioController.Count <= 0) return;
            foreach (var controller in fxAudioController) {
                controller.Pause(isUnpause);
            }
        }

        /// <summary>
        /// Stop all sound effect
        /// </summary>
        public void StopAllSoundEffect() {
            if (fxAudioController.Count <= 0) return;
            foreach (var controller in fxAudioController) {
                controller.Stop();
            }
        }
        
        #endregion

        #region Exposed Param
        
        /// <summary>
        /// Set Channel Volume, using preset name as the same as the enum
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="isValueLinear">Is value in (0 - 1) or (-80db - 0db)</param>
        public void SetChannelVolume(AudioType type, float value, bool isValueLinear) {
            if (!_volumeParam.TryGetValue(type, out var targetParam)) {
                Debug.LogError($"Cannot find param of type {type.ToString()}");
                return;
            }
            
            SetExposedParam(targetParam, isValueLinear ? value.LinearToDecibel() : value);            
        }

        /// <summary>
        /// Set AudioMixer's exposed param via string name.
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        public void SetExposedParam(string paramName, float value) {
            if (!mainMixer.SetFloat(paramName, value)) {
                Debug.LogError($"Cannot set exposed param value of {paramName}");
            }
        }

        /// <summary>
        /// Get Channel Volume, using preset name as the same as the enum
        /// </summary>
        /// <param name="type"></param>
        /// <param name="convertValueLinear"></param>
        /// <returns></returns>
        public float? GetChannelVolume(AudioType type, bool convertValueLinear) {
            if (_volumeParam.TryGetValue(type, out var targetParam)) return GetExposedParam(targetParam);
            
            Debug.LogError($"Cannot find param of type {type.ToString()}");
            return null;

        }

        /// <summary>
        /// Set AudioMixer's exposed param via string name.
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public float? GetExposedParam(string paramName) {
            if (mainMixer.GetFloat(paramName, out var val)) {
                return val;
            }

            return null;
        }
        
        #endregion
        
        #region Internal Don't Call Anywhere

        private AudioSourceController GetAvailableSource(ref List<AudioSourceController> sources, AudioType type) {
            AudioSourceController availableSource;
            
            if (sources.Count <= 0) {
                var inst = InternalCreateSource(type);
                availableSource = inst;
                sources.Add(inst);
            }
            
            else {
                var toFind = sources.Find(x => x.IsFree);
                if (!toFind) {
                    var inst = InternalCreateSource(type);
                    availableSource = inst;
                    sources.Add(inst);
                }
                else {
                    availableSource = toFind;
                }
            }

            return availableSource;
        }
        
        private AudioSourceController InternalCreateSource(AudioType type) {
            var inst = new GameObject(type + " Source", typeof(AudioSource), 
                typeof(AudioSourceController));
            
            inst.transform.SetParent(type switch {
                AudioType.BGM => bgmContainer,
                AudioType.FX => fxContainer,
                _ => fxContainer
            });

            return inst.GetComponent<AudioSourceController>();
        }

        #endregion
    }
}