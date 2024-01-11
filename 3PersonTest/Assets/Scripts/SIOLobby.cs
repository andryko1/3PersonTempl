using Firesplash.GameDevAssets.SocketIO;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using StarterAssets;
using System.Data.Common;
using Unity.VisualScripting;

#if HAS_JSON_NET
//If Json.Net is installed, this is required for Example 6. See documentation for information on how to install Json.NET
//Please note that most recent unity versions bring Json.Net with them by default, you would only need to enable the compiler flag as documented.
using Newtonsoft.Json;
#endif

public class SIOLobby : MonoBehaviour
{
  public SocketIOCommunicator sioCom;
  public Text uiStatus, uiGreeting, uiPodName;
  public PlayerInfo player;
  public string SceneToLoad;
  public TMP_Text chatText;
  public ScrollRect scrollChat;

  private Boolean chatBotton = false;

  private ThirdPersonController _thirdPersonController;

  [Serializable]
  struct ItsMeData
  {
    public string version;
  }

  [Serializable]
  struct ServerTechData
  {
    public string timestamp;
    public string podName;
  }

  //[SerializeField]
  //private Transform networkConteiner;

  private Dictionary<string, GameObject> serverObjects;
  private Dictionary<string, GameObject> playerObjects;

  // Start is called before the first frame update

  private void Awake()
  {
    Debug.Log("WWW 1");
    if (DataHolder.port.Length > 1)
    {
      // Debug.Log(DataHolder.port);
      //    sioCom.Instance.Close();
      //sioCom.socketIOAddress = "192.168.1.71:" + DataHolder.port.ToString();
      sioCom.Instance.Connect(DataHolder.adress + ":" + DataHolder.port.ToString(), true);
      //sioCom.socketIOPort = DataHolder.port;          
      PlayerInfo.instance.id = DataHolder.playerId;
      //    //Destroy(sioCom);
      //    //Instantiate(sioCom);
      //    //sioCom.Instance.Connect();

      //        //Connect((sioCom.secureConnection ? "https" : "http") + "://" + sioCom.socketIOAddress + ":" + sioCom.socketIOPort, sioCom.autoReconnect);

    }

    // Debug.Log("WWW " + sioCom.socketIOAddress);
  }
  void Start()
  {
    _thirdPersonController = FindObjectOfType<ThirdPersonController>();
    //sioCom.Instance.Connect();
    scrollChat.onValueChanged.AddListener(ListenerMethod);
    //scrollRect.onValueChanged.AddListener(ListenerMethod);

    sioCom.Instance.On("connect", (string data) =>
    {
      Debug.Log("LOCAL LOBBY : Hey, we are connected !");
      //uiStatus.text = "Socket.IO Connected. Doing work...";

      //NOTE: All those emitted and received events (except connect and disconnect) are made to showcase how this asset works. The technical handshake is done automatically.

      //First of all we knock at the servers door
      //EXAMPLE 1: Sending an event without payload data



      //sioCom.Instance.Emit("KnockKnock");
      //socket.on('playerInfo', async (data) => {
      player.PlayerInfo_();
      string logData;
      //logData = "{\"nikName\": \"" + nikName + "\","
      //        + "\"email\": \"" + email + "\","
      //        + "\"psw\": \"" + psw + "\","
      //        + "\"id\": \"" + id + "\","
      //        + "\"status\": \"" + status + "\"}";

      logData = JsonUtility.ToJson(PlayerInfo.instance);
      Debug.Log(logData);

      sioCom.Instance.Emit("joinL", logData, false);

    });

    sioCom.Instance.On("open", (E) =>
    {
      Debug.Log("Connection made to the Server!!!!!!!!");
    });

    Initialize();
    SetupEvens();
  }
  public void ListenerMethod(Vector2 value)
  {
    Debug.Log("ListenerMethod: " + value);
    if (chatBotton)
    {
      scrollChat.verticalNormalizedPosition = 0f;
      chatBotton = false;
    }
  }

  private void Initialize()
  {
    serverObjects = new Dictionary<string, GameObject>();
    playerObjects = new Dictionary<string, GameObject>();

    // playerObjects.Add(PlayerInfo.instance.id,GameObject.Find("PlayerArmature"));
  }


  private void SetupEvens()
  {


    sioCom.Instance.On("register", (E) =>
    {
      string id = NetObjectID.CreateFromJSON(E).id;
      Debug.Log("register " + id);
    });

    //sioCom.Instance.On("spawn", (E) => {
    //    NetPlayerInfo playerInfo = NetPlayerInfo.CreateFromJSON(E);
    //    Debug.Log("spawn " + playerInfo.id +' '+ playerInfo.username);
    //    GameObject go = new GameObject("Server ID = " + playerInfo.id);
    //    go.transform.SetParent(networkConteiner);
    //    serverObjects.Add(playerInfo.id, go); 
    //});

    sioCom.Instance.On("login", (E) =>
    {
      //string id = NetObjectID.CreateFromJSON(E).id;
      Debug.Log("LOGIN !!!! " + E);
      JsonUtility.FromJsonOverwrite(E.Replace("\\", ""), PlayerInfo.instance);
      // JsonUtility.FromJsonOverwrite()
      string st = NetPlayerInfo.CreateFromJSON(E.Replace("\\", "")).status;

      Debug.Log("STATUS !!!! " + st);

      //PlayerInfo.instance.LobbyStart();

      DataHolder.playerId = PlayerInfo.instance.id;
      //DataHolder.port = PlayerInfo.instance.port;



      //sioCom.Instance.Close();

      //SceneManager.LoadScene(SceneToLoad);

      PlayerInfo.instance.LobbyStart();

    });

    sioCom.Instance.On("lobby", (E) =>
    {
      //string id = NetObjectID.CreateFromJSON(E).id;
      Debug.Log("lobby " + E);
      JsonUtility.FromJsonOverwrite(E.Replace("\\", ""), PlayerInfo.instance);
      // JsonUtility.FromJsonOverwrite()
      string st = NetPlayerInfo.CreateFromJSON(E.Replace("\\", "")).status;

      Debug.Log("STATUS ### " + st);


      DataHolder.playerId = PlayerInfo.instance.id;
      DataHolder.port = PlayerInfo.instance.port;


      sioCom.Instance.Close();

      SceneManager.LoadScene(SceneToLoad);



    });

    sioCom.Instance.On("game", (E) =>
    {
      //string id = NetObjectID.CreateFromJSON(E).id;
      Debug.Log("game " + E);
      JsonUtility.FromJsonOverwrite(E.Replace("\\", ""), PlayerInfo.instance);
      // JsonUtility.FromJsonOverwrite()
      string st = NetPlayerInfo.CreateFromJSON(E.Replace("\\", "")).status;

      Debug.Log("STATUS ### " + st);


      // DataHolder.playerId = PlayerInfo.instance.id;
      DataHolder.port = PlayerInfo.instance.port;

      Debug.Log("GAME PORT " + DataHolder.port.ToString());


      sioCom.Instance.Close();

      SceneManager.LoadScene(SceneToLoad);

    });

    sioCom.Instance.On("playerInfo", (E) =>
    {
      //string id = NetObjectID.CreateFromJSON(E).id;
      Debug.Log("info " + E);
      JsonUtility.FromJsonOverwrite(E.Replace("\\", ""), PlayerInfo.instance);
      // JsonUtility.FromJsonOverwrite()
      string st = NetPlayerInfo.CreateFromJSON(E.Replace("\\", "")).status;
    });


    sioCom.Instance.On("joinL", (E) =>
    {
      //string id = NetObjectID.CreateFromJSON(E).id;
      // Debug.Log("joinL " + E);
      // JsonUtility.FromJsonOverwrite(E.Replace("\\", ""), PlayerInfo.instance);
      //PlayerInfo newPl =  JsonUtility.FromJson<PlayerInfo>(E.Replace("\\", ""));
      // JsonUtility.FromJsonOverwrite()
      string id = NetPlayerInfo.CreateFromJSON(E.Replace("\\", "")).id;
      Debug.Log("newPl.id " + id);
      // Debug.Log("newPl.status "+st);
      playerObjects.Add(id, GameObject.Find("PlayerArmature1"));

      // Debug.Log("playerObjects[id].transform.position.x " + playerObjects[id].transform.position.x);
      // Debug.Log("playerObjects[id].GetComponent<TestMove>().targetPos.z " + playerObjects[id].GetComponent<TestMove>().targetPos.z);
      ;
    });


    sioCom.Instance.On("plAct", (E) =>
    {
      string stData = E.Replace("\\", "");

      NetPlayerAct act = NetPlayerAct.CreateFromJSON(stData);
    


      TestMove _testMove = playerObjects[act.id].GetComponent<TestMove>();
    
      if (act.act == "r")
      {
        Debug.Log(stData);
      //    _testMove.g = act.g;
      //   _testMove.m = false;
      //    if (_testMove.g)
      //   {
      //     _testMove.f = false;
      //     _testMove.j = false;
      //   }
      //   //Vector3 newRotation = new Vector3(0f,act.py ,0f);

      // //  playerObjects[act.id].transform.rotation.eulerAngles.Set(0.0f, act.py, 0.0f);
      //   //playerObjects[act.id].transform.rotation = Quaternion.Euler(0.0f, act.py, 0.0f);
      //   _testMove.moveSpeed = 0;
      //   Debug.Log("_testMove.m " + _testMove.m.ToString());
      }
      else
      {
        // Debug.Log(stData);
        // Debug.Log("act.ry " + act.ry.ToString());
        //  Debug.Log("act.ry1 " + stData.IndexOf("ry\":",1).ToString());
        if(stData.IndexOf("ry\":",1)>=0) {        
           _testMove.target_ry = act.ry;
        }

    
        _testMove.m = true;
        _testMove.targetPos.x = act.px;
        _testMove.targetPos.y = act.py;
        _testMove.targetPos.z = act.pz + 1;
        _testMove.g = act.g;
        _testMove.lastUpdate = System.DateTime.Now;
        if (_testMove.g)
        {
          _testMove.f = false;
          _testMove.j = false;
        }
        else
        {
          _testMove.m  =false;
          _testMove.f = act.f;
          _testMove.j = act.j;
        }


        _testMove.moveSpeed = (_testMove.targetPos - playerObjects[act.id].transform.position).magnitude / (DataHolder.updateDelay / 1000);
    
      }
    });

    sioCom.Instance.On("chatMes", (E) =>
    {
      //string id = NetObjectID.CreateFromJSON(E).id;
      Debug.Log("chatMes " + E);
      // JsonUtility.FromJsonOverwrite(E.Replace("\\", ""), PlayerInfo.instance);
      // JsonUtility.FromJsonOverwrite()
      NetChatMes mes = NetChatMes.CreateFromJSON(E.Replace("\\", ""));
      chatText.text += "\n" + mes.nickName + ": " + mes.chatMes;
      //scrollChat.verticalNormalizedPosition = 0f;
      // scrollChat.verticalScrollbar.value = -0.1f;
      chatBotton = true;



    });


    sioCom.Instance.On("disconnected", (E) =>
    {
      string id = NetObjectID.CreateFromJSON(E).id;
      Debug.Log("disconnected " + id);
      GameObject go = serverObjects[id];
      Destroy(go);

      serverObjects.Remove(id);
    });
  }

}

public class NetChatMes
{

  //public string nikName = "Andyko";
  //public string email;
  //public string psw = "123123";
  //public string id;
  public string nickName;
  public string chatMes;

  public static NetChatMes CreateFromJSON(string jsonString)
  {
    return JsonUtility.FromJson<NetChatMes>(jsonString);

    //PlayerInfo.instance.email = i.email;
    //PlayerInfo.instance.nikName = i.nikName;
    //PlayerInfo.instance.status = i.status;
    //PlayerInfo.instance.id = i.id;
  }



  // Given JSON input:
  // {"name":"Dr Charles","lives":3,"health":0.8}
  // this example will return a PlayerInfo object with
  // name == "Dr Charles", lives == 3, and health == 0.8f.
}

