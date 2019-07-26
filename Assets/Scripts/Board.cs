using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    wait,
    move
}

public class Board : MonoBehaviour
{

    FindMatches findMatches;
    ScoreManager scoreManager;
    GameManager gameManager;
    public int basePiceVal;
    int streak = 1;

    [SerializeField] GameObject destroyEffect;

    public GameState currentGameState = GameState.wait;
    public int width;
    public int height;
    public int offset;
    public GameObject tilePrefab;
    TileBackground[,] allTiles;
    public GameObject[] dots;
    public GameObject[,] allDots;
    public bool attacked;
    int[] _gemval;
    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        scoreManager = FindObjectOfType<ScoreManager>();
        gameManager = FindObjectOfType<GameManager>();
        allTiles = new TileBackground[width, height];
        allDots = new GameObject[width, height];
        
        //Setup();
    }

    
    public void Setup(int[] gemval) {
        width = JsonPar.jsonPar.inData.coloum;
        int randomnum = 0;
        _gemval = gemval;
        int gemNumber = 0;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                Vector2 tempPos = new Vector2(i, j + offset);
                // GameObject backgroundTile = Instantiate(tilePrefab,tempPos, Quaternion.identity) as GameObject;
                // backgroundTile.transform.parent = transform;
                // backgroundTile.name = "( " + i + "," + j + " )" ;
                int dotToUse = gemval[gemNumber];
                int maxIterations = 0;
                while (MatchedAT(i, j, dots[dotToUse]) && maxIterations < 50) {
                    
                    dotToUse = gemval[randomnum];
                    maxIterations++;
                    randomnum++;
                    if (randomnum >= 36)
                    {
                        randomnum = 0; 
                    }
                }
                maxIterations = 0;
                GameObject dot = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                gemNumber++;
                dot.GetComponent<Dot>().row = j; 
                dot.GetComponent<Dot>().column = i; 
                dot.transform.parent = transform;
                dot.name =  i + "" + j;
                allDots[i, j] = dot;

                
            }
        }
    }

    bool MatchedAT(int column, int row, GameObject pice) {
        if (column > 1 && row > 1) {
            if (allDots[column -1, row].tag == pice.tag && allDots[column - 2, row].tag == pice.tag) {
                return true;
            }
            if (allDots[column, row - 1].tag == pice.tag && allDots[column, row - 2].tag == pice.tag) {
                return true;
            }
        } else if(column <= 1 || row <= 1) {
            if (row > 1) {
                if (allDots[column, row -1].tag == pice.tag && allDots[column, row -2].tag == pice.tag) {
                    return true;
                }
            }
            if (column > 1) {
                if (allDots[column -1, row].tag == pice.tag && allDots[column -2, row ].tag == pice.tag) {
                    return true;
                }
            }
        }
        
        return false;
    }
   
    void DestroyMatchesAt(int column, int row) {
        if (allDots[column, row].GetComponent<Dot>().isMatched) {
            findMatches.currentMatch.Remove(allDots[column, row]);
//            Instantiate(destroyEffect, allDots[column,row].transform.position, Quaternion.identity);
            Destroy(allDots[column, row]);
            scoreManager.IncreaseScore(allDots[column, row].GetComponent<Dot>().dotValue * streak);
            print(scoreManager.score);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i,j] != null) {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCO());
    }

    IEnumerator DecreaseRowCO() {
        int nullcount = 0;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i,j] == null) {
                    nullcount++;
                } else if (nullcount >0) {
                    allDots[i, j].GetComponent<Dot>().row -= nullcount;
                    

                    allDots[i, j] = null;
                }
            }
            nullcount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    void RefillBoard() {
        int randnum = 5;
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i,j] == null) {
                    Vector2 tempPos = new Vector2(i, j + offset);
                    
                    int dotToUse = _gemval[randnum];
                    randnum++;
                    if (randnum >= 36)
                    {
                        randnum = 0;
                    }
                    GameObject pice = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                    allDots[i, j] = pice;
                    pice.GetComponent<Dot>().row = j;
                    pice.GetComponent<Dot>().column = i;

                }
            }
        }
    }

    bool MatchesOnBoard() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                if (allDots[i,j] != null) {
                    if (allDots[i,j].GetComponent<Dot>().isMatched) {
                        return true;
                    }
                }
            }
        }

        return false;
    }


    void SetPrevPos() {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++) {
                allDots[i, j].GetComponent<Dot>().prevColumn = allDots[i, j].GetComponent<Dot>().column;
                allDots[i, j].GetComponent<Dot>().prevRow = allDots[i, j].GetComponent<Dot>().row;
                allDots[i, j].name = i + "" + j;
                allDots[i, j].transform.parent = FindObjectOfType<Board>().transform;
            }
        }
    }

    IEnumerator FillBoardCo() {
        RefillBoard();
        yield return new WaitForSeconds(0.5f);

        while (MatchesOnBoard()) {
            streak++;
            yield return new WaitForSeconds(0.5f);
            DestroyMatches();
            
        }
        yield return new WaitForSeconds(0.3f);
        currentGameState = GameState.move;
        streak = 1;
        //gameManager.oncecall1 = false;
        if (Camera.main.GetComponent<GemInputManager>().IsPlayer)
        {
            print(Camera.main.GetComponent<GemInputManager>().IsPlayer);
            Camera.main.transform.GetComponent<Match3Conector>().PlayerMoveMade(GameObject.Find("Board").GetComponent<ScoreManager>().score);
            GameObject.Find("Board").GetComponent<ScoreManager>().score = 0; 
  
            //Camera.main.GetComponent<GemInputManager>().IsPlayer = false;
        
        }

        SetPrevPos();

        //gameManager.oncecall1 = false;




    }
}
