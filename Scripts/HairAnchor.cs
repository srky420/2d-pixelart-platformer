using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairAnchor : MonoBehaviour
{

    [SerializeField] public Vector2 partoffset = Vector2.zero;
    private float lerpSpeed = 20f;
    private Transform[] hairParts;
    private Transform hairAnchor;

    private void Awake()
    {
        hairAnchor = GetComponent<Transform>();
        hairParts = GetComponentsInChildren<Transform>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Transform pieceToFollow = hairAnchor;
        foreach (var hairPart in hairParts)
        {
            //hair anchor not included
            if(!hairPart.Equals(hairAnchor))
            {   
                Vector2 targetPos = (Vector2)pieceToFollow.position + partoffset;
                Vector2 newPos = Vector2.Lerp(hairPart.position, targetPos, Time.deltaTime * lerpSpeed);

                hairPart.position = newPos;
                pieceToFollow = hairPart;
            }
        }
    }
}
