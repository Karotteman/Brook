using System;
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
    private AudioSource audio;

    private bool asCaddie = false;
    private bool asVolant = false;

    private bool moteurSortie = false;
    private bool volantSortie = false;

    private bool boisFait = false;
    private bool essenceFait = false;
    private bool moteurFait = false;
    private bool volantFait = false;

    private int hintIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        mouvementJoueur = GetComponentInParent<PlayerMouvement>();
        audio = GetComponentInParent<AudioSource>();
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

            if (tableauPositionX != (int)prochainTableau.x || tableauPositionY != (int)prochainTableau.y)
            {
                //mouvementJoueur.TeleportationNouveauTableau();

                GameManager.Position spawnPoint = manager.ChangementTableau(tableauPositionX, tableauPositionY, coteEntre);

                mouvementJoueur.TeleportationNouveauTableau(spawnPoint.x, spawnPoint.y, spawnPoint.z);
                Invoke("TrueEnabledMouvementJoueur", 0.5f);
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
                        SelectHint(collider);
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
                transform.GetChild(3).transform.gameObject.GetComponent<AudioSource>().Play();
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

                asVolant = false;
                contenuCaddie = Contenu.vide;
                collider.gameObject.GetComponent<AudioSource>().Play();
            }
        }

        if (collider.gameObject.CompareTag("DropZone") && boisFait && essenceFait && moteurFait && volantFait)
        {
            FinDuJeu();
        }

        SelectHint(collider);
    }

    private void SelectHint(Collider collider)
    {
        if (collider.gameObject.CompareTag("SwapZone") && !asCaddie && !asVolant)
        {
            AfficherIndice(1); // E
        }

        else if (collider.gameObject.CompareTag("Caddie"))
        {
            if (asVolant)
            {
                AfficherIndice(0); // Bateau
            }
            if(deuxiemeBras == Bras.bras)
            {
                AfficherIndice(2); // F
            }
            else
            {
                AfficherIndice(9); // Bras
            }
        }

        else if (collider.gameObject.CompareTag("Pickup"))
        {
            if(contenuCaddie != Contenu.vide || asVolant)
            {
                AfficherIndice(0); // Bateau
                return;
            }

            GameObject item = collider.transform.GetChild(0).gameObject;
            if (item.CompareTag("Bois"))
            {
                if (!boisFait)
                {
                    if(item.activeSelf)
                    {
                        if (asCaddie)
                        {
                            AfficherIndice(2); // F
                        }
                        else
                        {
                            AfficherIndice(8); // Caddie
                        }
                    }
                    else
                    {
                        if (deuxiemeBras == Bras.hache)
                        {
                            AfficherIndice(2); // F
                        }
                        else
                        {
                            AfficherIndice(11); // Hache
                        }
                    }
                }
            }
            else if (item.CompareTag("Volant"))
            {
                if (!volantFait)
                {
                    if (volantSortie)
                    {
                        if (deuxiemeBras == Bras.bras)
                        {
                            AfficherIndice(2); // F
                        }
                        else
                        {
                            AfficherIndice(9); // Bras
                        }
                    }
                    else
                    {
                        if (deuxiemeBras == Bras.pelle)
                        {
                            AfficherIndice(2); // F
                        }
                        else
                        {
                            AfficherIndice(10); // Pelle
                        }
                    }
                }
            }
            else if (item.CompareTag("Moteur"))
            {
                if (!moteurFait)
                {
                    if (moteurSortie)
                    {
                        if (asCaddie)
                        {
                            AfficherIndice(2); // F
                        }
                        else
                        {
                            AfficherIndice(8); // Caddie
                        }
                    }
                    else
                    {
                        if (deuxiemeBras == Bras.piedBiche)
                        {
                            AfficherIndice(2); // F
                        }
                        else
                        {
                            AfficherIndice(12); // PiedBiche
                        }
                    }
                }
            }
            else if (item.CompareTag("Essence") && !asCaddie && !essenceFait)
            {
                if (!asCaddie)
                {
                    AfficherIndice(8); // Caddie
                }
            }
        }

        else if (collider.gameObject.CompareTag("DropZone"))
        {
            if(boisFait == false)
            {
                AfficherIndice(4); // Bois
            }
            else if (moteurFait == false)
            {
                AfficherIndice(5); // Moteur
            }
            else if (essenceFait == false)
            {
                AfficherIndice(6); // Essence
            }
            else if (volantFait == false)
            {
                AfficherIndice(7); // Volant
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

            audio.Play();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!asVolant)
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
                        CacherIndice();
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
                            asVolant = true;
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
    }

    private void OnTriggerExit(Collider other)
    {
        CacherIndice();
    }

    void AfficherIndice(int index)
    {
        if(index == 2 && asCaddie)
        {
            return;
        }

        CacherIndice();

        Transform espaceIndice = transform.GetChild(4).gameObject.transform;
        GameObject indice = espaceIndice.GetChild(index).gameObject;
        indice.SetActive(true);

        hintIndex = index;
    }

    void CacherIndice()
    {
        if (!(hintIndex < 0))
        {
            transform.GetChild(4).gameObject.transform.GetChild(hintIndex).gameObject.SetActive(false);
            hintIndex = -1;
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
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
