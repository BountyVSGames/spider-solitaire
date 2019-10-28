using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Manager
{
    public class MenuUIManager : MonoBehaviour
    {
        public void LoadGameScene(int sceneIndex)
        {
            GameManager.StaticGameManager.LoadScene(sceneIndex);
        }
    }
}