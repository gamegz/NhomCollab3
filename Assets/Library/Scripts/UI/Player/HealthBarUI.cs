using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Library.Scripts.UI.Player
{
    public class HealthBarUI : MonoBehaviour
    {
        public GameObject healthIcon;
        public GameObject overHealHealthIcon;
        [Space] 
        public RectTransform healthContainer;
        

        private class HealthIconData
        {
            public GameObject icon;
            public bool isActive;
        }
        private List<HealthIconData> _activeHealthIcon = new();

        private void Awake()
        {
            PlayerBase.HealthModified += UpdateHealthBar;
        }

        private void UpdateHealthBar(float modifiedHealth, float maxHealth, bool? increased)
        {
            var delta = (int)modifiedHealth - _activeHealthIcon.Count;
            if (_activeHealthIcon.Count <= modifiedHealth)
            {
                for (int i = 0; i < delta; i++)
                {
                    if (_activeHealthIcon.ElementAtOrDefault(i) != null) continue;
                    var inst = Instantiate(healthIcon, healthContainer);
                    _activeHealthIcon.Add(new HealthIconData {icon = inst, isActive = true});
                }
            }
            else
            {
                for (int i = 0; i < _activeHealthIcon.Count; i++)
                {
                    if (i < modifiedHealth)
                    {
                        _activeHealthIcon[i].isActive = true;
                        _activeHealthIcon[i].icon.SetActive(true);
                        continue;
                    }
                    
                    _activeHealthIcon[i].isActive = false;
                    _activeHealthIcon[i].icon.SetActive(false);
                }
            }
        }
    }
}