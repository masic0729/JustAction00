using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GameObject mainCamera;
    float h, v;
    float moveSpeed = 5f;
    float rotateSpeed = 20f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
    }

    void Init()
    {
    }

    void PlayerInput()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 moveVector = new Vector3(h, 0, v);

        if(moveVector.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveVector);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

            // 이동 (앞 방향으로 전진)
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
            transform.Translate(moveVector * moveSpeed * Time.deltaTime);
        }

    }

}
