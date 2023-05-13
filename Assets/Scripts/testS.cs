using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class testC
{
    public string name = "text";
    public Sprite sprite;
    
    public object Clone()
    {
        return this.MemberwiseClone();
    }
}

public class testS : MonoBehaviour
{
    float speed = 1f;
    float duration = 2.5f;
    public GameObject notification;
    List<GameObject> notifications = new List<GameObject>();
    List<GameObject> notificationsDeleted = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var go = Instantiate(notification, GameObject.Find("Canvas").transform);
            go.transform.localPosition = new Vector3(704f, -590f, 0f);
            notifications.Insert(0, go);
            Invoke("RemoveLast", duration);

        }

        if(notifications.Count != 0)
        {
            if (notifications[notifications.Count-1].transform.position.y != -490f)
            {
                foreach(var n in notifications)
                {
                    Vector3 to = new Vector3(704f, -590f + (notifications.IndexOf(n)+1) * 100, 0f);
                    n.transform.localPosition = Vector3.Lerp(n.transform.localPosition, to, speed * Time.deltaTime);
                }
            }
        }

        if(notificationsDeleted.Count != 0)
        {
            foreach(var nd in notificationsDeleted)
            {
                Vector3 to = new Vector3(nd.transform.localPosition.y + 100f, nd.transform.localPosition.x + 100f, 0f);
                nd.transform.localPosition = Vector3.Lerp(nd.transform.localPosition, to, speed * Time.deltaTime);
                
            }
        }

    }

    void RemoveLast()
    {
        notificationsDeleted.Add(notifications[notifications.Count - 1]);
        notifications.RemoveAt(notifications.Count - 1);
        Invoke("remove", 5f);

    }
    void remove()
    {
        var go = notificationsDeleted[notificationsDeleted.Count - 1];
        notificationsDeleted.RemoveAt(notifications.Count - 1);
        Destroy(go);
    }
}
