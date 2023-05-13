using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;


public class Stats
{
    public int kills;
    public int deads;
    public int money;
}
public class RoomManager : MonoBehaviour
{
    public Stats playerStats = new Stats();
    public ConstructionSet constructionSet;
    public List<GameObject> PlayersPrefabs = new List<GameObject>();
    public List<GameObject> BulletsPrefabs = new List<GameObject>();
    public List<Weapon> weapons = new List<Weapon>();
    WeaponSpawner[] ws;
    public Dictionary<string,GameObject> players = new Dictionary<string, GameObject>();
    int playerId;
    public GameObject localPlayer;
    public GameObject pauseMenu;
    PlayerMove localPlayerMove;
    


    public GameObject selectScreen;
    public Text Log;

    System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");

    

    // Start is called before the first frame update
    void Start()
    {
        
        GameManager.instance.roomManager = this;
        RenderMap(GameManager.instance.futureMap);
        GameManager.RequestPlayers();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
            if(localPlayerMove != null) { localPlayerMove.inPause = pauseMenu.activeInHierarchy; }
            
        }
    }

    void RenderMap(SocketIOEvent e){
        Debug.Log(e.data);
        List<JSONObject> jSONs = e.data["mapObjects"].list;
        for(int i = 0; i < jSONs.Count; i++){
            float x;
            float y;
            float.TryParse(jSONs[i]["x"].ToString().Replace("\"", "").Replace(",", "."), System.Globalization.NumberStyles.Any, culture, out x);
            float.TryParse(jSONs[i]["y"].ToString().Replace("\"", "").Replace(",", "."), System.Globalization.NumberStyles.Any, culture, out y);
            int ID = int.Parse(jSONs[i]["ID"].ToString().Replace("\"", ""));
            Vector3 pos = new Vector3(x,y, constructionSet.prefabs[ID].transform.position.z);
            
            var go = Instantiate(constructionSet.prefabs[ID],pos,Quaternion.identity);
        }
        ws = GameObject.FindObjectsOfType<WeaponSpawner>();
    }
    public void sePlayerId(int id){
        playerId = id;
        string text = "Ability:";
        StatsState ss = PlayersPrefabs[id].GetComponent<PlayerMove>().Ability;
        text += "\n<size=18>Duration: " + ss.stateDuration + "\nVelocity multiplier: x" + ss.stateVelocityMultiplier + "\nWeapon velocity M: x" + ss.stateWeaponVelocityMultiplier + "\nDamage multiplier: x" + ss.stateDamageMultiplier + "</size>";
    }
    public void Play(){
        selectScreen.SetActive(false);
        GameObject[] spawns = GameObject.FindGameObjectsWithTag("Respawn");
        int spawn = Random.Range(0,spawns.Length);
        var go = Instantiate(PlayersPrefabs[playerId],spawns[spawn].transform.position,Quaternion.identity);
        go.GetComponent<PlayerMove>().localPlayer = true;
        localPlayer = go;
        localPlayerMove = localPlayer.GetComponent<PlayerMove>();
        GameManager.Render(playerId);
    }

    public void AddPlayer(SocketIOEvent e){
        Debug.Log("New player");
        string user = e.data["user"].ToString().Replace("\"", "");
        string id = e.data["playerID"].ToString().Replace("\"", "");
        AddToLog(string.Format(@"{0} are connected", user));
        var go = Instantiate(PlayersPrefabs[0],new Vector3(1f,1f,0f),Quaternion.identity);
        go.GetComponent<PlayerMove>().user = user;
        players.Add(id,go);
        if(localPlayer != null){
            GameManager.Render(playerId);
        }else{
            GameManager.NoRender();
        }
        
    }
    public void DeletePlayer(SocketIOEvent e){
        
        string id = e.data["playerID"].ToString().Replace("\"", "");
        GameObject go = players[id];
        AddToLog(string.Format(@"<color=blue><b>{0}</b></color> are disconnected", go.GetComponent<PlayerMove>().user ));
        players.Remove(id);
        Destroy(go);
    }
    public void NoRender(SocketIOEvent e){
        Debug.Log("No Render");
        string id = e.data["playerID"].ToString().Replace("\"", "");
        players[id].GetComponent<BoxCollider2D>().enabled = false;
        players[id].GetComponent<PlayerMove>().enabled = false;
        players[id].GetComponent<SpriteRenderer>().enabled = false;
        players[id].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    public void Render(SocketIOEvent e){
        Debug.Log("New render");
        string id = e.data["playerID"].ToString().Replace("\"", "");
        int cid = int.Parse(e.data["characterID"].ToString().Replace("\"", ""));

        GameObject go = players[id];
        string user = go.GetComponent<PlayerMove>().user;
        
        var ngo = Instantiate(PlayersPrefabs[cid],new Vector3(1f,1f,0f),Quaternion.identity);

        AddToLog(string.Format(@"<color=blue><b>{0}</b></color> select character number {1}", user, cid));
        players.Remove(id);
        Destroy(go);
        players.Add(id,ngo);

        players[id].GetComponent<PlayerMove>().user = user;


        Destroy(go);
    }

    public void UpdatePlayer(SocketIOEvent e){
        //Debug.Log(e.data.ToString());
        
        string id = e.data["playerID"].ToString().Replace("\"", "");
        
        float x;
        float y;
        float.TryParse(e.data["PX"].ToString().Replace("\"", "").Replace(",", "."), System.Globalization.NumberStyles.Any, culture, out x);
        float.TryParse(e.data["PY"].ToString().Replace("\"", "").Replace(",", "."), System.Globalization.NumberStyles.Any, culture, out y);
        Vector3 pos = new Vector3(x,y,0f);
        float vx;
        float vy;
        float.TryParse(e.data["VX"].ToString().Replace("\"", "").Replace(",", "."), System.Globalization.NumberStyles.Any, culture, out vx);
        float.TryParse(e.data["VY"].ToString().Replace("\"", "").Replace(",", "."), System.Globalization.NumberStyles.Any, culture, out vy);
        Vector3 vel = new Vector3(vx,vy,0f);
        float lx;
        float ly;
        float.TryParse(e.data["LX"].ToString().Replace("\"", "").Replace(",", "."), System.Globalization.NumberStyles.Any, culture, out lx);
        float.TryParse(e.data["LY"].ToString().Replace("\"", "").Replace(",", "."), System.Globalization.NumberStyles.Any, culture, out ly);
        Vector2 dir = new Vector2(lx, ly);
        float H;
        float V;
        float.TryParse(e.data["H"].ToString().Replace("\"", "").Replace(",", "."), System.Globalization.NumberStyles.Any, culture, out H);
        float.TryParse(e.data["V"].ToString().Replace("\"", "").Replace(",", "."), System.Globalization.NumberStyles.Any, culture, out V);
        float healt = int.Parse(e.data["healt"].ToString().Replace("\"", ""));
        players[id].GetComponent<PlayerMove>().setUser(pos,vel,H,V,healt, dir);
    }

    public void SendRoomInfo(SocketIOEvent e)
    {
        
        string weapons = "[";

        for(int i = 0; i < ws.Length; i++){
            string weaponData = string.Format(@"{{ ""ID"":""{0}"",""state"":""{1}"",""weapon"":""{2}"" }}", i,ws[i].currentState, ws[i].currentWeapon);
            weapons += weaponData;
            if(i != ws.Length - 1)
            {
                weapons += ",";
            }
        }
        weapons += "]";

        string data = string.Format(@"{{""WeaponSpawner"":{0} }}", weapons);

        GameManager.SimpleSend("RoomInfo", data);
        
    }

    public void SetRoomInfo(SocketIOEvent e)
    {
        List<JSONObject> jSONs = e.data["WeaponSpawner"].list;
        foreach(JSONObject jSON in jSONs)
        {
            int ID = int.Parse( jSON["ID"].ToString().Replace("\"", "") );
            bool state = bool.Parse(jSON["state"].ToString().Replace("\"", ""));
            int weapon = int.Parse(jSON["weapon"].ToString().Replace("\"", ""));
            ws[ID].Set(weapon, state);
        }
    }

    public void Pick(SocketIOEvent e)
    {
        int wws = int.Parse(e.data["WeaponSpawner"].ToString().Replace("\"", ""));
        ws[wws].Set(0, false);
    }

    public void Tick()
    {
        foreach(WeaponSpawner w in ws)
        {
            w.spawnWeapon();
        }
        SendRoomInfo(new SocketIOEvent("null"));
    }

    public int getIndex(WeaponSpawner wws)
    {

        for(int i = 0; i < ws.Length; i++)
        {
            if (wws == ws[i])
            {
                return i;
            }
        }
        return 0;
    }

    public void Shoot(SocketIOEvent e)
    {

        float rx;
        float ry;
        float.TryParse(e.data["RX"].ToString().Replace("\"", "").Replace(",", "."), System.Globalization.NumberStyles.Any, culture, out rx);
        float.TryParse(e.data["RY"].ToString().Replace("\"", "").Replace(",", "."), System.Globalization.NumberStyles.Any, culture, out ry);

        Vector2 dir = new Vector2(rx,ry);
        float x;
        float y;
        float.TryParse(e.data["X"].ToString().Replace("\"", "").Replace(",", "."), System.Globalization.NumberStyles.Any, culture, out x);
        float.TryParse(e.data["Y"].ToString().Replace("\"", "").Replace(",", "."), System.Globalization.NumberStyles.Any, culture, out y);
        Vector2 pos = new Vector2(x,y);
        int pref = int.Parse(e.data["Bullet"].ToString().Replace("\"", ""));
        int damage = int.Parse(e.data["Damage"].ToString().Replace("\"", ""));
        string shoter = e.data["Shoter"].ToString().Replace("\"", "");
        var go = Instantiate(BulletsPrefabs[pref], pos, Quaternion.identity);
        go.GetComponent<SimpleBullet>().shoter = shoter;
        go.GetComponent<SimpleBullet>().damage = damage;
        go.transform.right = dir;

    }

    public void Disconnect()
    {
        GameManager.SimpleSend("DisconnectRoom", string.Format(@"{{ ""kills"":""{0}"", ""deads"":""{1}"", ""money"":""{2}"" }}", playerStats.kills, playerStats.deads, playerStats.money));
        GameManager.instance.LoadScene("Lobby");
    }
    public void Reanude()
    {
        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
        if (localPlayerMove != null) { localPlayerMove.inPause = pauseMenu.activeInHierarchy; }
    }
    public void Die(string killer)
    {
        Destroy(localPlayer);
        localPlayer = null;
        localPlayerMove = null;
        selectScreen.SetActive(true);
        GameManager.NoRender();
        GameManager.SimpleSend("Die", string.Format(@"{{ ""Killer"":""{0}"",""Kill"":""{1}"" }}",killer, GameManager.instance.myUser));
        AddToLog(string.Format(@"<color=red><b>{0}</b></color> killed <b>You</b>", killer));
        playerStats.deads++;

    }
    public void AddToLog(string text)
    {
        Log.text += "\n" + text;
    }

    public void showPositions()
    {
        string txt = "";
        foreach(KeyValuePair<string,GameObject> go in players)
        {
            txt += go.Key + ":";
            txt += "( " + go.Value.transform.position.x + "," + go.Value.transform.position.y + " )";
            txt += "\n";
        }

        Debug.Log(txt);
    }
}
