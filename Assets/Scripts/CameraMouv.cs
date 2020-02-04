using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouv : MonoBehaviour
{
    public Transform objetASuivre;
    Vector3 distance;

    // Start is called before the first frame update
    void Start()
    {
        distance = transform.position - objetASuivre.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = objetASuivre.position + distance;
    }
}
