using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dot : MonoBehaviour {

    [Header("Board Thingi")]
    public int column;
    public int row;

    public int offset;
    GameObject otherDots;
    Board board;
    FindMatches findMatches;
    public int targetX;
    public int targetY;

    public bool isMatched = false;
    public int prevColumn;
    public int prevRow;
    public float swipeResist = 1f;

    Vector2 firstTouch;
    Vector2 lastTouch;
    Vector2 tempos;


    public int dotValue;



    public float swipeAngle = 0;

    // Start is called before the first frame update
    void Start() {
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        // targetX = (int)transform.position.x;
        // targetY = (int)transform.position.y;
        // row = targetY;
        // column = targetX;
        // prevColumn = column;
        // prevRow = row;

    }

    // Update is called once per frame
    void Update() {
        targetX = column;
        targetY = row;
        if (Mathf.Abs(targetX - transform.position.x) > .1) {
            //Move toward the target
            tempos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempos, .6f);
            if (board.allDots[column, row] != gameObject) {
                board.allDots[column, row] = gameObject;
            }
            //findMatches.FindAllMatch();
        } else {
            //Direct set pos
            tempos = new Vector2(targetX, transform.position.y);
            transform.position = tempos;
            //board.allDots[column, row] = gameObject;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1) {
            //Move toward the target
            tempos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempos, .6f);
            if (board.allDots[column, row] != gameObject) {
                board.allDots[column, row] = gameObject;
            }
            //findMatches.FindAllMatch();
        } else {
            //Direct set pos
            tempos = new Vector2(transform.position.x, targetY);
            transform.position = tempos;
           
        }

        if (isMatched) {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, .2f);
        }

        findMatches.FindAllMatch();
    }

    private void OnMouseDown() {
        if (!EventSystem.current.IsPointerOverGameObject()) {
            if (board.currentGameState == GameState.move) {
                firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        
    }

    private void OnMouseUp() {
        if (!EventSystem.current.IsPointerOverGameObject()) {
            if (board.currentGameState == GameState.move) {
                lastTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //CalculateAngle();

            }
        }

    }

    void CalculateAngle() {
        if (Mathf.Abs(lastTouch.y - firstTouch.y) > swipeResist || Mathf.Abs(lastTouch.x - firstTouch.x) > swipeResist) {
            swipeAngle = Mathf.Atan2(lastTouch.y - firstTouch.y, lastTouch.x - firstTouch.x) * 180 / Mathf.PI;
            MovePices();
            board.currentGameState = GameState.wait;
        } else {
            board.currentGameState = GameState.move;
        }
       
    }

    public void MovePices() {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1) {
            print("Right Swipe");
            otherDots = board.allDots[column + 1, row];
            prevColumn = column;
            prevRow = row;
            otherDots.GetComponent<Dot>().column--;
            column++;
        } else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) {
            //Up Swipe
            print("Up Swipe");
            otherDots = board.allDots[column, row + 1];
            prevColumn = column;
            prevRow = row;
            otherDots.GetComponent<Dot>().row--;
            row++;
        } else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0) {
            print("Left Swipe");
            otherDots = board.allDots[column - 1, row];
            prevColumn = column;
            prevRow = row;
            otherDots.GetComponent<Dot>().column += 1;
            column -= 1;
        } else if ((swipeAngle < -45 && swipeAngle >= -135 && row > 0)) {
            //Down Swipe
            print("Down Swipe");
            otherDots = board.allDots[column, row - 1];
            prevColumn = column;
            prevRow = row;
            otherDots.GetComponent<Dot>().row += 1;
            row--;

        }
        StartCoroutine(CheckMoveCo());
        
         
    }

    void FindMatches() {
        if (column > 0 && column < board.width - 1) {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if (leftDot1 != null && rightDot1 != null) {
                if (leftDot1.tag == gameObject.tag && rightDot1.tag == gameObject.tag) {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                        rightDot1.GetComponent<Dot>().isMatched = true;
                        isMatched = true;
                       
                    
                }
            }
            

        }
        if (row > 0 && row < board.width - 1) {
            GameObject UpDot1 = board.allDots[column , row + 1];
            GameObject DownDot1 = board.allDots[column, row - 1];
            if (UpDot1 != null && DownDot1  != null) {
                if (UpDot1.tag == gameObject.tag && DownDot1.tag == gameObject.tag) {
                    UpDot1.GetComponent<Dot>().isMatched = true;
                       DownDot1.GetComponent<Dot>().isMatched = true;
                       isMatched = true;
                       

                }
            }
        }
    }

    public IEnumerator CheckMoveCo() {
        yield return new WaitForSeconds(.5f);
        if (otherDots != null) {
            if (!isMatched && !otherDots.GetComponent<Dot>().isMatched) {
                otherDots.GetComponent<Dot>().row = row;
                otherDots.GetComponent<Dot>().column = column;
                row = prevRow;
                column = prevColumn;
                yield return new WaitForSeconds(0.5f);
                board.currentGameState = GameState.move;
                //print(board.currentGameState);
            } else {
                board.DestroyMatches();
                
               
            }
            otherDots = null;
        } 

    }

}
