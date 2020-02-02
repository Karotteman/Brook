using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoueurCollision : MonoBehaviour
{
    PlayerMouvement mouvementJoueur;
    public GameManager manager;

    private bool asCaddie = false;
    private bool caddieFull = false;

    // Start is called before the first frame update
    void Start()
    {
        mouvementJoueur = GetComponentInParent<PlayerMouvement>();
    }

    public void TrueEnabledMouvementJoueur()
    {
        mouvementJoueur.enabled = true;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Trigger"))
        {
            //// Activate traps animations
            //Animator anim = collider.GetComponentInParent<Animator>();
            //anim.SetTrigger("Trigger");

            //mouvementJoueur.enabled = false;
            Vector2 prochainTableau = manager.CheckTableauActif();
            int tableauPositionX = (int)prochainTableau.x;
            int tableauPositionY = (int)prochainTableau.y;
            GameManager.Cote coteEntre;

            switch (collider.gameObject.name)
            {
                case "TriggerHaut":
                    if ((int)prochainTableau.x - 1 >= 0)
                    {
                        tableauPositionX -= 1;
                    }
                    coteEntre = GameManager.Cote.bas;
                    break;
                case "TriggerBas":
                    if ((int)prochainTableau.x + 1 <= 2)
                    {
                        tableauPositionX += 1;
                    }
                    coteEntre = GameManager.Cote.haut;
                    break;
                case "TriggerDroite":
                    if ((int)prochainTableau.y + 1 <= 2)
                    {
                        tableauPositionY += 1;
                    }
                    coteEntre = GameManager.Cote.gauche;
                    break;
                case "TriggerGauche":
                    if ((int)prochainTableau.y - 1 >= 0)
                    {
                        tableauPositionY -= 1;
                    }
                    coteEntre = GameManager.Cote.droit;
                    break;
                default:
                    coteEntre = GameManager.Cote.erreur;
                    break;
            }
            print(tableauPositionX);
            print(tableauPositionY);

            if (tableauPositionX != (int)prochainTableau.x || tableauPositionY != (int)prochainTableau.y)
            {
                //mouvementJoueur.TeleportationNouveauTableau();

                GameManager.Position spawnPoint = manager.ChangementTableau(tableauPositionX, tableauPositionY, coteEntre);

                mouvementJoueur.TeleportationNouveauTableau(spawnPoint.x, spawnPoint.y);
                Invoke("TrueEnabledMouvementJoueur", 2);
            }
        }

        else if (collider.gameObject.CompareTag("Pickup") && asCaddie)
        {
            GameObject item = collider.transform.GetChild(0).gameObject;
            if (item.CompareTag(""))
            {
            }
            else if(item.CompareTag(""))
            {
            }
            else if (item.CompareTag(""))
            {
            }
        }
    }

    public void OnTriggerStay(Collider collider)
    {
        if (Input.GetKeyDown(KeyCode.E) && collider.gameObject.CompareTag("SwapZone"))
        {
            if (asCaddie)
            {
                return;
            }

            Transform swapZone = collider.gameObject.transform;
            GameObject newItem = swapZone.GetChild(0).gameObject;

            GameObject bras = transform.GetChild(0).gameObject;
            GameObject oldItem = bras.transform.GetChild(0).gameObject;

            oldItem.transform.parent = swapZone;
            newItem.transform.parent = bras.transform;
        }

        if (Input.GetKeyDown(KeyCode.Q) && collider.gameObject.CompareTag("Caddie"))
        {
            if (asCaddie)
            {
                Transform espaceCaddie = transform.GetChild(1).transform;
                Transform caddie = espaceCaddie.GetChild(0).transform;

                GameObject tableau = manager.GetTableauActif();

                caddie.parent = tableau.transform;
            }
            else
            {
                if(true) // (robot have 2 arms)
                {
                    Transform espaceCaddie = transform.GetChild(1).transform;
                    collider.transform.parent = espaceCaddie;
                }
            }
            asCaddie = !asCaddie;
        }
    }
}
