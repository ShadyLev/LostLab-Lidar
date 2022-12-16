using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSequence : MonoBehaviour
{
    [SerializeField] GameObject[] slides;

    private int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void NextSlide()
    {
        if(counter + 1 > slides.Length)
        {
            Debug.Log("No more slides");
            return;
        }

        counter++;

        foreach(GameObject slide in slides)
        {
            slide.SetActive(false);
        }

        slides[counter].SetActive(true);
    }
}
