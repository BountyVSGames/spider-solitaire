using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;

namespace Game
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private CardObject m_cardScriptableObject;
        [SerializeField] private SpriteRenderer m_frontCardObject;

        [SerializeField] private Material m_normalMaterial;
        [SerializeField] private Material m_selectedMaterial;

        [SerializeField] private bool m_triggerCard;

        [SerializeField] private bool m_cardScale;

        private CardHolder m_currentHolder;

        private bool m_cardShown;
        private bool m_cardSelectState;
        

        #region Properties
        public CardObject CardScriptableObject => m_cardScriptableObject;

        public CardHolder GetCurrentHolder => m_currentHolder;

        public bool CardShown => m_cardShown;

        public bool CardScale
        {
            get { return m_cardScale; }
            set { m_cardScale = value; }
        }
        #endregion

        private void Start()
        {
            Initialize(m_cardScriptableObject);
        }

        private void Update()
        {
            if (m_triggerCard)
            {
                ShowCard(!m_cardShown);
                m_triggerCard = false;
            }
        }

        public void Initialize(CardObject cardScriptableObject)
        {
            if (cardScriptableObject == null)
            {
                Debug.LogError("CardScripableObject is null. This should not happen. Please give it an index");
                Debug.LogError("Disabling object");

                this.gameObject.SetActive(false);
                return;
            }

            m_frontCardObject.sprite = cardScriptableObject.FrontSprite;
            m_cardScriptableObject = cardScriptableObject;

            this.name = m_cardScriptableObject.name;
        }

        public void ShowCard(bool cardShownState)
        {
            if (!cardShownState)
                transform.localEulerAngles = new Vector3(0, 180, 0);
            else if (cardShownState)
                transform.localEulerAngles = new Vector3(0, 0, 0);

            m_cardShown = cardShownState;
        }
        public void SetCardHolder(CardHolder cardHolder)
        {
            if (m_currentHolder != null)
                m_currentHolder.RemoveObjectFromHolder(this);

            transform.parent = cardHolder.transform;
            transform.localPosition = Vector3.zero;
            transform.localPosition = new Vector3(0, .2f + -(.5f * cardHolder.GetHolderObject.Count), .5f * cardHolder.GetHolderObject.Count);

            cardHolder.AddObjectToHolder(this);

            m_currentHolder = cardHolder;
        }
        public void SelectCard(bool state)
        {
            m_cardSelectState = state;

            if (m_cardSelectState)
                m_frontCardObject.material = m_selectedMaterial;
            else if (!m_cardSelectState)
                m_frontCardObject.material = m_normalMaterial;
        }
    }
}
