using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Controllers;
using Sfs2X.Entities.Variables;
using System;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Util;

public class Match3Conector : MonoBehaviour
{

    private SmartFox sfs;
    
    Board board; 
    [SerializeField] int whoseTurn;

    [SerializeField]
    private int myPlayerID;
    private bool isPlayer1;

    public string ServerIP = "127.0.0.1";
    public int ServerPort = 9933;
    public string ZoneName = "BasicExamples";
    public string userName = " ";
    public string RoomName = "test";
    private string ExtName = "matchThreeExtension";
    private string ExtClass = "matchThreeJExtension";
    GameManager gameManager;
    [SerializeField]
    private Text _scoreText;



    [SerializeField]
    int dotC, dotR;

    [SerializeField]
    string dotName;
    [SerializeField]
    float swipeAng;



    [SerializeField] int[] GemsRandValue;


    // Start is called before the first frame update
    void Start() {
        
        gameManager = GameObject.Find("Board").GetComponent<GameManager>();
        board = GameObject.Find("Board").GetComponent<Board>();
        sfs = new SmartFox();
        sfs.ThreadSafeMode = true;

        sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginR);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnJoinRoom);
        sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
    }

    public void OnLoginR(BaseEvent e){
        print("no");
    }

    public void StartLogin(){
        print("a");
        sfs.Connect(ServerIP, ServerPort);
    }

    private void OnExtensionResponse(BaseEvent e) {
        string cmd = (string)e.Params["cmd"];
        SFSObject dataObject = (SFSObject)e.Params["params"];


        if (cmd == "sum") {
            print("sum " + dataObject.GetInt("res"));
            GemsRandValue = dataObject.GetIntArray("gemType");
            board.Setup(GemsRandValue);
        }
        if (cmd == "start") {
            
            StartGame(dataObject.GetInt("t"),
                   dataObject.GetInt("p1i"),
                   dataObject.GetInt("p2i"),
                   dataObject.GetInt("p1h"),
                   dataObject.GetInt("p2h")
                   );
        }
        if (cmd == "move") {
            OnMoveRecived(dataObject.GetInt("t"), dataObject.GetInt("p1h"), dataObject.GetInt("p2h"));
        }

        if (cmd == "dotdata")
        { 
            
            PiceMoveData(dataObject.GetInt("Dotc"), dataObject.GetInt("Dotr"), dataObject.GetUtfString("dotName"), dataObject.GetFloat("swipeAngle"));
        }
        if (cmd == "updateTurn")
        {
           StartCoroutine(UpdateTurn(dataObject.GetInt("t")));
        }

        
        
    }

    IEnumerator UpdateTurn(int _whoseTurn){
        yield return new WaitForSeconds(1f);
        whoseTurn = _whoseTurn;
        if (whoseTurn == myPlayerID)
        {
            print(whoseTurn + "" + myPlayerID);
            Camera.main.GetComponent<GemInputManager>().IsPlayer = true;
        }else
        {
            print("a");
            Camera.main.GetComponent<GemInputManager>().IsPlayer = false;
        }
    }

    //my move 
    public void PlayerMoveMade(int amt) {
        
        gameManager.SetSliderValue();
        SFSObject obj = new SFSObject();
        obj.PutInt("attack", amt);
        
        sfs.Send(new ExtensionRequest("move", obj, sfs.LastJoinedRoom));
        
        CheckTurn(whoseTurn);
        gameManager.SetSliderValue();
        
        //PlayParticale(true);
    }



    


    public void SetPiceMovementData(int Dotc, int Dotr, string DotName, float swipeAngle){
            
            dotC = Dotc;
            dotR = Dotr;
            dotName = DotName;
            swipeAng = swipeAngle;

            SFSObject obj = new SFSObject();
            obj.PutInt("DotC", dotC);
            obj.PutInt("DotR", dotR);
            obj.PutUtfString("dotName", dotName);
            
            obj.PutFloat("swipeAngle", swipeAng);
            
            //
            sfs.Send(new ExtensionRequest("dotdata", obj, sfs.LastJoinedRoom));




    }

    public void PiceMoveData(int Dotc, int Dotr, string DotName, float swipeAngle){

            SFSObject obj = new SFSObject();

            if (whoseTurn != myPlayerID)
            {
            
               Camera.main.GetComponent<GemInputManager>().setdata(Dotc, Dotr, DotName, swipeAngle);
               
            }
    }

    //enemy move 
    void OnMoveRecived(int movingPlayer, int P1H, int P2H) {
        print(movingPlayer);
       
        if (movingPlayer != myPlayerID) {
            PlayParticale(false);
            
            
        }else if (movingPlayer == myPlayerID)
        {
            PlayParticale(true);
        }
        gameManager.SetSliderValue();
        
        CheckTurn(whoseTurn);
        whoseTurn = movingPlayer;
        print(movingPlayer);
        //CheckTurn(whoseTurn);
         _scoreText.text = (myPlayerID == whoseTurn) ? "It's your opponent's turn" : "It's your turn";
          

         if (myPlayerID == whoseTurn)
         {
             gameManager.SetTurnPanel(false);
             print("my turn");
             
             
         }else
         {
             gameManager.SetTurnPanel(true);
             print("not my turn");
             
         }
        if (isPlayer1)
            {
                gameManager.myHealth = P1H;
                gameManager.EnemyHelth = P2H;
            }else{
                gameManager.myHealth = P2H;
                gameManager.EnemyHelth = P1H;
            }
            
    }

    void PlayParticale(bool player1){
        if (player1)
        {
            gameManager.PlayerEffects();
        }else{
            gameManager.EnemyEffects();
        }
    }

    private void StartGame(int _whoseTurn, int p1Id, int p2Id, int p1h, int p2h)
    {
        SetHealthValue(p1Id, p2Id, p1h, p2h);
        CheckTurn(_whoseTurn);
        whoseTurn = _whoseTurn;
        _scoreText.text = (myPlayerID == whoseTurn) ? "It's your turn" : "It's your opponent's turn";
        Camera.main.GetComponent<GemInputManager>().IsPlayer = (myPlayerID == whoseTurn) ? true : false;
    }

    private void SetHealthValue(int p1Id, int p2Id, int p1h, int p2h)
    {
        if (myPlayerID == p1Id)
        {
            gameManager.myHealthSlider.maxValue = p1h;
            gameManager.myHealth = p1h;
            gameManager.EnemyHealthSlider.maxValue = p2h;
            gameManager.EnemyHelth = p2h;
            isPlayer1 = true;

        }
        else if (myPlayerID == p2Id)
        {
            gameManager.myHealthSlider.maxValue = p2h;
            gameManager.myHealth = p2h;
            gameManager.EnemyHealthSlider.maxValue = p1h;
            gameManager.EnemyHelth = p1h;
            isPlayer1 = false;
        }
    }

    void CheckTurn(int iSturn) {
        if (iSturn == 1) {
            gameManager.playerTurn = PlayerTurn.player1;
        } else if (iSturn == 2) {
            gameManager.playerTurn = PlayerTurn.player2;
        }

    }

    private void OnRoomJoinError(BaseEvent e) {

        CreateNewRoom();
        
    }

    private void OnApplicationQuit() {
        if (sfs.IsConnected) {
            sfs.Disconnect();
        }
    }
    private void OnConnection(BaseEvent e) {
        if ((bool)e.Params["success"]) {
            print("sucess");
            sfs.Send(new LoginRequest(userName, "", ZoneName));
            print("b");

        } else {
            print("fail");
        }
    }


    void OnLogin(BaseEvent e) {
        print("LoggedIN " + e.Params["user"]);
        sfs.Send(new JoinRoomRequest(RoomName));
        

    }

    void OnJoinRoom(BaseEvent e) {
        print("Joined Room" + e.Params["room"]);

        if (sfs.RoomManager.GetRoomCount() == 1) {
            CreateNewRoom();
        }
        ISFSObject test = new SFSObject();
        test.PutInt("a", 25);
        test.PutInt("b", 17);
        test.PutInt("c", JsonPar.jsonPar.inData.coloum);
        test.PutInt("r", JsonPar.jsonPar.inData.row);
        test.PutInt("p", JsonPar.jsonPar.inData.spldotperct);
        
       
        sfs.Send(new ExtensionRequest("ready", test, sfs.LastJoinedRoom));

        sfs.Send(new ExtensionRequest("sum", test, sfs.LastJoinedRoom));

        myPlayerID = sfs.MySelf.PlayerId;
        print(myPlayerID);

        
        
    }


    private void CreateNewRoom() {
        
        RoomSettings settings = new RoomSettings("First Game");
        settings.MaxUsers = 2;
        settings.MaxSpectators = 0;
        settings.IsGame = true;
        
        settings.Extension = new RoomExtension(ExtName, ExtClass);

        sfs.Send(new CreateRoomRequest(settings, true, sfs.LastJoinedRoom));
    }



    // Update is called once per frame
    void Update()
    {
        if(sfs != null)
            sfs.ProcessEvents();
    }


    
}
