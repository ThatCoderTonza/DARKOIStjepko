using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sniper1Shoot : MonoBehaviour
{
    //pauza za delay pucanja
    float ShotDelayTimer = 0;
    //GLOBALNO VRIJEME
    float GlobalTimer;
    //bullet izgled
    public Rigidbody BulletPrefab;
    //bullet gameobject
    Rigidbody[] Bullet = new Rigidbody[30];
    //sniper
    public GameObject Sniper;
    //INDEX ZA ARRAY BULLETA
    int BulletIndex = 0;
    //provjera je li bullet stvoren
    bool BulletCreated = false;
    //PRAZAN OBJEKT
    public Rigidbody empty;
    //DESTROY BULLET SCRIPT
    GameObject[] DestroyBullet = new GameObject[30];
    //player
    public GameObject player;
    //stranica c hipotenuza
    float c;
    //dodatni angle;
    float HoldAngle;
    //weapon place
    public GameObject weapon_place;
    //weapon place top
    public GameObject weapon_place_top;


    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {
        //izračunati hipotenuzu
        c = MathF.Sqrt((player.transform.localScale.x / 2 + Sniper.transform.localScale.x / 2) * (player.transform.localScale.x / 2 + Sniper.transform.localScale.x / 2) + (player.transform.localScale.z / 2 + Sniper.transform.localScale.z / 2) * (player.transform.localScale.z / 2 + Sniper.transform.localScale.z / 2));
       //postaviti koliko je dugačak štap na kojem se nalazi puška. mora bit duplo duži jer u centru je s playerom
        weapon_place.transform.localScale = new Vector3(0.3f, 0.3f, c * 2 + 1);
        //puška dolazi na vrh tog štapa
        Sniper.transform.position = weapon_place_top.transform.position;
        //puškina rotacija uvijek mora bit 0 da puca ravno
        Sniper.transform.localEulerAngles = new Vector3(0, player.transform.eulerAngles.y, 0);
        ///rotiranje štapa da puška uvijek stoji blizu playera
        weapon_place.transform.localEulerAngles = new Vector3(0, player.transform.eulerAngles.y + (Mathf.Asin((player.transform.localScale.x / 2 + Sniper.transform.localScale.x / 2) / c) * 180 / MathF.PI), 0);
        
        
        //NAĐI SVE BULLETE ZA DESTROYAT
        DestroyBullet = GameObject.FindGameObjectsWithTag("DestroyBullet");
        //globaltimer setup - povećava se
        GlobalTimer += Time.deltaTime;

        //DESTROYAJ
        for (int i = 0; i < DestroyBullet.Length; i++) 
        {
            Destroy(DestroyBullet[i], 0.2f);
        }


        //AKO SE MIŠ KLIKNE 
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ///AKO JE GLOBALTIMER ZA 1 VEĆI OD SHOTDELAYTIMERA NAZOVI FUNKCIJU ZA PUCANJE
            if (GlobalTimer - 1 >= ShotDelayTimer) 
            {
                ShotBullet();
            }    
        }



        if (BulletCreated == true) 
        {
            for (int i = 0; i < BulletIndex; i++) 
            {
                //Debug.Log(i);
                //Debug.Log(Bullet[i]);
                //kretanje bulleta
                Bullet[i].transform.Translate(Vector3.forward * 1f);

                //provjera bi li se bullet trebao destroyat
                if (Vector3.Distance(Bullet[i].transform.position, Sniper.transform.position) >= 1000) 
                {
                    if (Bullet[i] != null) 
                    {
                        if (Bullet[i].gameObject.name == "Bullet(Clone)") 
                        {
                            Bullet[i].gameObject.tag = "DestroyBullet";
                        }
                        Bullet[i] = empty;
                        Debug.Log(Bullet[i]);
                    }
                }
            }
        }
    }



    public void ShotBullet() 
    {
        BulletCreated = true;
        //stvori bullet
        Bullet[BulletIndex] = Instantiate(BulletPrefab, Sniper.transform.position, Sniper.transform.rotation);
        //Debug.Log(Bullet[BulletIndex]);
        //STARTNA POZICIJA
        Bullet[BulletIndex].transform.position = Sniper.transform.position;
        
        //POVEĆA SE INDEX KAKO BI SE MOGAO STVORITI NOVI BULLET
        BulletIndex++;
        
        //SHOTDELAYTIMER POSTAJE GLOBALTIMER DA IMA DELAYA
        ShotDelayTimer = GlobalTimer;

        //ako je bullet index veči od 30 vrati ga na 0
        if (BulletIndex >= 30) 
        {
            BulletIndex = 0;
        }
    }
}
