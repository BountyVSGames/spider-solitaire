using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Game.Manager
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_numberOfMovesText;
        [SerializeField] private TextMeshProUGUI m_completedDecksText;
        [SerializeField] private GameObject m_winPanel;

        public void ActivateWinPanel()
        {
            m_winPanel.SetActive(true);
        }
        public void UpdateNumberOfMovesText(int numberOfMoves)
        {
            m_numberOfMovesText.text = "Completed Moves: " + numberOfMoves;
        }
        public void UpdateCompletedDecksText(int numberOfCompletedDecks)
        {
            m_completedDecksText.text = "Completed Decks: " + numberOfCompletedDecks;
        }
        public void RestartGameButtonPressed()
        {
            GameManager.StaticGameManager.RestartCurrenScene();
        }
    }
}
