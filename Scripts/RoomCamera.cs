using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    [SerializeField] private Color BGcolor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position, Time.deltaTime * 10f);
            Camera.main.backgroundColor = new Color(BGcolor.r, BGcolor.g, BGcolor.b, BGcolor.a);
        }
    }
}
