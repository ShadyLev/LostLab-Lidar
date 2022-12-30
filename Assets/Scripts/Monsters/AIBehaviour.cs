using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    [Header("Script references")]
    [SerializeField] LIDARScanner scanner;
    [SerializeField] VFXGraphManager graphManager;
    [SerializeField] LevelLoader levelLoader;

    [SerializeField] int endGameSceneIndex;
    [SerializeField] int phase = 0;
    [SerializeField] float timeBeforeClearPoint = 1f;

    [SerializeField] bool isBeingScannedByPlayer = false;
    [SerializeField] bool startPhase = false;

    [SerializeField] float cooldownBetweenPhases = 4f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!CheckIfBeingScanned())
            return;

        if (startPhase)
            return;

        if (phase == 0)
            StartCoroutine(Phase1(timeBeforeClearPoint));
        else if (phase == 1)
            StartCoroutine(Phase2(timeBeforeClearPoint));
    }

    bool CheckIfBeingScanned()
    {
        PointType type =  scanner.GetPointTypeData("Enemy");

        isBeingScannedByPlayer = type.isBeingScanned;

        return isBeingScannedByPlayer;
    }

    IEnumerator Phase1(float time)
    {
        startPhase = true;

        yield return new WaitForSeconds(time);

        graphManager.DestroyVFXList();
        phase++;

        Invoke("ResetPhase", cooldownBetweenPhases);
    }

    IEnumerator Phase2(float time)
    {
        startPhase = true;

        yield return new WaitForSeconds(time);

        graphManager.DestroyVFXList();
        // teleport player
        this.gameObject.tag = "Invisible";
        phase++;

        Invoke("ResetPhase", cooldownBetweenPhases);
    }

    void ResetPhase()
    {
        startPhase = false;
    }

    public void KillingAttack()
    {
        if(phase == 2)
            levelLoader.LoadLevel(endGameSceneIndex);
    }
}
