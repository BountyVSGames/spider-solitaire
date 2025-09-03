using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;
using Game.Manager;

namespace Game
{
    public class CardHolder : MonoBehaviour
    {
        [SerializeField] private List<Card> m_holderObject;

        private CardManager m_cardManagerScript;

        [SerializeField] private bool m_isCompletePackCheckRunning;

        #region Properties
            public List<Card> GetHolderObject => m_holderObject;
        #endregion

        private void Start()
        {
            if(m_holderObject == null)
                m_holderObject = new List<Card>();

            m_cardManagerScript = FindFirstObjectByType<CardManager>();

            m_isCompletePackCheckRunning = false;
        }

        public void FinishMove()
        {
            if (m_holderObject.Count >= 10)
            {
                for (int i = 0; i < m_holderObject.Count; i++)
                {
                    if (!m_holderObject[i].CardScale)
                    {
                        m_holderObject[i].transform.localScale = new Vector3(.8f, .8f, m_holderObject[i].transform.localScale.z);

                        m_holderObject[i].CardScale = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_holderObject.Count; i++)
                {
                    if (m_holderObject[i].CardScale)
                    {
                        m_holderObject[i].transform.localScale = new Vector3(1, 1, m_holderObject[i].transform.localScale.z);
                        m_holderObject[i].CardScale = false;
                    }
                }
            }

            if (!m_isCompletePackCheckRunning)
                StartCoroutine(CompletePackCheck());
        }

        private IEnumerator CompletePackCheck()
        {
            bool completedPack = false;
            int kingIndex;
            List<Card> cardSet = new();

            m_isCompletePackCheckRunning = true;

            if (m_holderObject.Count == 0)
                yield break;

            for (int i = 0; i < m_holderObject.Count; i++)
            {
                if(m_holderObject[i].CardScriptableObject.CardType == CardType.cardKing && m_holderObject[i].CardShown)
                {
                    kingIndex = i;
                    cardSet.Add(m_holderObject[kingIndex]);

                    if (completedPack)
                        break;

                    for (int j = 1; j < (m_holderObject.Count - kingIndex); j++)
                    {
                        while (m_holderObject[j + kingIndex].GetCurrentHolder != this)
                            yield return null;

                        if ((int)m_holderObject[j + kingIndex].CardScriptableObject.CardType == ((int)m_holderObject[kingIndex].CardScriptableObject.CardType) + j)
                            cardSet.Add(m_holderObject[j + kingIndex]);
                        else
                            cardSet.Clear();

                        if(j == ((m_holderObject.Count - kingIndex) - 1))
                        {
                            if(m_holderObject[j + kingIndex].CardScriptableObject.CardType != CardType.cardA)
                            {
                                cardSet.Clear();
                            }
                            else if(m_holderObject[j + kingIndex].CardScriptableObject.CardType == CardType.cardA)
                            {
                                completedPack = true;
                                break;
                            }
                        }
                    }
                }
            }

            if(completedPack)
            {
                m_cardManagerScript.SetCompletedCardDecks(m_cardManagerScript.CompletedCardDecks + 1);

                for (int i = 0; i < cardSet.Count; i++)
                {
                    cardSet[i].GetCurrentHolder.RemoveObjectFromHolder(cardSet[i]);
                    cardSet[i].SelectCard(false);
                    m_cardManagerScript.RemoveCard((cardSet[i]));

                    yield return new WaitForSeconds(.005f);
                }
            }

            m_isCompletePackCheckRunning = false;
        }

        public void AddObjectToHolder(Card holderObject)
        {
            m_holderObject.Add(holderObject);
        }
        public void RemoveObjectFromHolder(Card holderObject)
        {
            int indexOfHolderObject = IndexOfObjectInHolder(holderObject);

            if (indexOfHolderObject > (m_holderObject.Count + 1))
                return;

            if(holderObject.CardScale)
            {
                holderObject.transform.localScale = new Vector3(1, 1, holderObject.transform.localScale.z);
                holderObject.CardScale = false;
            }

            m_holderObject.Remove(m_holderObject[indexOfHolderObject]);

            if ((m_holderObject.Count - 1) >= 0 && !m_holderObject[m_holderObject.Count - 1].CardShown)
                m_holderObject[m_holderObject.Count - 1].ShowCard(true);
        }
        public int IndexOfObjectInHolder(Card holderObject)
        {
            int returnValue = 0;

            for(int i = 0; i < m_holderObject.Count; i++)
            {
                if (m_holderObject[i] == holderObject)
                {
                    returnValue = i;
                    break;
                }
                else if (i == (m_holderObject.Count - 1) && m_holderObject[i] != holderObject)
                {
                    Debug.LogErrorFormat("Object {0} not found in {1} object holders list. Did something go wrong?", holderObject, this.gameObject);
                    returnValue = m_holderObject.Count + 1;
                }
            }

            return returnValue;
        }

        public List<Card> GetCardMatch(int index)
        {
            Card startCard = m_holderObject[index];
            List<Card> returnCardList = new List<Card>();

            returnCardList.Add(startCard);

            for(int i = 1; i < (m_holderObject.Count - index); i++)
            {
                if ((int)m_holderObject[i + index].CardScriptableObject.CardType == ((int)startCard.CardScriptableObject.CardType + i))
                    returnCardList.Add(m_holderObject[i + index]);
                else if(i == ((m_holderObject.Count - index) - 1))
                    returnCardList = new List<Card>();
            }

            if (returnCardList.Count > 0)
                return returnCardList;

            return null;
        }
        
        public bool DoesTheCardFit(Card newHolderObject)
        {
            if (m_holderObject.Count == 0)
                return true;

            if(newHolderObject.CardScriptableObject.CardType == (m_holderObject[m_holderObject.Count - 1].CardScriptableObject.CardType + 1))
                return true;

            return false;
        }
    }
}
