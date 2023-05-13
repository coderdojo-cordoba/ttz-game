using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    RoomManager roomManager;
    public GameObject child;
    public int currentWeapon;
    public bool currentState;
    // Start is called before the first frame update
    void Start()
    {
        roomManager = GameObject.FindObjectOfType<RoomManager>();
        child.SetActive(currentState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnWeapon()
    {
        int weaponToSpawn = Random.Range(0, roomManager.weapons.Count);
        if (!currentState)
        {
            child.SetActive(true);
            currentWeapon = weaponToSpawn;
            child.GetComponent<SpriteRenderer>().sprite = roomManager.weapons[weaponToSpawn].weaponSprite;
            currentState = true;
        }
        

    }

    public Weapon GetWeapon()
    {
        
        if (currentState)
        {
            
            child.SetActive(false);
            currentState = false;
            return roomManager.weapons[currentWeapon];
        }
        else
        {
            Debug.Log("-----");
            return null;
        }
        
    }

    public void Set(int weapon, bool state)
    {
        child.SetActive(state);
        currentWeapon = weapon;
        child.GetComponent<SpriteRenderer>().sprite = roomManager.weapons[currentWeapon].weaponSprite;
        currentState = state;
    }
}
