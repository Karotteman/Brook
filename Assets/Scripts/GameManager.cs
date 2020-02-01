using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[][] matrixTableaux; 
    public GameObject joueur;

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        GameObject[] currentTableaux = GameObject.FindGameObjectsWithTag("Tableau");

        Vector2 tableauxSize = new Vector2(3, 3);
        matrixTableaux = new GameObject[(int)tableauxSize.x][];

        for (int x = 0; x < tableauxSize.x; x++)
        {
            matrixTableaux[x] = new GameObject[(int)tableauxSize.y];
            for (int y = 0; y < tableauxSize.y; y++)
            {
                // manipulate gameobject here
                matrixTableaux[x][y] = currentTableaux[i];
                i++;
            }
        }
        DesactiverTableaux();
        matrixTableaux[2][2].SetActive(true);
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

    public void ChangementTableau (int tableauPositionX, int tableauPositionY)
    {
        DesactiverTableaux();
        matrixTableaux[tableauPositionX][tableauPositionY].SetActive(true);
        print(matrixTableaux[tableauPositionX][tableauPositionY].name);
    }
}
