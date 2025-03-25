using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoBehaviour
{


    FadeinOut fade;

    // Start is called before the first frame update
    void Start()
    {
        fade = FindAnyObjectByType<FadeinOut>();

        fade.FadeOut();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
