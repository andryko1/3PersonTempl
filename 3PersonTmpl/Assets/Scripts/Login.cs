using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Firesplash.GameDevAssets.SocketIO;

public class Login : MonoBehaviour
{
    
    [Header("UI") ]
    public TMP_InputField inputNickName;
    public TMP_InputField inputPsw;
    public Button LoginButtom;  

    public SocketIOCommunicator sioCom;



    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void Login()
    //{
    //    //string logData;
    //    ////logData = "{\"nikName\": \"" + nikName + "\","
    //    ////        + "\"email\": \"" + email + "\","
    //    ////        + "\"psw\": \"" + psw + "\","
    //    ////        + "\"id\": \"" + id + "\","
    //    ////        + "\"status\": \"" + status + "\"}";

    //    //logData = JsonUtility.ToJson(PlayerInfo.instance);
    //    //Debug.Log(logData);

        
    //    ////sioCom.Instance.Emit("login", logData, false);
    //}

    public void LoginButtomClick()
    {
        //Test
        inputNickName.text = inputNickName.text; //"andryko";
        inputPsw.text = "123123";

        Debug.Log(inputNickName.text);

        if (inputNickName.text.IndexOf('@') > 0)
        {
            PlayerInfo.instance.email = inputNickName.text;
            PlayerInfo.instance.nickName = "";
        }
        else
        {
            PlayerInfo.instance.email = "";
            PlayerInfo.instance.nickName = inputNickName.text;
        }

        PlayerInfo.instance.psw = inputPsw.text;

        string logData = JsonUtility.ToJson(PlayerInfo.instance);
        Debug.Log(logData);


        sioCom.Instance.Emit("login", logData, false);

    }
}


