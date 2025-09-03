using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Manager;

namespace Game
{
    public class RemainingCardSpawn : MonoBehaviour
    {
        [SerializeField] private int m_cardsToSpawn = 10;

        private CardManager m_cardManager;

        private void Start()
        {
            m_cardManager = FindFirstObjectByType<CardManager>();
        }

        public void OnClick()
        {
            m_cardManager.StartCoroutine(m_cardManager.SpawnCardsOnHolders(m_cardsToSpawn));
            Destroy(this.gameObject);
        }
    }
}
