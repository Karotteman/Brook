using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bateau : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Caddie"))
        {
            switch (collider.gameObject.transform.GetChild(0).gameObject.name)
            {
                case "Bois":
                    break;
                case "Moteur":
                    break;
                case "Volant":
                    break;
                case "Essence":
                    break;
            }
        }
    }
}
