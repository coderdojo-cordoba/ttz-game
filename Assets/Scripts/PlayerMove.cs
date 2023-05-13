using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StatsState
{
    public string stateName;
    public float stateDuration;
    public float stateRechargeDuration;
    public float stateVelocityMultiplier;
    public float stateWeaponVelocityMultiplier;
    public float stateDamageMultiplier;

    public bool stateAffectsOthers;

    public float stateOthersDistance;
    public float stateOthersDamage;

    public float timeTo;
    

}

public class PlayerMove : MonoBehaviour
{

    public StatsState NormalState;
    public StatsState Ability;
    StatsState CurrentState;

    public float speed;
    public string user;
    Rigidbody2D rb2d;
    Animator anim;

    public bool localPlayer = false;

    public int index = 0;
    public Text nick;
    public RectTransform Healt;
    public RectTransform AbilityRecharge;

    public float maxHealt = 0;
    public float healt;

    int cols;


    // Inventory
    public Text pickUI;
    public WeaponSpawner currentWS;
    AudioSource shot;
    Text charge;
    // Weapon
    public Weapon[] Inventory = new Weapon[4];
    public GameObject[] places = new GameObject[4];

    RoomManager roomManager;
    int selected;

    public Color weaponSelected;
    public Color weaponNoSelected;

    public bool inPause;

    public Sprite AbilityNone;
    public Sprite AbilitySuccess;
    public Sprite WeaponNone;

    public Transform light;

    public GameObject ImagePrefabs;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Created " + localPlayer);
        currentWS = null;
        pickUI = GameObject.Find("pick").GetComponent<Text>();
        CurrentState = NormalState;
        nick = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        Healt = transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<RectTransform>();
        AbilityRecharge = GameObject.Find("AbilityLevel").GetComponent<RectTransform>();
        


        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        shot = GetComponent<AudioSource>();
        if(localPlayer){
            pickUI.text = "";
            AbilityRecharge.parent.GetComponent<Image>().sprite = AbilityNone;
            charge = GameObject.Find("weaponCharge").GetComponent<Text>();
            charge.text = "-/-";
            pickUI = GameObject.Find("pick").GetComponent<Text>();
            GameManager.Render(index);
            healt = maxHealt;
            nick.text = GameManager.instance.myUser;
            roomManager = GameObject.FindObjectOfType<RoomManager>();
            for(int i = 0; i < places.Length; i++)
            {
                places[i] = GameObject.Find("Place" + i);
                places[i].transform.GetChild(0).GetComponent<Image>().sprite = WeaponNone;
                places[i].transform.GetChild(0).GetComponent<Image>().color = weaponNoSelected;
            }
            places[0].transform.GetChild(0).GetComponent<Image>().color = weaponSelected;
        } else
        {
            nick.text = user;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!localPlayer){return;}

        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 dir = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);

        light.right = dir;

        float H = Input.GetAxis("Horizontal");
        float V = Input.GetAxis("Vertical");

        rb2d.velocity = new Vector2(H*speed * CurrentState.stateVelocityMultiplier, V*speed*CurrentState.stateVelocityMultiplier);
        anim.SetFloat("X",H);
        anim.SetFloat("Y",V);
        Camera.main.transform.position = new Vector3(transform.position.x,transform.position.y,-10f);

        string data = string.Format(@"{{  ""PX"":""{0}"",""PY"":""{1}"",""VX"":""{2}"",""VY"":""{3}"",""H"":""{4}"",""V"":""{5}"",""playerID"":""{6}"", ""healt"":""{7}"", ""LX"":""{8}"", ""LY"":""{9}""}}", transform.position.x.ToString(),transform.position.y.ToString(), rb2d.velocity.x.ToString(), rb2d.velocity.y.ToString(), H.ToString(), V.ToString(), GameManager.instance.myId,(int)healt, dir.x, dir.y);
        GameManager.updatePlayer(data);
        
    }

    private void Update()
    {
        if (inPause || !localPlayer)
        {
            return;
        }

        if (CurrentState == NormalState)
        {
            
            if (Ability.timeTo >= Ability.stateRechargeDuration)
            {
                Ability.timeTo = Ability.stateRechargeDuration;
                AbilityRecharge.sizeDelta = new Vector2(AbilityRecharge.sizeDelta.x,0);
                AbilityRecharge.parent.GetComponent<Image>().sprite = AbilitySuccess;
                if (Input.GetKeyDown(KeyCode.V))
                {
                    Ability.timeTo = Ability.stateDuration;
                    CurrentState = Ability;
                    AbilityRecharge.parent.GetComponent<Image>().sprite = AbilityNone;

                }
            }
            else
            {
                Ability.timeTo += Time.deltaTime;
                AbilityRecharge.sizeDelta = new Vector2(AbilityRecharge.sizeDelta.x, 128 / (Ability.stateRechargeDuration / Ability.timeTo));
            }
            


        }
        else if (CurrentState == Ability)
        {
            Ability.timeTo -= Time.deltaTime;
            if(Ability.timeTo <= 0)
            {
                Ability.timeTo = 0;
                CurrentState = NormalState;
            }
            AbilityRecharge.sizeDelta = new Vector2(AbilityRecharge.sizeDelta.x, 128 / (Ability.stateDuration / Ability.timeTo));
        }


       

        // Shoot
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            shot.clip = Inventory[selected].weaponShotAudio;
            if(Inventory[selected].weaponSprite != null)
            {
                StartCoroutine(Shoot(transform.position, new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y), Inventory[selected]));
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            shot.clip = Inventory[selected].weaponRechargeAudio;
            if (Inventory[selected].weaponSprite != null)
            {
                StartCoroutine(Recharge(Inventory[selected]));
            }
        }
        else
        // Inventory keys
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            places[selected].transform.GetChild(0).GetComponent<Image>().color = weaponNoSelected;
            selected = 0;
            places[selected].transform.GetChild(0).GetComponent<Image>().color = weaponSelected;
            if (Inventory[selected].weaponSprite != null)
            {
                charge.text = Inventory[selected].weaponCharger + "/" + Inventory[selected].weaponReserve;
            }
            else
            {
                charge.text = "-/-";
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            places[selected].transform.GetChild(0).GetComponent<Image>().color = weaponNoSelected;
            selected = 1;
            places[selected].transform.GetChild(0).GetComponent<Image>().color = weaponSelected;
            if (Inventory[selected].weaponSprite != null)
            {
                charge.text = Inventory[selected].weaponCharger + "/" + Inventory[selected].weaponReserve;
            }
            else
            {
                charge.text = "-/-";
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            places[selected].transform.GetChild(0).GetComponent<Image>().color = weaponNoSelected;
            selected = 2;
            places[selected].transform.GetChild(0).GetComponent<Image>().color = weaponSelected;
            if (Inventory[selected].weaponSprite != null)
            {
                charge.text = Inventory[selected].weaponCharger + "/" + Inventory[selected].weaponReserve;
            }
            else
            {
                charge.text = "-/-";
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            places[selected].transform.GetChild(0).GetComponent<Image>().color = weaponNoSelected;
            selected = 3;
            places[selected].transform.GetChild(0).GetComponent<Image>().color = weaponSelected;
            if (Inventory[selected].weaponSprite != null)
            {
                charge.text = Inventory[selected].weaponCharger + "/" + Inventory[selected].weaponReserve;
            }
            else
            {
                charge.text = "-/-";
            }
        }
        // Pick Weapon
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(currentWS != null)
            {
                GameManager.SimpleSend("Pick", string.Format(@"{{""WeaponSpawner"":""{0}""}}",roomManager.getIndex(currentWS) ));
                bool ok = false;
                for(int i = 0; i < Inventory.Length; i++)
                {
                    if(Inventory[i].weaponSprite == null)
                    {
                        ok = true;
                        Weapon w = (Weapon)currentWS.GetWeapon().Clone();

                        if(w != null)
                        {
                            Inventory[i] = w;
                            places[i].transform.GetChild(0).GetComponent<Image>().sprite = Inventory[i].weaponSprite;
                            if (selected == i)
                            {
                                charge.text = Inventory[i].weaponCharger + "/" + Inventory[i].weaponChargerMax;
                            }
                            break;
                        }
                        
                    }
                }
                if (!ok)
                {
                    if(currentWS != null)
                    {
                        Weapon w = (Weapon)currentWS.GetWeapon().Clone();
                        if(w != null)
                        {
                            Inventory[selected] = w;

                            places[selected].transform.GetChild(0).GetComponent<Image>().sprite = Inventory[selected].weaponSprite;
                            charge.text = Inventory[selected].weaponCharger + "/" + Inventory[selected].weaponChargerMax;
                        }
                        
                    }
                }
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other) {
        
        if (other.tag == "Alpha"){
            if (!localPlayer)
            {
                cols++;
                //GetComponent<SpriteRenderer>().sortingOrder = 0;
            }
            //Color tmp = other.transform.parent.GetComponent<SpriteRenderer>().color;
            //tmp.a = 0.5f;
            //other.transform.parent.GetComponent<SpriteRenderer>().color = tmp;
            other.transform.parent.GetComponent<SpriteRenderer>().sortingOrder = 2;
        }
        if (other.tag == "Weapon")
        {
            if (localPlayer)
            {
                WeaponSpawner ws = other.GetComponent<WeaponSpawner>();
                currentWS = ws;
                if (currentWS.currentState)
                {
                    pickUI.text = "Press E to pick up " + roomManager.weapons[currentWS.currentWeapon].weaponName;
                }
                else
                {
                    pickUI.text = "";
                }
            }
           
        }

    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.tag == "Alpha"){
            if (!localPlayer)
            {
                cols--;
                if(cols == 0)
                {
                    GetComponent<SpriteRenderer>().sortingOrder = 1;
                }
                
            }
            //Color tmp = other.transform.parent.GetComponent<SpriteRenderer>().color;
            //tmp.a = 1f;
            //other.transform.parent.GetComponent<SpriteRenderer>().color = tmp;
            other.transform.parent.GetComponent<SpriteRenderer>().sortingOrder = 0;
        }
        if(other.tag == "Weapon")
        {
            WeaponSpawner ws = other.GetComponent<WeaponSpawner>();
            if(currentWS == ws)
            {
                currentWS = null;
                pickUI.text = "";
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Bullet")
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 0.2f, 0.2f, 1f);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.transform.tag == "Bullet")
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }
    }

    public void setUser(Vector3 pos, Vector3 vel, float H, float V, float h, Vector2 dir){
        transform.position = pos;
        rb2d.velocity = vel;
        anim.SetFloat("X",H);
        anim.SetFloat("Y",V);
        Healt.GetComponent<RectTransform>().sizeDelta = new Vector2(1200f * (h / maxHealt), Healt.GetComponent<RectTransform>().sizeDelta.y);
        light.right = dir;
    }

    IEnumerator Shoot(Vector2 pos, Vector2 dir, Weapon w)
    {
        if (w.weaponCanShot)
        {
            if (w.weaponCharger >= w.weaponBulletsPerShot)
            {
                GameManager.Shoot(pos, dir, w.weaponBulletPref, (int)(w.weaponDamage * CurrentState.stateWeaponVelocityMultiplier));
                w.weaponCanShot = false;
                w.weaponCharger-= w.weaponBulletsPerShot;
                charge.text = w.weaponCharger + "/" + w.weaponReserve;
                shot.Play();
                yield return new WaitForSeconds(w.weaponCadency * CurrentState.stateWeaponVelocityMultiplier);
                w.weaponCanShot = true;
            }

        }
    }

    IEnumerator Recharge(Weapon w)
    {
        if (w.weaponCharger < w.weaponChargerMax)
        {
            Debug.Log("Recharge");
            yield return new WaitForSeconds(w.weaponRechargeTime * CurrentState.stateWeaponVelocityMultiplier);
            int missing = w.weaponChargerMax - w.weaponCharger;
            if (w.weaponReserve >= missing)
            {
                w.weaponReserve -= missing;
                w.weaponCharger += missing;
            }
            else
            {
                w.weaponCharger += w.weaponReserve;
                w.weaponReserve = 0;
            }
            charge.text = w.weaponCharger + "/" + w.weaponReserve;
        }

    }

    public void takeDamage(int damage, string from)
    {
        healt -= (int)(damage * CurrentState.stateDamageMultiplier);
        if (healt <= 0)
        {
            healt = 0;
            roomManager.Die(from);
        }

        Healt.GetComponent<RectTransform>().sizeDelta = new Vector2(1200f * (healt / maxHealt), Healt.GetComponent<RectTransform>().sizeDelta.y);
    }
}
