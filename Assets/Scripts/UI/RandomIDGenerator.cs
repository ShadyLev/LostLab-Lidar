using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

enum Mode
{
    RandomNumberInRange,
    RandomText,
    RandomCrime,
    RandomDate,
}

public class RandomIDGenerator : MonoBehaviour
{
    [SerializeField] Mode mode;

    [SerializeField] TextMeshProUGUI text;

    [Header("Random Number In Range")]
    [SerializeField] int m_randomNumberMin;
    [SerializeField] int m_randomNumberMax;

    string st = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    string[] crimes = new string[]
    {
        "Murder",
        "Manslaughter",
        "Child destruction",
        "Infanticide",
        "Soliciting to murder",
        "Kidnapping",
        "Arson",
        "Aggravated arson",
        "Endangering an aircraft",
        "Drug trafficking",
        "Riot",
        "Violent disorder",
        "Fund raising for terrorism",
        "Terrorism",
        "Armed robbery",
        "Blackmail",
        "Child abduction"
    };

    // Start is called before the first frame update
    void Start()
    {
        switch (mode)
        {
            default:
                break;
            case Mode.RandomText:
                RandomText();
                break;
            case Mode.RandomNumberInRange:
                RandomNumberText();
                break;
            case Mode.RandomCrime:
                RandomCrime();
                break;
            case Mode.RandomDate:
                RandomDate();
                break;
        }

    }

    void RandomNumberText()
    {
        int rnd = UnityEngine.Random.Range(m_randomNumberMin, m_randomNumberMax);

        text.SetText(rnd.ToString());
    }

    void RandomText()
    {
        string generatedID = st[UnityEngine.Random.Range(0, st.Length)].ToString() + st[UnityEngine.Random.Range(0, st.Length)].ToString() + " - " + UnityEngine.Random.Range(0, 9).ToString() + UnityEngine.Random.Range(0, 9).ToString() + UnityEngine.Random.Range(0, 9).ToString();
        text.SetText(generatedID);
    }

    void RandomCrime()
    {
        string tmp = crimes[UnityEngine.Random.Range(0, crimes.Length)].ToString();

        text.SetText(tmp);
    }

    void RandomDate()
    {
        DateTime end = new DateTime(2025, 1, 1);
        DateTime today = DateTime.Today;

        int range = (end - today).Days;

        DateTime randomDate = DateTime.Today.AddDays(UnityEngine.Random.Range(0, range))
            .AddHours(UnityEngine.Random.Range(0, 24))
            .AddMinutes(UnityEngine.Random.Range(0, 24))
            .AddSeconds(UnityEngine.Random.Range(0, 24));

        text.SetText(randomDate.ToString());
    }
}
