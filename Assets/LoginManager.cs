
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LoginManager : MonoBehaviour
{
    public Match3Conector match3Conector;
    public Board board;
    [SerializeField] InputField IpInputField, UserInputFeild;


    public void OnLogginButton()
    {
        match3Conector.ServerIP = IpInputField.text;
        match3Conector.userName = UserInputFeild.text;
        match3Conector.StartLogin();
        board.enabled = true;
        Destroy(gameObject);
    }
}
