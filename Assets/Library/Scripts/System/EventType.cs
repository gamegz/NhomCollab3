namespace Core.Events {
    public enum EventType {
        //UI Events
        OnPlayerHPChange,
        OnSickleChargeChange,
        OnAmmoChange,
        OnSpecialAmmoChange,
        OnWeaponListLoaded,
        OnBossDamaged,
        
        OnWeaponAmmoDepleted,
        OnWeaponChange,
        OnWeaponAmmoAdded,
        OnPlayerMove,
        OnStunAnchorExpires,
        
        OnPlayerDamaged,
        OnPlayerDeath,
        OnPlayerRespawn,
        OnSpawnerActivated,
        OnSpawnerDefeated,
        OnCheckpointActivated,
        
        OnEnemyDeath,
        OnAnimationParamUpdate_Dashing,
        OnAnimationParamUpdate_Fire,
        OnAnimationParamUpdate_ThrowSickle,
        OnAnimationParamUpdate_Melee,
        
        OnRootMotion_Recoil,
        OnMeleeEnd,
        OnMeleeStart,
        OnSickleReturn,
        
        OnWeaponLockGetRef,
        OnWeaponLockSetRef,
        OnTutorialWeaponUnlock,
        OnTutorialWeaponUI,
        
        OnBossEntered,
        OnBossDefeated,

        OnGamePause,
        OnGameUnpause,

        OnPickup,

        OnResetGame,
        OnBackToMainMenu,
        OnLevelLoad,
        
        //Gamesate ahh 
        OnGameWin,
        OpenWinUI,

        //Audio events
        OnPlaySoundEffect,
        OnPlaySoundEffect3D,
    }
}