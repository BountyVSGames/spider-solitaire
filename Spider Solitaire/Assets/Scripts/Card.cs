using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;

namespace Game
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private CardType m_cardType;
        [SerializeField] private CardColour m_cardColour;

        [SerializeField] private Sprite m_frontSprite;

        [SerializeField] private bool m_triggerCard;

        private bool m_cardShown;

        #region Properties
        public Sprite GetFrontSprite => m_frontSprite;
        public Sprite SetFrontSprite(Sprite frontSprite) => m_frontSprite = frontSprite;

        public bool CardShown => m_cardShown;
        #endregion

        private void Update()
        {
            if(m_triggerCard)
            {
                ShowCard(!m_cardShown);
                m_triggerCard = false;
            }
        }

        public void ShowCard(bool cardShownState)
        {
            if (cardShownState)
                transform.localEulerAngles = new Vector3(0, 180, 0);
            else if(!cardShownState)
                transform.localEulerAngles = new Vector3(0, 0, 0);

            m_cardShown = cardShownState;
        }
    }
}
