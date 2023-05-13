using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorManager : MonoBehaviour
{
    public ConstructionSet constructionSet;
    public List<GameObject> ObjectsMap = new List<GameObject>();
    public int currentPrefab = 0;
    public int spawnerPrefab = 0;
    Transform cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;

        int order = constructionSet.prefabs[spawnerPrefab].GetComponent<SpriteRenderer>().sortingOrder;
            var go = Instantiate(constructionSet.prefabs[spawnerPrefab],new Vector3(0f,0f,constructionSet.prefabs[spawnerPrefab].transform.position.z),Quaternion.identity);
            go.name = spawnerPrefab.ToString();
            ObjectsMap.Add(go);
    }

    // Update is called once per frame
    void Update()
    {


        // Camera move

        Vector3 pm = Input.mousePosition;
        Vector3 p = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        Vector3 pint = new Vector3((int)p.x,(int)p.y);
        transform.GetChild(0).transform.position = new Vector2((int)p.x,(int)p.y);
        if(pm.x < 10f){
            cam.position = cam.position - new Vector3(0.2f,0f,0f);
        }
        if(pm.x > Screen.width-10){
            cam.position = cam.position + new Vector3(0.2f,0f,0f);
        }
        if(pm.y < 10f){
            cam.position = cam.position - new Vector3(0f,0.2f,0f);
        }
        if(pm.y > Screen.height-10){
            cam.position = cam.position + new Vector3(0f,0.2f,0f);
        }

        // Add & Remove ConstructionBlocks

        if(Input.GetMouseButtonDown(0)){
            if(pm.y > Mathf.Round( Screen.height-(Screen.height/100*25f))){
                return;
            }
            int order = constructionSet.prefabs[currentPrefab].GetComponent<SpriteRenderer>().sortingOrder;
            foreach (var pref in ObjectsMap)
            {
                if(pref.transform.position.x == pint.x && pref.transform.position.y == pint.y && pref.GetComponent<SpriteRenderer>().sortingOrder == order ){return;}
            }
            pint.z = constructionSet.prefabs[currentPrefab].transform.position.z;
            var go = Instantiate(constructionSet.prefabs[currentPrefab],pint,Quaternion.identity);
            go.name = currentPrefab.ToString();
            ObjectsMap.Add(go);
        }

        if(Input.GetMouseButton(0) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            if (pm.y > Mathf.Round(Screen.height - (Screen.height / 100 * 25f)))
            {
                return;
            }
            int order = constructionSet.prefabs[currentPrefab].GetComponent<SpriteRenderer>().sortingOrder;
            foreach (var pref in ObjectsMap)
            {
                if (pref.transform.position.x == pint.x && pref.transform.position.y == pint.y && pref.GetComponent<SpriteRenderer>().sortingOrder == order) { return; }
            }
            pint.z = constructionSet.prefabs[currentPrefab].transform.position.z;
            var go = Instantiate(constructionSet.prefabs[currentPrefab], pint, Quaternion.identity);
            go.name = currentPrefab.ToString();
            ObjectsMap.Add(go);
        }

        if(Input.GetMouseButtonDown(1)){
            if(pm.y > Mathf.Round( Screen.height-(Screen.height/100*20f))){
                return;
            }
            foreach (var pref in ObjectsMap)
            {
                if(pref.transform.position.x == pint.x && pref.transform.position.y == pint.y){
                    ObjectsMap.Remove(pref);
                    Destroy(pref.gameObject);
                    return;
                }
            }
            
            
        }

        if (Input.GetMouseButton(1) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            if (pm.y > Mathf.Round(Screen.height - (Screen.height / 100 * 20f)))
            {
                return;
            }
            foreach (var pref in ObjectsMap)
            {
                if (pref.transform.position.x == pint.x && pref.transform.position.y == pint.y)
                {
                    ObjectsMap.Remove(pref);
                    Destroy(pref.gameObject);
                    return;
                }
            }
        }
    }

    public void setPrefab(int prefab){
        currentPrefab = prefab;
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = constructionSet.prefabs[currentPrefab].GetComponent<SpriteRenderer>().sprite;
    }

    public string GetObjects(){
        string data = "[";
        foreach (var pref in ObjectsMap)
        {
            string objS = string.Format(@"{{""ID"":""{0}"",""x"":""{1}"",""y"":""{2}""}}", int.Parse(pref.name), pref.transform.position.x,pref.transform.position.y);
            data += objS;
            if(ObjectsMap.IndexOf(pref) != ObjectsMap.Count-1){
                data += ",";
            }
        }
        data += "]";
        Debug.Log(data);


        return data;
    }

    public int GetID(Sprite sprite, List<GameObject> Prefabs){
        foreach (var go in Prefabs)
        {
            if(go.GetComponent<SpriteRenderer>().sprite == sprite){
                return Prefabs.IndexOf(go);
            }
        }
        return 0;
    }

    public void exitEditor(bool save){
        GameManager.exitEditor(save,GetObjects());
    }
}
