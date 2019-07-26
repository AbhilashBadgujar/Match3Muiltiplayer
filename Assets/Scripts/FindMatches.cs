using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    Board board;
    public List<GameObject> currentMatch;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        currentMatch = new List<GameObject>();
    }

    public void FindAllMatch() {
        StartCoroutine(FindAllMatchesCo());
    }

    IEnumerator FindAllMatchesCo () {
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < board.width; i++) {
            for (int j = 0; j < board.height; j++) {
                GameObject currentDot = board.allDots[i, j];
                if (currentDot != null) {
                    if (i >0 && i < board.width -1) {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];
                        if (leftDot != null && rightDot != null) {
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag) {
                                if (!currentMatch.Contains(leftDot)) {
                                    currentMatch.Add(leftDot);

                                }
                                if (!currentMatch.Contains(rightDot)) {
                                    currentMatch.Add(rightDot);

                                }
                                if (!currentMatch.Contains(currentDot)) {
                                    currentMatch.Add(currentDot);

                                }
                                leftDot.GetComponent<Dot>().isMatched = true;
                                rightDot.GetComponent<Dot>().isMatched = true;
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                    if (j > 0 && j < board.height - 1) {
                        GameObject UpDot = board.allDots[i, j + 1];
                        GameObject DownDot = board.allDots[i, j - 1];

                        if (UpDot != null && DownDot != null) {
                            if (UpDot.tag == currentDot.tag && DownDot.tag == currentDot.tag) {
                                if (!currentMatch.Contains(UpDot)) {
                                    currentMatch.Add(UpDot);

                                }
                                if (!currentMatch.Contains(DownDot)) {
                                    currentMatch.Add(DownDot);

                                }
                                if (!currentMatch.Contains(currentDot)) {
                                    currentMatch.Add(currentDot);

                                }
                                UpDot.GetComponent<Dot>().isMatched = true;
                                DownDot.GetComponent<Dot>().isMatched = true;
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }
}
