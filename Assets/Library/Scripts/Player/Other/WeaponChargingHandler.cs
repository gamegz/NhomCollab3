using Core.Events;
using UnityEngine;
using EventType = Core.Events.EventType;

namespace Library.Scripts.Player.Other
{
    public struct UIChargingData
    {
        public bool state;
        public float currentValue;
        public float maxValue;
    }
    
    [RequireComponent(typeof(WeaponManager))]
    // this to handle Attack Charge / Recovery Charge numbers, as a middle man script so it doesn't fuck over the WeaponManager script
    public class WeaponChargingHandler : MonoBehaviour
    {
        public WeaponManager weaponManager;
        public PlayerWorldPositionReference playerPositionReference;

        private void Awake()
        {
            if (weaponManager)
            {
                WeaponManager.OnHoldChargeATK += OnHoldingAttack;
                WeaponManager.OnCoolDownState += OnCooldown;
            }
        }

        private void Start()
        {
            this.FireEvent(EventType.UISendPositionReference, playerPositionReference);
        }

        private void OnDestroy()
        {
            if (weaponManager)
            {
                WeaponManager.OnHoldChargeATK -= OnHoldingAttack;
                WeaponManager.OnCoolDownState -= OnCooldown;
            } 
        }

        private void OnCooldown(bool isCoolingDown, float currentRecoverTime, float maxRecoverTime)
        {
            this.FireEvent(EventType.UIOnAttackCooldown, new UIChargingData
            {
                state = isCoolingDown,
                currentValue = currentRecoverTime,
                maxValue = maxRecoverTime
            });
        }

        private void OnHoldingAttack(bool isHolding, float currentChargeTime, float maxChargeTime)
        {
            this.FireEvent(EventType.UIOnAttackCharge, new UIChargingData
            {
                state = isHolding,
                currentValue = currentChargeTime,
                maxValue = maxChargeTime
            });
        }

        private void OnValidate()
        {
            if (!weaponManager) weaponManager = GetComponent<WeaponManager>();
        }
        
    }
}
