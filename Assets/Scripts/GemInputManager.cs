using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemInputManager : MonoBehaviour
{

    [SerializeField] GameObject Dot1,Dot2;
    [SerializeField] Board board;
    [SerializeField] int dot1C, dot2C, dot1r, dot2r;
    Vector2 firstTouch;
    Vector2 lastTouch;
    public float swipeResist = 1f;

    public float swipeAngle = 0;
    public bool IsPlayer;

    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 1000);

            if (hit)
            {
                firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
                Dot1 = hit.collider.gameObject;
                dot1C = Dot1.GetComponent<Dot>().column;
                dot1r = Dot1.GetComponent<Dot>().row;

            }


        }
        else if(Input.GetMouseButtonUp(0) && Dot1 != null)
        {
            lastTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
            
        }
    }

    void CalculateAngle(){
        if (IsPlayer)
        {
            if (Mathf.Abs(lastTouch.y - firstTouch.y) > swipeResist || Mathf.Abs(lastTouch.x - firstTouch.x) > swipeResist) {
            swipeAngle = Mathf.Atan2(lastTouch.y - firstTouch.y, lastTouch.x - firstTouch.x) * 180 / Mathf.PI;
            Dot1.GetComponent<Dot>().swipeAngle =swipeAngle;
            Dot1.GetComponent<Dot>().MovePices();
            board.currentGameState = GameState.wait;
            Camera.main.GetComponent<Match3Conector>().SetPiceMovementData(dot1C, dot1r, Dot1.gameObject.name, swipeAngle);
            //IsPlayer = false; 
            }
        }
            
        
        
    }

    public void setdata(int dotc, int dotr, string dotname, float swipeangle){
        Dot1 = GameObject.Find(dotname);

        dot1C = dotc;
        dot1r = dotr;
        
        Dot1.GetComponent<Dot>().swipeAngle = swipeangle;
        
        Dot1.GetComponent<Dot>().MovePices(); 
        board.currentGameState = GameState.wait;
        
    }


    
}
