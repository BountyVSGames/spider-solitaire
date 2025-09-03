using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Manager;

namespace Game.MouseInput
{
    public class MouseInput : MonoBehaviour
    {
        [SerializeField] private Color m_hoverCardSpriteColor;
        [SerializeField] private SpriteRenderer m_hoverCardSpriteRenderer;
        [SerializeField] private List<Card> m_selectedCardObjects;

        private CardManager m_cardManager;

        private int m_test;
        private void Start()
        {
            m_cardManager = FindFirstObjectByType<CardManager>();
        }
        private void Update()
        {
            RayCast();
        }

        private void RayCast()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(mousePosition, Vector3.zero);

            if(hit2D && hit2D.collider.transform.GetComponentsInChildren<SpriteRenderer>().Length > 0)
            {
                Color tempColour;

                if(m_hoverCardSpriteRenderer != null)
                {
                    m_hoverCardSpriteRenderer.color = m_hoverCardSpriteColor;
                }

                m_hoverCardSpriteRenderer = hit2D.collider.transform.GetComponentsInChildren<SpriteRenderer>()[0];

                m_hoverCardSpriteColor = m_hoverCardSpriteRenderer.color;
                tempColour = m_hoverCardSpriteRenderer.color;
                tempColour.a = .25f;
                m_hoverCardSpriteRenderer.color = tempColour;
            }
            else if(m_hoverCardSpriteRenderer != null)
            {
                m_hoverCardSpriteRenderer.color = m_hoverCardSpriteColor;

                m_hoverCardSpriteRenderer = null;
            }

            if (Input.GetMouseButtonDown(0) && hit2D)
            {
                if (hit2D.collider.GetComponent<RemainingCardSpawn>())
                {
                    hit2D.collider.GetComponent<RemainingCardSpawn>().OnClick();
                    return;
                }

                if (m_selectedCardObjects.Count == 0 && hit2D.collider.GetComponent<Card>() != null && hit2D.collider.GetComponent<Card>().CardShown)
                {
                    Card currentSelectedCard = hit2D.collider.gameObject.GetComponent<Card>();

                    if (currentSelectedCard.GetCurrentHolder != null && currentSelectedCard.GetCurrentHolder.IndexOfObjectInHolder(currentSelectedCard) < (currentSelectedCard.GetCurrentHolder.GetHolderObject.Count - 1))
                    {
                        m_selectedCardObjects = currentSelectedCard.GetCurrentHolder.GetCardMatch(currentSelectedCard.GetCurrentHolder.IndexOfObjectInHolder(currentSelectedCard));
                        m_selectedCardObjects[0].SelectCard(true);

                        return;
                    }

                    m_selectedCardObjects.Add(hit2D.collider.gameObject.GetComponent<Card>());
                    m_selectedCardObjects[0].SelectCard(true);
                    return;
                }
                else if(m_selectedCardObjects.Count > 0)
                {
                    CardHolder originalCardHolder = m_selectedCardObjects[0].GetCurrentHolder;

                    if (hit2D.collider.GetComponent<CardHolder>() != null)
                    {
                        if (hit2D.collider.GetComponent<CardHolder>().DoesTheCardFit(m_selectedCardObjects[0]) && (m_selectedCardObjects[0].GetCurrentHolder == null || m_selectedCardObjects[0].GetCurrentHolder != hit2D.collider.GetComponent<CardHolder>()))
                        {
                            for (int i = 0; i < m_selectedCardObjects.Count; i++)
                            {
                                m_selectedCardObjects[i].SetCardHolder(hit2D.collider.GetComponent<CardHolder>());
                            }

                            originalCardHolder.FinishMove();
                            hit2D.collider.GetComponent<CardHolder>().FinishMove();
                            m_cardManager.SetNumberOfMoves(m_cardManager.NumberOfMoves + 1);
                        }
                    }
                    else if(hit2D.collider.GetComponent<Card>() != null)
                    {
                        if (hit2D.collider.GetComponent<Card>().GetCurrentHolder != null && hit2D.collider.GetComponent<Card>().GetCurrentHolder.DoesTheCardFit(m_selectedCardObjects[0]) && m_selectedCardObjects[0].GetCurrentHolder != hit2D.collider.GetComponent<Card>().GetCurrentHolder)
                        {
                            for (int i = 0; i < m_selectedCardObjects.Count; i++)
                            {
                                m_selectedCardObjects[i].SetCardHolder(hit2D.collider.GetComponent<Card>().GetCurrentHolder);
                            }

                            originalCardHolder.FinishMove();
                            hit2D.collider.GetComponent<Card>().GetCurrentHolder.FinishMove();
                            m_cardManager.SetNumberOfMoves(m_cardManager.NumberOfMoves + 1);
                        }
                    }       
                }

                if (m_selectedCardObjects.Count > 0 && m_selectedCardObjects[0] != null)
                {
                    m_selectedCardObjects[0].SelectCard(false);
                }

                m_selectedCardObjects.Clear();
            }
            else if(Input.GetMouseButtonDown(0) && m_selectedCardObjects.Count > 0)
            {
                m_selectedCardObjects[0].SelectCard(false);
                m_selectedCardObjects.Clear();
            }
        }
    }
}