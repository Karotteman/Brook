using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouvement : MonoBehaviour
{

    [Header("Player Motor")]
    //[Range(1f, 15f)]
    public float walkSpeed;

    CharacterController characterController;

    // Use this for initialization
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerControls();
    }

    void HandlePlayerControls()
    {
        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(hInput, 0, vInput);

        if(direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        characterController.SimpleMove(direction * walkSpeed * Time.deltaTime);

    }
}
