using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum Cote { bas, haut, gauche, droit, erreur };
    public class Position
    {
        public Position(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public float x;
        public float y;
        public float z;
    };

    public GameObject[][] matrixTableaux; 
    public GameObject joueur;
    
    void Start()
    {
        Application.targetFrameRate = 60;


        int nbRow = 3;
        int nbCol = 3;
        
        GameObject[] currentTableaux = GameObject.FindGameObjectsWithTag("Tableau");

        Vector2 tableauxSize = new Vector2(nbCol, nbRow);
        matrixTableaux = new GameObject[(int)tableauxSize.x][];

        for (int x = 0; x < tableauxSize.x; x++)
        {
            matrixTableaux[x] = new GameObject[(int)tableauxSize.y];
        }

        
        for (int x = 0, i = 0; x < tableauxSize.x; x++)
        {
            for (int y = 0; y < tableauxSize.y; y++, i++)
            {
                string name = currentTableaux[i].name;
                int reverseId = name[name.Length - 1] - '0';

                int id = (nbRow * nbCol) - reverseId;

                int row = id % nbRow;
                int col = id / nbCol;

                matrixTableaux[col][row] = currentTableaux[i];
            }
        }

        DesactiverTableaux();
        matrixTableaux[2][1].SetActive(true);
    }

    public Vector2 CheckTableauActif()
    {
        for (int i = 0; i < matrixTableaux.Length; i++)
        {
            for (int j = 0; j < matrixTableaux[i].Length; j++)
            {
                if (matrixTableaux[i][j].activeSelf)
                {
                    return new Vector2(i, j);
                }
            }
        }
        return Vector2.zero;
    }

    public GameObject GetTableauActif()
    {
        for (int i = 0; i < matrixTableaux.Length; i++)
        {
            for (int j = 0; j < matrixTableaux[i].Length; j++)
            {
                if (matrixTableaux[i][j].activeSelf)
                {
                    return matrixTableaux[i][j];
                }
            }
        }
        return new GameObject();
    }

    public void DesactiverTableaux()
    {
        for (int i = 0; i < matrixTableaux.Length; i++)
        {
            for (int j = 0; j < matrixTableaux[i].Length; j++)
            {
                matrixTableaux[i][j].SetActive(false);
            }
        }
    }

    public Position ChangementTableau (int tableauPositionX, int tableauPositionY, Cote coteEntre)
    {
        DesactiverTableaux();
        GameObject current = matrixTableaux[tableauPositionX][tableauPositionY];

        current.SetActive(true);
        SpawnPoint spawn = current.GetComponent<SpawnPoint>();

        switch(coteEntre)
        {
            case Cote.bas:
                return new Position(spawn.basX, spawn.basY, spawn.basH);
            case Cote.haut:
                return new Position(spawn.hautX, spawn.hautY, spawn.hautH);
            case Cote.gauche:
                return new Position(spawn.gaucheX, spawn.gaucheY, spawn.gaucheH);
            case Cote.droit:
                return new Position(spawn.droitX, spawn.droitY, spawn.droitH);
            default:
                return new Position(0, 0, 0);
        }
    }
}
