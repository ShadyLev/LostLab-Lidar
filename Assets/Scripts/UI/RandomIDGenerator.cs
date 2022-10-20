using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RandomIDGenerator : MonoBehaviour
{
    TextMeshProUGUI text;

    string st = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        string generatedID = "D " + Random.Range(0, 9).ToString() + st[Random.Range(0, st.Length)] + st[Random.Range(0, st.Length)] + Random.Range(0, 9).ToString() + Random.Range(0, 9).ToString() + Random.Range(0, 9).ToString();
        text.SetText(generatedID);
    }
}
