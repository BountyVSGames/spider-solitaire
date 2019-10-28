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

        private bool m_isCompletePackCheckRunning;

        [SerializeField] private bool m_debugActive;

        #region Properties
            public List<Card> GetHolderObject => m_holderObject;
        #endregion

        private void Start()
        {
            if(m_holderObject == null)
                m_holderObject = new List<Card>();

            m_cardManagerScript = FindObjectOfType<CardManager>();

            m_isCompletePackCheckRunning = false;
        }
        private void Update()
        {
            if (m_holderObject.Count > 0 && Camera.main.WorldToScreenPoint(m_holderObject[m_holderObject.Count - 1].transform.position).y < 30 && !m_debugActive)
                ScaleCards(true);
        }

        private void ScaleCards(bool state)
        {
            if(state)
            {
                for (int i = 0; i < m_holderObject.Count; i++)
                {
                    float scaleX = m_holderObject[i].transform.localScale.x * .8f;
                    float scaleY = m_holderObject[i].transform.localScale.y * .75f;
                    m_holderObject[i].transform.localScale = new Vector3(scaleX, scaleY, m_holderObject[i].transform.localScale.z);
                    m_holderObject[i].transform.localPosition = new Vector3(m_holderObject[i].transform.localPosition.x, m_holderObject[i].transform.localPosition.y + .5f, m_holderObject[i].transform.localPosition.z);
                }

                m_debugActive = true;
            }
        }

        private IEnumerator CompletePackCheck()
        {
            bool completedPack = false;
            int kingIndex;
            List<Card> cardSet = new List<Card>();

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
                    m_cardManagerScript.RemoveCard((cardSet[i]));

                    yield return new WaitForSeconds(.005f);
                }
            }

            m_isCompletePackCheckRunning = false;
        }

        public void AddObjectToHolder(Card holderObject)
        {
            m_holderObject.Add(holderObject);

            if(holderObject.CardShown && !m_isCompletePackCheckRunning)
                StartCoroutine(CompletePackCheck());
        }
        public void RemoveObjectFromHolder(Card holderObject)
        {
            int indexOfHolderObject = IndexOfObjectInHolder(holderObject);

            if (indexOfHolderObject > (m_holderObject.Count + 1))
                return;

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
