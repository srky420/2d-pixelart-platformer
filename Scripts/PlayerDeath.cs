using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Behaviour[] componentsToDisable;
    [SerializeField] private Transform respawnPoint;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Trap"))
        {
            foreach (var component in componentsToDisable)
            {
                component.enabled = false;
            }
            anim.SetBool("isDead", true);
            rb.bodyType = RigidbodyType2D.Static;
            StartCoroutine(Respawn(2f));
        }
    }

    private IEnumerator Respawn(float t)
    {
        yield return new WaitForSeconds(t);
        transform.position = respawnPoint.position;
        anim.SetBool("isDead", false);
        foreach (var component in componentsToDisable)
        {
            component.enabled = true;
        }
        rb.bodyType = RigidbodyType2D.Dynamic;
        sr.enabled = true;
    }
}


