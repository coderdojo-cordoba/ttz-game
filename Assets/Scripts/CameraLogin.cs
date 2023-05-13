using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PositionsStates
{
    public Vector3 position;
    public List<GameObject> objectsToSpawn = new List<GameObject>();
    public List<Vector3> positions = new List<Vector3>();
}

public class CameraLogin : MonoBehaviour
{

    

    public float speed;
    public float stopTime;
    public List<PositionsStates> positions = new List<PositionsStates>();
    float counter;
    bool inMove = true;
    int to = 0;
    bool spawned = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inMove)
        {
            transform.position = Vector3.Lerp(transform.position, positions[to].position, speed * Time.deltaTime);
            if(Vector3.Distance(transform.position, positions[to].position) < 1f && !spawned)
            {
                for (int i = 0; i < positions[to].objectsToSpawn.Count; i++)
                {
                    Vector3 pos = new Vector3(positions[to].positions[i].x, positions[to].positions[i].y, 0f);
                    Vector3 euler = new Vector3(0f, 0f, positions[to].positions[i].z);
                    var go = Instantiate(positions[to].objectsToSpawn[i], pos, Quaternion.identity);
                    go.transform.eulerAngles = euler;
                }
                spawned = true;
            }
            if (Vector3.Distance(transform.position, positions[to].position) < 0.1f)
            {
                transform.position = positions[to].position;
                inMove = false;
                
                to++;
                if (to == positions.Count) { to = 0; }
                
            }
        }
        else
        {
            counter += Time.deltaTime;
            if (counter >= stopTime)
            {
                inMove = true;
                spawned = false;
                counter = 0f;
            }
        }
    }

    public void Login()
    {
        GameManager.instance.Login();
    }
    public void Register()
    {
        GameManager.instance.Register();
    }
}
