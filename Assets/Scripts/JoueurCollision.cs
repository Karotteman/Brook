using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoueurCollision : MonoBehaviour
{
    PlayerMouvement mouvementJoueur;
    public GameManager manager;

    private enum Contenu { bois, essence, moteur, vide }
    private enum Bras { pelle, piedBiche, hache, bras, aucun }

    private Contenu contenuCaddie = Contenu.vide;
    private Bras deuxiemeBras = Bras.aucun;

    private bool asCaddie = false;
    private bool moteurSortie = false;
    private bool volantSortie = false;

    private bool boisFait = false;
    private bool essenceFait = false;
    private bool moteurFait = false;
    private bool volantFait = false;

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
                Invoke("TrueEnabledMouvementJoueur", 1);
            }
        }
        else if (asCaddie)
        {
            if (collider.gameObject.CompareTag("Pickup") && contenuCaddie == Contenu.vide)
            {
                GameObject item = collider.transform.GetChild(0).gameObject;
                if (item.CompareTag("Bois"))
                {
                    if (!boisFait)
                    {
                        contenuCaddie = Contenu.bois;
                        boisFait = true;
                        item.SetActive(false);
                    }
                    else
                    {
                        return;
                    }
                }
                else if (item.CompareTag("Essence"))
                {
                    contenuCaddie = Contenu.essence;
                    essenceFait = true;
                    item.SetActive(false);
                }
                else if (item.CompareTag("Moteur") && moteurSortie)
                {
                    contenuCaddie = Contenu.moteur;
                    moteurFait = true;
                    item.SetActive(false);
                }
            }
            else if (collider.gameObject.CompareTag("DropZone") && contenuCaddie != Contenu.vide)
            {
                Transform bateau = collider.gameObject.transform.GetChild(0);
                string tag = "";
                if (contenuCaddie == Contenu.bois)
                {
                    tag = "Bois";
                }
                else if (contenuCaddie == Contenu.moteur)
                {
                    tag = "Moteur";
                }

                if (!System.String.IsNullOrEmpty(tag))

                {
                    foreach (Transform group in bateau)
                    {
                        if (group.CompareTag(tag))
                        {
                            group.gameObject.SetActive(true);
                            break;
                        }
                    }
                }

                contenuCaddie = Contenu.vide;
            }
        }

        if (collider.gameObject.CompareTag("DropZone") && boisFait && essenceFait && moteurFait && volantFait)
        {
            FinDuJeu();
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

            transform.GetChild(0).gameObject.transform.GetChild((int)deuxiemeBras).gameObject.SetActive(false);

            if (newItem.CompareTag("Bras"))
            {
                deuxiemeBras = Bras.bras;

            }
            else if (newItem.CompareTag("Pelle"))
            {
                deuxiemeBras = Bras.pelle;
            }
            else if (newItem.CompareTag("Hache"))
            {
                deuxiemeBras = Bras.hache;
            }
            else if (newItem.CompareTag("PiedBiche"))
            {
                deuxiemeBras = Bras.piedBiche;
            }
            else
            {
                deuxiemeBras = Bras.aucun;
            }
            transform.GetChild(0).gameObject.transform.GetChild((int)deuxiemeBras).gameObject.SetActive(true);
            //GameObject bras = transform.GetChild(0).gameObject;
            //GameObject oldItem = bras.transform.GetChild(0).gameObject;
            //GameObject oldItem = bras.transform.GetChild(bras.transform.childCount-1).gameObject;

            GameObject espaceBras = transform.GetChild(2).gameObject;
            GameObject oldItem = espaceBras.transform.GetChild(0).gameObject;
            
            oldItem.transform.parent = swapZone;
            oldItem.transform.position = swapZone.transform.position;
            oldItem.SetActive(true);
            newItem.transform.parent = espaceBras.transform;
            newItem.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (collider.gameObject.CompareTag("Caddie") && deuxiemeBras == Bras.bras)
            {
                if (asCaddie)
                {
                    Transform espaceCaddie = transform.GetChild(3).transform;
                    Transform caddie = espaceCaddie.GetChild(0).transform;

                    GameObject tableau = manager.GetTableauActif();

                    caddie.parent = tableau.transform;

                    asCaddie = false;
                }
                else
                {
                    Transform espaceCaddie = transform.GetChild(3).transform;
                    collider.transform.parent = espaceCaddie;
                    asCaddie = true;
                }
            }

            if (collider.gameObject.CompareTag("Pickup"))
            {
                GameObject item = collider.transform.GetChild(0).gameObject;
                if (item.CompareTag("Bois") && deuxiemeBras == Bras.hache)
                {
                    item.SetActive(true);
                }
                
                if (item.CompareTag("Volant"))
                {
                    if (deuxiemeBras == Bras.pelle && !volantSortie)
                    {
                        collider.gameObject.transform.position += new Vector3(0, 0.5f, 0);
                        volantSortie = true;
                    }
                    else if (deuxiemeBras == Bras.bras && volantSortie)
                    {
                        // TODO: revoir pick up
                        volantFait = true;
                        item.SetActive(false);
                    }
                }

                if (item.CompareTag("Moteur") && deuxiemeBras == Bras.piedBiche)
                {
                    if (!moteurSortie)
                    {
                        collider.gameObject.transform.position += new Vector3(-1f, 0, -1f); // TODO: voir distance
                        moteurSortie = true;
                    }
                }
            }
        }
    }

    public void FinDuJeu()
    {
        transform.position += new Vector3(0,-5,0);
        Reinitialiser();
    }

    public void Reinitialiser()
    {
    }
}
