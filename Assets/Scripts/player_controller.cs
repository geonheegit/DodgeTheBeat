using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_controller : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float speed;

    void Start()
    {
        
    }


    void FixedUpdate()
    {
        rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed * Time.fixedDeltaTime;
    }
}
