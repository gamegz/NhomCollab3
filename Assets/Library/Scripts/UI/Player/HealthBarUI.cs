using System.Collections.Generic;
using System.Linq;
using Library.Scripts.UI.Elements;
using UnityEngine;

namespace Library.Scripts.UI.Player
{
    public class HealthBarUI : MonoBehaviour
    {
        public UIHealthBarIndicator healthIcon;
        public UIHealthBarIndicator overHealHealthIcon;
        [Space] 
        public RectTransform healthContainer;
        

        private class HealthIconData
        {
            public GameObject icon;
            public bool isActive;
        }
        private readonly List<UIHealthBarIndicator> _activeHealthIcon = new();
        private UIHealthBarIndicator _currentOverHealIcon;

        private void Awake()
        {
            PlayerBase.HealthModified += UpdateHealthBar;
            PlayerBase.HBOverheal += UpdateOverHealBar;
        }

        private void OnDestroy()
        {
            PlayerBase.HealthModified -= UpdateHealthBar;
            PlayerBase.HBOverheal -= UpdateOverHealBar;
        }

        private void UpdateHealthBar(float modifiedHealth, float maxHealth, bool? increased)
        {
            var delta = (int)modifiedHealth - _activeHealthIcon.Count;
            if (_activeHealthIcon.Count < modifiedHealth)
            {
                for (int i = 0; i < delta; i++)
                {
                    var inst = Instantiate(healthIcon, healthContainer);
                    _activeHealthIcon.Add(inst);
                    inst.Disable(false);
                }

                foreach (var element in _activeHealthIcon)
                {
                    element.Enable(true);
                }
            }
            else
            {
                for (int i = 0; i < _activeHealthIcon.Count; i++)
                {
                    var element = _activeHealthIcon[i];
                    
                    if (i < modifiedHealth)
                    {
                        if (!element.isActive) element.Enable(true);
                        continue;
                    }
                    
                    if (element.isActive) element.Disable(true);
                }
            }
        }
        
        private void UpdateOverHealBar(bool isOverhealing)
        {
            if (!_currentOverHealIcon)
            {
                var inst = Instantiate(overHealHealthIcon, healthContainer);
                _currentOverHealIcon = inst;
                _currentOverHealIcon.Disable(false);
            }

            _currentOverHealIcon.transform.SetSiblingIndex(healthContainer.childCount);
            if (isOverhealing) _currentOverHealIcon.Enable(true);
            else _currentOverHealIcon.Disable(true);
        }
    }
}