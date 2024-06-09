using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class StartFishGame : MonoBehaviour
{
    private GameObject rod;
    public bool GameStart;
    
    private void Start()
    {
        // 开始时隐藏对象
        rod = GameObject.FindGameObjectWithTag("Start");
        GameStart = true;
        rod.SetActive(true);
        
    }
    public void StartGame()
    {
        GameStart = true;
        rod.SetActive(true);
    }
    public void EndGame()
    {
        GameStart = false;
        rod.SetActive(false);
    }

}
