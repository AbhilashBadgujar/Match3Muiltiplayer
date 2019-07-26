using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PlayerTurn {
    player1, player2, none
}

public class GameManager : MonoBehaviour
{

    //PlayerTwoAI playerTwoAI;
    ScoreManager scoreManager;

    public PlayerTurn playerTurn;
    public int myHealth, EnemyHelth;
    public Slider myHealthSlider, EnemyHealthSlider;
    [SerializeField] GameObject TurnPannel;

    [SerializeField] ParticleSystem p1FireParticle;
    [SerializeField] Animator anim;

    [SerializeField] ParticleSystem EnemyFireParticle;
    [SerializeField] Animator Enemyanim;
    string winnerName;
    public bool gameEnd = false;

    [SerializeField] GameObject board, GameOverPannel, GameOverObjects;
    [SerializeField] Text WinnerText;

    // Start is called before the first frame update
    void Start()
    {
        playerTurn = PlayerTurn.none;
        //playerTwoAI = FindObjectOfType<PlayerTwoAI>();
        scoreManager = FindObjectOfType<ScoreManager>();
        myHealthSlider.maxValue = myHealth;
        EnemyHealthSlider.maxValue = EnemyHelth;
        myHealthSlider.value = myHealth;
        EnemyHealthSlider.value = EnemyHelth;

    }

    public void SetTurnPanel(bool isTurn) {
        TurnPannel.SetActive(isTurn);
    }

    private void Update() {
        SetSliderValue();
       

        EndGameCheck();
        if (gameEnd) {

            StartCoroutine(GameOver());
        }
    }

   

    public void SetSliderValue() {
        myHealthSlider.value = (float) myHealth;
        EnemyHealthSlider.value = (float) EnemyHelth;
    }

    public void PlayerEffects() {
        p1FireParticle.Play();
        anim.SetTrigger("attack");
    }

    public void EnemyEffects() {
        EnemyFireParticle.Play();
        Enemyanim.SetTrigger("attack");
    }

    void EndGameCheck() {
        if (myHealth <= 0 || EnemyHelth <= 0) {
            gameEnd = true;
        }
    }

    private string EndGameNameCheck() {
        if (myHealth <= 0) {
            return "You lose";
        }else if( EnemyHelth <= 0) {
            return "you Won";
        }
        return " ";
    }

    IEnumerator GameOver() {
        board.SetActive(false);
        WinnerText.text = EndGameNameCheck();
        GameOverPannel.SetActive(true);
        yield return new WaitForSeconds(1f);
        GameOverObjects.SetActive(true);

    }


    public void Retry() {
        Application.Quit();
    }






}
