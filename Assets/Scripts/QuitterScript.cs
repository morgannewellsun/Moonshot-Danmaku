using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitterScript : MonoBehaviour
{
    void Update()
    {
        if (Input.GetButtonDown("Menu")) {
            Application.Quit();
        }
    }
}
