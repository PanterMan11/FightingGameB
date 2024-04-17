using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


/*
        IMPORTANT TO ANY DEVS THAT WILL WORK ON THIS

  I created a list called allNor which consists of 4 objects which are all a different normal in order of punch,kick,slash,and hSlash
  for the sake of convenience and cross team workability please use these specific list location
   allNor[0] WILL ALWAYS BE PUNCH
   allNor[1] WILL ALWAYS BE KICK
   allNor[2] WILL ALWAYS BE SLASH
   allNor[3] WILL ALWAYS BE HEAVY SLASH
 
    if you change these up two things will happen :

    1) the game WILL NOT EVER function correctly and all the attacks will get messed up
    2) i will personally hunt you down and end ur bloodline :3

    also for any sort of framedata we use the courutine's yield return new WaitForSeconds(float x) function therefore use some of that good tyt matematik
    skills to enter correct framedata
    EX:  
    on 60 fps
    if you want a 20 frame startup use x= 0.3f which is one third of a second aka 20 frames its math if you are a coder you should be able to do this
 */


public class Attack : NetworkBehaviour {

    float VeriInput;
    float HoriInput;
    [SerializeField] Move move_script;
    [NonSerialized] public bool isCoolDown;
    [SerializeField] public List<Normals> allNor = new List<Normals>(4);
    [NonSerialized] public float facing;
    [SerializeField] public enum Attacks // Bread i did this cuz i idiot if u wanna u can fix this lol E
    {
        Punch = 0,
        Kick = 1,
        Slash = 2,
        HSlash = 3,
        None = 100
    };
    [NonSerialized] Attacks currentAttack = Attacks.None;

    
    // Start is called before the first frame update
    void Start()
    {
       foreach(Normals x in allNor){
            x.HitBox.SetActive(false);
        }
    }
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) enabled = false;
    }
    
    // Update is called once per frame
    void Update()
    {
        VeriInput = Input.GetAxisRaw("Vertical");
        HoriInput = Input.GetAxisRaw("Horizontal");
        //Debug.Log("are ya punching yet "+Input.GetButtonUp("Punch"));
    
        CheckForInput();
        
    }

    private void CheckForInput()
    {

        if(Input.GetButtonDown("Punch"))
        {
            currentAttack = Attacks.Punch;
            StartCoroutine(Activate(allNor[0])); // i guess ill have to hardcode all the areas for the allNor list of objects mmmm 
            Debug.Log("ur punchin");
        }

        else if (Input.GetButtonDown("Kick"))
        {
            currentAttack = Attacks.Kick;
            StartCoroutine(Activate(allNor[1])); // i guess ill have to hardcode all the areas for the allNor list of objects mmmm 
            Debug.Log("ur kickin");
        }

        else if (Input.GetButtonDown("Slash"))
        {
            currentAttack = Attacks.Slash;
            
            StartCoroutine(Activate(allNor[2])); // i guess ill have to hardcode all the areas for the allNor list of objects mmmm 
            
            Debug.Log("ur slashin");
        }

        else if (Input.GetButtonDown("HSlash"))
        {
            currentAttack = Attacks.HSlash;
            StartCoroutine(Activate(allNor[3])); // i guess ill have to hardcode all the areas for the allNor list of objects mmmm 
            Debug.Log("ur slashin bigtime");
        }
    }

    public IEnumerator Activate(Normals current)
    {
        move_script.an.SetBool(current.DebugName, true);
        yield return new WaitForSeconds(current.startupFrames);
        current.HitBox.SetActive(true);
        yield return new WaitForSeconds(current.hitFrames);
        current.HitBox.SetActive(false);
        move_script.an.SetBool(current.DebugName, false);
        currentAttack = Attacks.None;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Unit"))
        {
            if (gameObject.GetComponent<Move>().facingRight)
            {
                facing = -1f;
            }
            else
            {
                facing = 1f;
            }
            col.gameObject.GetComponent<UnitProperties>().GetHit(allNor[(int)currentAttack].knockbackPower * facing, allNor[(int)currentAttack].hitPower);
        }
    }
}

[Serializable]
public class Normals
{
    public GameObject HitBox;
    public String DebugName;
    public float startupFrames;
    public float hitFrames;
    public float whiffFrames;
    public float knockbackPower;
    public float hitPower;
}
