
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Library.Scripts.Audio
{
    public class AudioSourceController : MonoBehaviour
    {
        private bool _isPlaying;
        private bool _isFree;
        private bool _isLoop;
        
        private AudioClip _currentClip;
        private AudioSource _audioSource;
        private IEnumerator _playingRoutine;

        public bool IsPlaying => _isPlaying;
        public bool IsFree => _isFree;
        public AudioClip CurrentClip => _currentClip;

        #region Public

        public void Play(AudioClip clip, AudioMixerGroup mixerGroup, bool isLoop = false) {
            ValidateSource();
            
            _currentClip = clip;
            _audioSource.clip = clip;
            _audioSource.outputAudioMixerGroup = mixerGroup;
            _audioSource.loop = isLoop;
            _isLoop = isLoop;
            _audioSource.Play();
            
            _isFree = false;
            _isPlaying = true;

            if (isLoop) return;
            if (_playingRoutine != null) StopCoroutine(_playingRoutine);
            _playingRoutine = PlayingRoutine();
            StartCoroutine(_playingRoutine);
        }

        public void Pause(bool isUnpause) {
            if (isUnpause) _audioSource.UnPause();
            else _audioSource.Pause();
        }
        public void Stop() {
            if (_playingRoutine != null) StopCoroutine(_playingRoutine);
            _currentClip = null;
            _isFree = true;
            _isPlaying = false;
            _audioSource.loop = false;
            _audioSource.Stop();
        }
        
        public void SetLoop(bool isLooping)
        {
            StopCoroutine(_playingRoutine);
            _audioSource.loop = isLooping;
            _isLoop = isLooping;
            _isFree = false;
        }

        public void SetSourceVolume(float vol) {
            if (!_audioSource) return;
            _audioSource.volume = vol;
        }
        
        #endregion

        #region Private
        
        private IEnumerator PlayingRoutine() {
            yield return new WaitForSeconds(_currentClip.length);
            Stop();
        }
        
        private void ValidateSource() {
            if (!_audioSource) _audioSource = GetComponent<AudioSource>();
        }
        
        #endregion
    }
}