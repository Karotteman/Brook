using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoueurCollision : MonoBehaviour
{
    PlayerMouvement mouvementJoueur ;
    public GameManager manager;

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

            switch (collider.gameObject.name)
            {
                case "TriggerHaut":
                    if((int)prochainTableau.x - 1 >= 0)
                    {
                        tableauPositionX -= 1;
                    }
                    break;
                case "TriggerBas":
                    if ((int)prochainTableau.x + 1 <= 2)
                    {
                        tableauPositionX += 1;
                    }
                    break;
                case "TriggerDroite":
                    if ((int)prochainTableau.y + 1 <= 2)
                    {
                        tableauPositionY += 1;
                    }
                    break;
                case "TriggerGauche":
                    if ((int)prochainTableau.y - 1 >= 0)
                    {
                        tableauPositionY -= 1;
                    }
                    break;
                default:
                    break;
            }
            print(tableauPositionX);
            print(tableauPositionY);

            if (tableauPositionX != (int)prochainTableau.x || tableauPositionY != (int)prochainTableau.y)
            {
                //mouvementJoueur.TeleportationNouveauTableau();

                mouvementJoueur.TeleportationNouveauTableau();
                Invoke("TrueEnabledMouvementJoueur", 2);

                manager.ChangementTableau(tableauPositionX, tableauPositionY);
            }
        }
    }
}
