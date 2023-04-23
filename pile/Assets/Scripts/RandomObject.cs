using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObject : MonoBehaviour
{
    [SerializeField] GameObject[] objects;
    
    private void OnEnable()
    {
        int chosenObject = Random.Range(0, objects.Length);
        for (int i = 0; i < objects.Length; i++)
        {
            if (i != chosenObject)
                objects[i].SetActive(false);
            else
                objects[i].SetActive(true);
        }
    }
}
