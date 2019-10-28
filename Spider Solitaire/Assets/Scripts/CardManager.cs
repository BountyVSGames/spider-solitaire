using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Manager
{
    public class CardManager : MonoBehaviour
    {
        [SerializeField] private CardObject[] m_cardSet;
        [SerializeField] private CardHolder[] m_cardHolderObjects;
        [SerializeField] private GameObject m_mainCardHolderObject;
        [SerializeField] private GameObject m_cardPrefab;
        [SerializeField] private int m_totalSetsNeededToWin;
        [SerializeField] private int m_cardToSpawn;
        [SerializeField] private int m_shuffleCounts;

        private int m_completedCardDecks;
        private int m_numberOfMoves;

        private List<Card> m_allCards;
        private UIManager m_uiManagerScript;

        #region Properties
            public int CompletedCardDecks => m_completedCardDecks;
            public int NumberOfMoves => m_numberOfMoves;
            public void SetCompletedCardDecks() => m_completedCardDecks += 1;
        #endregion

        private void Awake()
        {
            int index = 0;

            m_allCards = new List<Card>();

            for (int i = 0; i < m_totalSetsNeededToWin; i++)
            {
                for (int j = 0; j < m_cardSet.Length; j++)
                {
                    GameObject newCard = Instantiate(m_cardPrefab, transform.position, transform.rotation);
                    Card cardObject = newCard.GetComponent<Card>();

                    m_allCards.Add(cardObject);

                    cardObject.transform.parent = m_mainCardHolderObject.transform;
                    cardObject.transform.position = Vector3.zero;

                    cardObject.Initialize(m_cardSet[j]);;

                    cardObject.gameObject.SetActive(false);

                    index++;
                }
            }
            for(int i = 0; i < m_shuffleCounts; i ++)
            {
                for (int j = 0; j < m_allCards.Count; j++)
                {
                    Card oldCardFromSelectedIndex = null;
                    int randomIndex = j;
                    int test = 0;

                    while (randomIndex == j && test < 2)
                    {
                        randomIndex = Random.Range(0, m_allCards.Count);
                    }

                    oldCardFromSelectedIndex = m_allCards[randomIndex];
                    m_allCards[randomIndex] = m_allCards[j];
                    m_allCards[j] = oldCardFromSelectedIndex;
                }
            }
        }
        private void Start()
        {
            m_uiManagerScript = FindObjectOfType<UIManager>();

            StartCoroutine(SpawnCardsOnHolders(m_cardToSpawn));
        }

        public void RemoveCard(Card card)
        {
            for (int i = 0; i < m_allCards.Count; i++)
            {
                if(m_allCards[i] == card)
                {
                    card = m_allCards[i];

                    m_allCards.Remove(card);
                    Destroy(card.gameObject);
                }
            }
        }
        public void SetCompletedCardDecks(int newCompletedCardDecksNumber)
        {
            m_completedCardDecks = newCompletedCardDecksNumber;
            m_uiManagerScript.UpdateCompletedDecksText(newCompletedCardDecksNumber);

            if (m_completedCardDecks == m_totalSetsNeededToWin)
                m_uiManagerScript.ActivateWinPanel();
        }
        public void SetNumberOfMoves(int newNumberOfMoves)
        {
            m_numberOfMoves = newNumberOfMoves;
            m_uiManagerScript.UpdateNumberOfMovesText(m_numberOfMoves);
        }

        private Card GetCardToSpawn()
        {
            for (int i = 0; i < m_allCards.Count; i++)
            {
                if (!m_allCards[i].gameObject.activeInHierarchy)
                    return m_allCards[i];
            }

            Debug.LogError("No card left. Returning null");
            return null;
        }

        public IEnumerator SpawnCardsOnHolders(int totalCardsToSpawn)
        {
            int row = 0;

            for (int i = 0; i < totalCardsToSpawn; i++)
            {
                Card cardThatIsBeingSet = GetCardToSpawn();

                cardThatIsBeingSet.SetCardHolder(m_cardHolderObjects[row]);
                cardThatIsBeingSet.gameObject.SetActive(true);

                if (i >= (totalCardsToSpawn - m_cardHolderObjects.Length))
                    cardThatIsBeingSet.ShowCard(true);
                else
                    cardThatIsBeingSet.ShowCard(false);

                if (row + 1 < m_cardHolderObjects.Length)
                    row++;
                else
                    row = 0;

                yield return new WaitForSeconds(.01f);
            }
        }
    }
}
