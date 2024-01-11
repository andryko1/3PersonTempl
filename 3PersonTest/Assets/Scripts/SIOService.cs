using Firesplash.GameDevAssets.SocketIO;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


#if HAS_JSON_NET
using Newtonsoft.Json;
#endif

public class SIOService : MonoBehaviour
{
    public SocketIOCommunicator sioCom;

    public PlayerInfo player;
    public string SceneToLoad;

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

     private Dictionary<string, GameObject> serverObjects;

    //We are defining a Coroutine to swittch the scene and wait for the load to complete, but this is no requirement.
    //This could also be done in various other ways.
    // IEnumerator SwitchSceneAndReply()
    // {
    //     Debug.Log("Coroutine: Now loading the second scene");

    //     //start switching the scene
    //     AsyncOperation loadOperation = SceneManager.LoadSceneAsync("MSE-TargetScene");

    //     //wait until the new scene is fully loaded
    //     yield return new WaitUntil(() => loadOperation.isDone);

    //     Debug.Log("Coroutine: The scene has loaded. Emitting next message to the server.");

    //     //Now send the next package to the server
    //     ItsMeData me = new ItsMeData()
    //     {
    //         version = Application.unityVersion
    //     };
    //     sioCom.Instance.Emit("ItsMe", JsonUtility.ToJson(me), false);
    // }



    void Start()
    {
        //Make the gameObject carrying this script survive a scene change. This is important!
        DontDestroyOnLoad(gameObject);


        sioCom.Instance.On("connect", (string data) => {
            Debug.Log("LOCAL: Hey, we are connected!");
            sioCom.Instance.Emit("KnockKnock");
        });


        //When the server sends WhosThere, we will switch the scene
        sioCom.Instance.On("WhosThere", (string payload) =>
        {
            Debug.Log("The server asked who we are... Switching scenes and then answering back");

            //This runs the previously defined coroutine
           // StartCoroutine(SwitchSceneAndReply());
        });

        //The following code is not related to the scene changing.


        sioCom.Instance.On("Welcome", (string payload) =>
        {
            Debug.Log("Now - after the scene change - the server said hi :)");

            Debug.Log("SERVER: " + payload);

            sioCom.Instance.Emit("Goodbye", "Thanks for talking to me!", true);
        });




        //When the conversation is done, the server will close our connection after we said Goodbye
        sioCom.Instance.On("disconnect", (string payload) => {
            if (payload.Equals("io server disconnect"))
            {
                Debug.Log("Disconnected from server.");
            }
            else
            {
                Debug.LogWarning("We have been unexpectedly disconnected. This will cause an automatic reconnect. Reason: " + payload);
            }
        });

       // sioCom.Instance.Connect("https://sio-v4-example.unityassets.i01.clu.firesplash.de", false);
        Initialize();
       SetupEvens();
    }

    private void Initialize()
    {
        serverObjects = new Dictionary<string, GameObject>();
    }

    private void SetupEvens()
    {
        sioCom.Instance.On("open", (E) => {
            Debug.Log("Connection made to the Server!!!!!!!!");           
        });

        sioCom.Instance.On("register", (E) => {
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

        sioCom.Instance.On("login", (E) => {
            //string id = NetObjectID.CreateFromJSON(E).id;
            Debug.Log("LOGIN !!!! " + E);
            JsonUtility.FromJsonOverwrite(E.Replace("\\",""), PlayerInfo.instance);
            // JsonUtility.FromJsonOverwrite()
            string st=  NetPlayerInfo.CreateFromJSON(E.Replace("\\", "")).status; 

            Debug.Log("STATUS !!!! " + st);

            

            //PlayerInfo.instance.LobbyStart();

            DataHolder.playerId = PlayerInfo.instance.id;
            //DataHolder.port = PlayerInfo.instance.port;
            Debug.Log("playerId !!!! " + DataHolder.playerId);


            //sioCom.Instance.Close();

            //SceneManager.LoadScene(SceneToLoad);

            PlayerInfo.instance.LobbyStart();

        });

        sioCom.Instance.On("lobby", (E) => {
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

        sioCom.Instance.On("getPlayerInfo", (E) => {
            //string id = NetObjectID.CreateFromJSON(E).id;
            Debug.Log("info " + E);
            JsonUtility.FromJsonOverwrite(E.Replace("\\", ""), PlayerInfo.instance);
            // JsonUtility.FromJsonOverwrite()
            string st = NetPlayerInfo.CreateFromJSON(E.Replace("\\", "")).status; 
        });


        sioCom.Instance.On("dosconnected", (E) => {
            string id = NetObjectID.CreateFromJSON(E).id;
            Debug.Log("dosconnected " + id);
            GameObject go = serverObjects[id];
            Destroy(go);

            serverObjects.Remove(id);
        });
    }
     
}

[System.Serializable]
public class NetObjectID
{
    public string id;
    
    public static NetObjectID CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<NetObjectID>(jsonString);
    }

    // Given JSON input:
    // {"name":"Dr Charles","lives":3,"health":0.8}
    // this example will return a PlayerInfo object with
    // name == "Dr Charles", lives == 3, and health == 0.8f.
}

[System.Serializable]
public class NetPlayerInfo
{

    //public string nikName = "Andyko";
    //public string email;
    //public string psw = "123123";
    public string id;
    public string status;

    public static NetPlayerInfo CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<NetPlayerInfo>(jsonString);

        //PlayerInfo.instance.email = i.email;
        //PlayerInfo.instance.nikName = i.nikName;
        //PlayerInfo.instance.status = i.status;
        //PlayerInfo.instance.id = i.id;
    }
}

[System.Serializable]
public class NetPlayerAct
{

    //public string nikName = "Andyko";
    //public string email;
    //public string psw = "123123";
    public string id;
    public string act;

    public float px;
    public float py;
    public float pz;
    public bool j;
    public bool f;
    public bool g;
    public float ry;

    public static NetPlayerAct CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<NetPlayerAct>(jsonString);

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
