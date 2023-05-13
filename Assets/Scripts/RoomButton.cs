using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    string roomid;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setID(string id){
        GetComponent<Button>().onClick.AddListener(join);
        roomid = id;
    }
    public void join(){
        GameObject.FindObjectOfType<LobbyManager>().joinRoom(roomid);
    }
}
