using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitProperties : MonoBehaviour
{
    [SerializeField] float hp;
    [SerializeField] int maxhp;
    [SerializeField] TMP_Text hpBar;
    [SerializeField] Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public IEnumerator Knockback(float side)
    {
        rb.velocity += new Vector2(side, 0);
        yield return new WaitForSeconds(0.2f);
        rb.velocity -= new Vector2(side, 0);
    }
    public void GetHit(float side, float damage)
    {
        hp -= damage;
        StartCoroutine(Knockback(side));  
    }
    
    void Update()
    {
        hpBar.text = hp + "/" + maxhp;
        if (hp <= 0)
        {
            hp = 0; // Theres something like a bug so add coroutine and fix i guess         
            gameObject.SetActive(false);
        }
    }
}
