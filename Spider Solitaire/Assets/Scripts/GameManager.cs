using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Manager
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager m_staticGameManager;

        #region Properties
        public static GameManager StaticGameManager => m_staticGameManager;
        #endregion

        private void Awake()
        {
            if(m_staticGameManager == null)
            {
                m_staticGameManager = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void RestartCurrenScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
