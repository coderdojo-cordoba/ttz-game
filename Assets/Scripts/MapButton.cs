using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapButton : MonoBehaviour
{
    string mapid;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setButton(string id)
    {
        mapid = id;
        GetComponent<Button>().onClick.AddListener(callF);
    }

    public void callF() {
        GameObject.FindObjectOfType<LobbyManager>().PreviewMap(mapid);
    }
}
