using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public int score;

    
     public void IncreaseScore(int ammountToIncrease) {
        score += ammountToIncrease;
    }
}
