using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerTwoAI : MonoBehaviour
{

    GameManager gameManager;
    [SerializeField] Animator anim;
    [SerializeField] ParticleSystem FireParticle;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

    }

    public IEnumerator Player2Turn() {

        int randomDmgSelector = 0;
        randomDmgSelector = Random.Range(0, 4);
        yield return new WaitForSeconds(2f);
        switch (randomDmgSelector) {
            case 0:
                gameManager.myHealth -= 100;
                break;
            case 1:
                gameManager.myHealth -= 30;
                break;
            case 2:
                gameManager.myHealth -= 120;
                break;
            case 3:
                gameManager.myHealth -= 80;
                break;

            case 4:
                gameManager.myHealth -= 60;
                break;
            default:
                gameManager.myHealth -= 30;
                break;
        }
        StartCoroutine(ChangeTurnCO());
        
        

    }

    public IEnumerator ChangeTurnCO() {
        anim.SetTrigger("attack");
        FireParticle.Play();
        yield return new WaitForSeconds(1.5f);
        gameManager.playerTurn = PlayerTurn.player1;
    }
        
}
