using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firesplash.GameDevAssets.SocketIO;

public  class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo instance;

    public string nickName ="Andyko";
    public string email;
    public string psw="123123";
    public string id;
    public string status;
    public string port;
    public SocketIOCommunicator sioCom;

    private void Awake()
    {
        instance = this;     

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     public void Login()
    {
        string logData;
        //logData = "{\"nikName\": \"" + nikName + "\","
        //        + "\"email\": \"" + email + "\","
        //        + "\"psw\": \"" + psw + "\","
        //        + "\"id\": \"" + id + "\","
        //        + "\"status\": \"" + status + "\"}";

        logData = JsonUtility.ToJson(this);
        Debug.Log(logData);

        //SocketIOCommunicator.in
        sioCom.Instance.Emit("login", logData, false);
    }

     public void LobbyStart()
    {
        string logData;
        //logData = "{\"nikName\": \"" + nikName + "\","
        //        + "\"email\": \"" + email + "\","
        //        + "\"psw\": \"" + psw + "\","
        //        + "\"id\": \"" + id + "\","
        //        + "\"status\": \"" + status + "\"}";

        this.id = DataHolder.playerId;
        logData = JsonUtility.ToJson(this);
        Debug.Log(logData);

        //SocketIOCommunicator.in
        sioCom.Instance.Emit("lobby", logData, false);
    }

     public void PlayerInfo_()
    {
        string logData;
        //logData = "{\"nikName\": \"" + nikName + "\","
        //        + "\"email\": \"" + email + "\","
        //        + "\"psw\": \"" + psw + "\","
        //        + "\"id\": \"" + id + "\","
        //        + "\"status\": \"" + status + "\"}";

        this.id = DataHolder.playerId;
        logData = JsonUtility.ToJson(this);
        Debug.Log(logData);

        //SocketIOCommunicator.in
        sioCom.Instance.Emit("playerInfo", logData, false);
    }

      public void ChatMes(string Mess)
    {
        string logData;
        if (Mess != "")
        {
            
            logData = "{\"nickName\": \"" + this.nickName + "\","
                    + "\"chatMes\": \"" + Mess + "\"}";

            Debug.Log(logData);
           sioCom.Instance.Emit("chatMes", logData, false);
        }

       

       
       
    }
}
