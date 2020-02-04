using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouvement : MonoBehaviour
{


    [Header("Player Motor")]
    //[Range(1f, 15f)]
    public float walkSpeed;
    public float spawnPosition = 4.4f;

    CharacterController characterController;
    Animator anim;

    // Use this for initialization
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        anim = transform.GetChild(1).gameObject.GetComponent<Animator>();
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
        characterController.SimpleMove(direction * walkSpeed * Time.deltaTime);


        if (direction != new Vector3(0,0,0) && !anim.GetBool("isWalking"))
        {
            print("OUI : " + direction);
            anim.SetBool("isWalking", true);
        }
        else
        {
            print("NON : " + direction);
            anim.SetBool("isWalking", false);
        }

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        transform.GetChild(4).transform.rotation = Quaternion.identity;
    }

    public void TeleportationNouveauTableau(float positionX, float positionY, float positionZ)
    {
        enabled = false;
        transform.position = new Vector3(positionX, positionZ, positionY);
    }
}
