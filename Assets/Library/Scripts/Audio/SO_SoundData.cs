using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Audio/SoundData")]
public class SO_SoundData : ScriptableObject
{
    public AudioClip PlayButtonSound;
    
    public List<AudioClip> footstepSounds;

    public AudioClip DashSound;
}
