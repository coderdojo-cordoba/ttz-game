using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public ConstructionSet constructionSet;
    Transform cam;
    // Start is called before the first frame update
    void Start()
    {

        cam = Camera.main.transform;
        List<JSONObject> jSONs = GameManager.instance.futureMap.data["objects"].list;
        for (int i = 0; i < jSONs.Count; i++)
        {
            Vector3 pos = new Vector3(float.Parse(jSONs[i]["x"].ToString().Replace("\"", "")), float.Parse(jSONs[i]["y"].ToString().Replace("\"", "")), 0f);
            int ID = int.Parse(jSONs[i]["ID"].ToString().Replace("\"", ""));
            var go = Instantiate(constructionSet.prefabs[ID], pos, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pm = Input.mousePosition;
        if (pm.x < 10f)
        {
            cam.position = cam.position - new Vector3(0.2f, 0f, 0f);
        }
        if (pm.x > Screen.width - 10)
        {
            cam.position = cam.position + new Vector3(0.2f, 0f, 0f);
        }
        if (pm.y < 10f)
        {
            cam.position = cam.position - new Vector3(0f, 0.2f, 0f);
        }
        if (pm.y > Screen.height - 10)
        {
            cam.position = cam.position + new Vector3(0f, 0.2f, 0f);
        }
    }

    public void exit() {
        GameManager.instance.LoadScene("Lobby");
    }
}
