using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSystem : MonoBehaviour
{

    public float fishingDuration = 10f; // 钓鱼的总时间（秒）

    private float elapsedTime = 0f; // 已经过去的钓鱼时间

    public bool Fishing()
    {
        
        elapsedTime += Time.deltaTime;
        Debug.Log(elapsedTime);
        // 计算钓鱼的概率
        float fishingProbability = elapsedTime / fishingDuration;
        float randomNumber = Random.Range(0.3f, 1f);
        // 如果随机数小于钓鱼的概率，则钓到鱼
        if (randomNumber < fishingProbability)
        {
            Debug.Log("ok" + randomNumber);
            
            return true;
        }

        if (elapsedTime >= fishingDuration)
        { 
            Debug.Log("Fishing time is over!");
            // 如果在钓鱼时间结束时还没有钓到鱼，就强制钓到一条鱼
            return true;
            
        }
        return false;
    }

    private void CatchFish()
    {
        
        Debug.Log("Caught a fish!");
        // 在此处添加钓到鱼后的逻辑
    }


}
