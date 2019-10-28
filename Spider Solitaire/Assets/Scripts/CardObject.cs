using System.Collections;
using System.Collections.Generic;
using Game.Enums;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "new Card", menuName = "Card", order = 0)]
    public class CardObject : ScriptableObject
    {
        [SerializeField] private CardType m_cardType;
        [SerializeField] private CardColour m_cardColour;

        [SerializeField] private Sprite m_frontSprite;

        #region Properties
        public CardType CardType => m_cardType;
        public CardColour CardColour => m_cardColour;

        public Sprite FrontSprite => m_frontSprite;
        #endregion
    }
}
