using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundData", menuName = "Audio/SoundData")]
public class SO_SoundData : ScriptableObject
{
    [Header("PlayButton")]
    public AudioClip PlayButtonSound;
    
    [Space(20)]
    [Header("Player")]
    public List<AudioClip> attackSound;
    public List<AudioClip> chargeAttackSound;
    
    [Space(20)]
    public List<AudioClip> parrySuccess;
    public AudioClip parryInnit;
    
    [Space(20)]
    public AudioClip DashSound;
    
    [Space(20)]
    public AudioClip PlayerHurtSound;
    
    [Space(20)]
    public AudioClip PlayerHealSound;
    
    [Space(20)]
    public List<AudioClip> PickupSound;
    public AudioClip LevelUpSound;
}
