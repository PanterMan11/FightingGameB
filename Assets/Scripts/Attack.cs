using System;
using System.Collections;
using System.Collections.Generic;
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


public class Attack : MonoBehaviour {

    float VeriInput;
    float HoriInput;
    
    [NonSerialized] public bool isCoolDown;
    [SerializeField] public List<Normals> allNor = new List<Normals>(4);
    [SerializeField] public int facing;
    // Start is called before the first frame update
    void Start()
    {
       foreach(Normals x in allNor){
            x.HitBox.SetActive(false);
        }
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
            StartCoroutine(Activate(allNor[0])); // i guess ill have to hardcode all the areas for the allNor list of objects mmmm 
            Debug.Log("ur puncjin");
        }
    }

    public IEnumerator Activate(Normals current)
    {
        yield return new WaitForSeconds(current.startupFrames);
        current.HitBox.SetActive(true);
        yield return new WaitForSeconds(current.hitFrames);
        current.HitBox.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Unit"))
        {
            if (gameObject.GetComponent<Move>().facingRight)
            {
                facing = -1;
            }
            else
            {
                facing = 1;
            }
            col.gameObject.GetComponent<UnitProperties>().GetHit(facing);
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
}
