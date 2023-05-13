using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class LobbyManager : MonoBehaviour
{
    public GameObject Alert;
    public SocketIOEvent currentMaps;
    public Transform maps;
    public GameObject MapPref;

    public SocketIOEvent currentRooms;
    public Transform rooms;
    public GameObject RoomPref;

    public List<JSONObject> jsonMaps;
    public List<JSONObject> jsonRooms;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("UserText").GetComponent<Text>().text = GameManager.instance.myUser;


        GameManager.SimpleSend("GetRoomsAMaps", string.Format(@"{{}}"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createMap(){
        GameManager.SimpleSend("OpenMapEditor", "{}");
        
    }

    public void OnCreateMap(SocketIOEvent e)
    {
        bool open = bool.Parse(e.data["open"].ToString().Replace("\"", ""));

        if (open)
        {
            string mapName = GameObject.Find("MapName").GetComponent<InputField>().text;
            bool isPublic = GameObject.Find("Public").GetComponent<Toggle>().isOn;
            GameManager.enterInEditor(mapName, isPublic);
        }
        else
        {
            Alert.SetActive(true);
            Alert.transform.Find("Text").GetComponent<Text>().text = "<color=red>Error: </color><b>Your account is not verified, do you want to send a verification email?</b>";
            Alert.transform.Find("OK").GetComponent<Button>().onClick.RemoveAllListeners();
            Alert.transform.Find("OK").GetComponent<Button>().onClick.AddListener(delegate {
                GameManager.SimpleSend("ResendVerificationEMail", "");
                GameObject.Find("Alert").SetActive(false);
            });
        }


        
    }

    public void RenderMaps(){
        jsonMaps = currentMaps.data["maps"].list;
        if(!maps.gameObject.activeInHierarchy){return;}
        for (int i = maps.childCount-1; i >= 0; i--)
        {
            Destroy(maps.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < jsonMaps.Count; i++)
        {
            var go = Instantiate(MapPref,Vector3.zero,Quaternion.identity,maps);
            go.transform.GetChild(0).GetComponent<Text>().text = jsonMaps[i]["MapName"].ToString().Replace("\"", "");
            go.transform.GetChild(1).GetComponent<Text>().text = " " + jsonMaps[i]["MapID"].ToString().Replace("\"", "");
            go.transform.GetChild(1).GetComponent<copyTo>().setButton(jsonMaps[i]["MapID"].ToString().Replace("\"", ""));
            go.transform.GetChild(2).GetComponent<MapButton>().setButton(jsonMaps[i]["MapID"].ToString().Replace("\"", ""));
        }
        
    }

    public void RenderRoom(){
        jsonRooms = currentRooms.data["rooms"].list;
        if(!rooms.gameObject.activeInHierarchy){return;}
        for (int i = rooms.childCount-1; i >= 0; i--)
        {
            Destroy(rooms.GetChild(i).gameObject);
        }
        
        for (int i = 0; i < jsonRooms.Count; i++)
        {
            var go = Instantiate(RoomPref,Vector3.zero,Quaternion.identity,rooms);
            go.transform.GetChild(0).GetComponent<Text>().text = jsonRooms[i]["roomName"].ToString().Replace("\"", "");
            go.transform.GetChild(1).GetComponent<Text>().text = jsonRooms[i]["roomPlayers"].ToString().Replace("\"", "");
            go.transform.GetChild(2).GetComponent<Text>().text = jsonRooms[i]["roomMapId"].ToString().Replace("\"", "");
            //joinRoom(jsonRooms[i]["roomId"].ToString().Replace("\"", ""));
            go.transform.Find("Button").GetComponent<RoomButton>().setID(jsonRooms[i]["roomId"].ToString().Replace("\"", ""));
        }
        
    }

    public void joinRoom(string id){
        Debug.Log("Joing to " + id);
        GameManager.JoinRoom(id);
    }

    public void CreateRoom(){
        string roomMap = GameObject.Find("RoomName").GetComponent<InputField>().text;
        string roomName = GameObject.Find("RoomMapCode").GetComponent<InputField>().text;
        int roomMaxPlayer = (int)GameObject.Find("Slider").GetComponent<Slider>().value;
        GameManager.createRoom(roomMap,roomName, roomMaxPlayer);
    }

    public void NoLogin()
    {
        PlayerPrefs.SetString("Email", "-");
        PlayerPrefs.SetString("Password", "-");
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void SetValue(GameObject yourSelf)
    {
        yourSelf.transform.GetChild(yourSelf.transform.childCount - 1).GetComponent<Text>().text = "\n" + yourSelf.GetComponent<Slider>().value.ToString();
    }
    public void PreviewMap(string id)
    {
        GameManager.SimpleSend("GetPreviewMap", string.Format(@"{{ ""mapId"":""{0}"" }}", id));
    }
}
