using DG.Tweening;
using Library.Scripts.Audio;
using UnityEngine;

namespace Library.Scenes.AudioManagerExample.ExampleScript
{
    public class AudioManagerExample : MonoBehaviour
    {
        public AudioClip soundEffect;
        [Space]
        public AudioClip music1;
        public AudioClip music2;

        private AudioManager _manager;

        private void Awake()
        {
            _manager = AudioManager.Instance;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _manager.PlaySoundEffect(soundEffect);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _manager.PauseAllSoundEffect(true);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _manager.PauseAllSoundEffect(false);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _manager.StopAllSoundEffect();
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                _manager.PlayMusic(music1);
                // or
                // _manager.PlayMusicFadeIn(music1, 0.8f, Ease.Linear);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                _manager.PauseMusic();
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                _manager.UnpauseMusic();
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                _manager.StopMusic();
                
                // or
                _manager.StopMusicFadeOut(0.8f, Ease.Linear);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                _manager.StopMusic();
                _manager.CrossfadeMusic(music2, 0.8f, Ease.Linear);
            }
        }
    }
}
