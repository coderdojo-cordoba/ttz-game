using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SocketIO;

public class GameManager : MonoBehaviour
{
    public string version;
    public Texture2D cursorNoClick;
    public Texture2D cursorClick;

    public static GameManager instance = null;
    public GameObject alertBox;
    public RoomManager roomManager;
    static SocketIOComponent socket;
    public GameObject Loader;
    public string myId;
    public string myUser;
    public string mySeparatorFrom;
    public string mySeparatorTo;

    bool inEditor;
    bool isPublicEditor;
    string nameEditor;
    public SocketIOEvent futureMap;
    bool alert;
    private void Awake() {
        if (instance == null)
                
                //if not, set instance to this
            instance = this;
            
            //If instance already exists and it's not this:
        else if (instance != this)
                
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);    
            
            //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {


        socket = GetComponent<SocketIOComponent>();
        socket.On("setId",setId);
        socket.On("LoginResponse",loginResponse);
        socket.On("RegisterResponse",registerResponse);
        socket.On("RoomsUpdate",roomsUpdate);
        socket.On("MapsUpdate",mapsUpdate);
        socket.On("ConnectToRoom",OnConnectToRoom);
        socket.On("AddPlayer",OnAddPlayer);
        socket.On("DeletePlayer",OnDeletelayer);
        socket.On("NoRender", OnNoRender);
        socket.On("Render", OnRender);
        socket.On("UpdatePlayer", OnUpdatePlayer);
        socket.On("RoomInfoRequest", OnRoomInfoRequest);
        socket.On("RoomInfo", OnRoomInfo);
        socket.On("Tick", OnTick);
        socket.On("Pick", OnPick);
        socket.On("Shoot", OnShoot);
        socket.On("OpenMapEditor", OnOpenMapEditor);
        socket.On("Die", OnDie);
        socket.On("ToPreview", OnToPreview);
        socket.On("version", OnVersion);

        string email = PlayerPrefs.GetString("Email", "-");
        string password = PlayerPrefs.GetString("Password", "-");
        if(email != "-" && password != "-")
        {
            Login(email, password);
        }

        Invoke("changeToLogin", 12f);
        Invoke("destroy", 13.1f);
    }

    void changeToLogin()
    {
        if (alert)
        {
            return;
        }
        LoadScene("Login");
    }
    void destroy()
    {
        if (alert)
        {
            return;
        }
        Destroy(GameObject.Find("SEP"));
        
    }

    void setId(SocketIOEvent e){
        myId = e.data["id"].ToString().Replace("\"", "");
        Debug.Log("your ID: " + myId);
    }
    void loginResponse(SocketIOEvent e){
        bool login = bool.Parse(e.data["login"].ToString().Replace("\"", ""));
        Debug.Log(e.data.ToString() + ":::" + login);
        if(login){
            
            myUser = e.data["u"]["nickname"].ToString().Replace("\"", "");
            Debug.Log(myUser);
            string email = GameObject.Find("LoginEmail").GetComponent<InputField>().text;
            string password = GameObject.Find("LoginPassword").GetComponent<InputField>().text;

            PlayerPrefs.SetString("Email", email);
            PlayerPrefs.SetString("Password", password);

            var go = Instantiate(Loader,Vector3.zero, Quaternion.identity, GameObject.Find("Canvas").transform);
            go.GetComponent<RectTransform>().position = new Vector3(go.GetComponent<RectTransform>().sizeDelta.x/2,go.GetComponent<RectTransform>().sizeDelta.y/2,0f);
            StartCoroutine(LoadNewScene("Lobby"));
        }else{
            GameObject.Find("Response").GetComponent<Text>().text = "<color=red>Login failed</color>";
        }
    }
    void registerResponse(SocketIOEvent e){
        bool register = bool.Parse(e.data["register"].ToString().Replace("\"", ""));
        if(register){
            GameObject.Find("Response").GetComponent<Text>().text = "<color=green>Register successful</color>";
        }else{
            GameObject.Find("Response").GetComponent<Text>().text = "<color=red>Register failed</color>";
        }
    }
    void roomsUpdate(SocketIOEvent e){
        Debug.Log(e.data.ToString());
        GameObject.FindObjectOfType<LobbyManager>().currentRooms = e;
        GameObject.FindObjectOfType<LobbyManager>().RenderRoom();
    }
    void mapsUpdate(SocketIOEvent e){
        GameObject.FindObjectOfType<LobbyManager>().currentMaps = e;
        GameObject.FindObjectOfType<LobbyManager>().RenderMaps();
    }
    void OnConnectToRoom(SocketIOEvent e){
        futureMap = e;
        var go = Instantiate(Loader,Vector3.zero, Quaternion.identity, GameObject.Find("Canvas").transform);
        go.GetComponent<RectTransform>().position = new Vector3(go.GetComponent<RectTransform>().sizeDelta.x/2,go.GetComponent<RectTransform>().sizeDelta.y/2,0f);
        StartCoroutine(LoadNewScene("Main"));

    }
    void OnAddPlayer(SocketIOEvent e){

        roomManager.AddPlayer(e);
    }
    void OnDeletelayer(SocketIOEvent e){
        roomManager.DeletePlayer(e);
    }

    void OnNoRender(SocketIOEvent e){
        roomManager.NoRender(e);
    }
    void OnRender(SocketIOEvent e){
        roomManager.Render(e);
    }
    void OnDie(SocketIOEvent e)
    {
        string kill = e.data["Kill"].ToString().Replace("\"", "");
        string killer = e.data["Killer"].ToString().Replace("\"", "");
        roomManager.AddToLog(string.Format(@"<color=blue><b>{0}</b></color> killed <color=red><b>{1}</b></color>",killer,kill));
        if(killer == myUser)
        {
            roomManager.playerStats.kills++;
            roomManager.playerStats.money++;
        }
    }
    void OnUpdatePlayer(SocketIOEvent e){
        roomManager.UpdatePlayer(e);
    }

    void OnRoomInfoRequest(SocketIOEvent e)
    {
        roomManager.SendRoomInfo(e);
    }
    void OnRoomInfo(SocketIOEvent e)
    {
        roomManager.SetRoomInfo(e);
    }
    void OnTick(SocketIOEvent e)
    {
        roomManager.Tick();
    }
    void OnPick(SocketIOEvent e)
    {
        roomManager.Pick(e);
    }
    void OnShoot(SocketIOEvent e)
    {
        roomManager.Shoot(e);
    }
    void OnOpenMapEditor(SocketIOEvent e)
    {
        GameObject.FindObjectOfType<LobbyManager>().OnCreateMap(e);
    }
    void OnToPreview(SocketIOEvent e) {
        futureMap = e;
        Debug.Log(e.data);
        LoadScene("PreviewMap");
    }
    void OnVersion(SocketIOEvent e)
    {
        string versionServer = e.data["version"].ToString().Replace("\"", "");
        if (version != versionServer)
        {
            Debug.Log(version + ":::" + versionServer);
            alertBox.SetActive(true);
            alert = true;
            Invoke("Close", 5f);
        }
    }
    public void Login(){
        string email = GameObject.Find("LoginEmail").GetComponent<InputField>().text;
        string password = GameObject.Find("LoginPassword").GetComponent<InputField>().text;
        string data = string.Format(@"{{""email"":""{0}"",""password"":""{1}""}}",email,password);
        socket.Emit("Login",new JSONObject(data));
    }
    public void Login(string email, string password)
    {
        string data = string.Format(@"{{""email"":""{0}"",""password"":""{1}""}}", email, password);
        socket.Emit("Login", new JSONObject(data));
    }
    public void Register(){
        string nickname = GameObject.Find("RegisterNickname").GetComponent<InputField>().text;
        string email = GameObject.Find("RegisterEmail").GetComponent<InputField>().text;
        string password = GameObject.Find("RegisterPassword").GetComponent<InputField>().text;
        if(!string.IsNullOrEmpty(nickname) && !string.IsNullOrWhiteSpace(nickname)){
            if(!string.IsNullOrEmpty(email) && !string.IsNullOrWhiteSpace(email)){
                if(!string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password)){
                    string data = string.Format(@"{{""email"":""{0}"",""password"":""{1}"",""nickname"":""{2}""}}",email,password,nickname);
                    socket.Emit("Register",new JSONObject(data));
                }
            }
        }
        
    }
    IEnumerator LoadNewScene(string scene) {

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

    }

    public void LoadScene(string scene)
    {
        StartCoroutine(instance.LoadNewScene(scene));
    }

    public static void enterInEditor(string name, bool isPublic) {

        instance.nameEditor = name;
        instance.isPublicEditor = isPublic;
        instance.inEditor = true;
        instance.LoadScene("MapEditor");
    }

    public static void exitEditor(bool save, string dataObjs){
        if(save){
            string data = string.Format(@"{{""mapName"":""{0}"",""public"":""{1}"",""objects"":{2} }}", instance.nameEditor, instance.isPublicEditor, dataObjs);
            socket.Emit("SaveMap",new JSONObject(data));
        }
        instance.inEditor = false;
        instance.LoadScene("Lobby");
    }

     

    public static void createRoom(string roomName, string roomMapId, int max){
        string data = string.Format(@"{{""roomName"":""{0}"",""roomMap"":""{1}"",""roomMaxPlayers"":""{2}""}}",roomName,roomMapId,max);
        socket.Emit("createRoom",new JSONObject(data));
    }
    public static void RequestPlayers(){
        socket.Emit("requestPlayers",new JSONObject(""));
    }
    public static void updatePlayer(string data){
        socket.Emit("UpdatePlayer",new JSONObject(data));
    }
    public static void Render(int index){
        string data = string.Format(@"{{""characterID"":""{0}"",""playerID"":""{1}"",""user"":""{2}""}}",index, instance.myId, instance.myUser);
        socket.Emit("Render",new JSONObject(data));

    }
    public static void NoRender(){
        string data = string.Format(@"{{""playerID"":""{0}""}}", instance.myId);
        socket.Emit("NoRender",new JSONObject(data));

    }

    public static void JoinRoom(string id){
        string data = string.Format(@"{{""roomId"":""{0}""}}",id);
        socket.Emit("joinRoom",new JSONObject(data));
    }

    public static void Shoot(Vector2 pos ,Vector2 dir, int pref, int damage)
    {
        string data = string.Format(@"{{ ""RX"":""{0}"", ""RY"":""{1}"", ""Bullet"":""{2}"",""Damage"":""{3}"", ""X"":""{4}"", ""Y"":""{5}"", ""Shoter"":""{6}"" }}",dir.x.ToString(), dir.y.ToString(), pref,damage,pos.x.ToString(), pos.y.ToString(), instance.myUser);
        socket.Emit("Shoot", new JSONObject(data));
    }

    public static void SimpleSend(string context,string data)
    {
        socket.Emit(context, new JSONObject(data));
    }

    void Close()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(cursorClick, new Vector2(32f, 32f), CursorMode.Auto);
        }
        if(Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(cursorNoClick, new Vector2(32f, 32f), CursorMode.Auto);
        }
    }
}
