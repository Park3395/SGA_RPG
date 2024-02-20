using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    CharacterController cc;
    float speed = 10f;
    private void Awake()
    {
        cc = this.GetComponent<CharacterController>();
    }

    private void Update()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Vector3 dir = new Vector3(h,0,v);
        dir = dir.normalized;
        dir = Camera.main.transform.TransformDirection(dir);
        dir *= speed * Time.deltaTime;
        dir.y = 0;
        cc.Move(dir);
    }
}
