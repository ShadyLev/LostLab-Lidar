using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    [Header("Script references")]
    [SerializeField] LIDARScanner scanner;
    [SerializeField] VFXGraphManager graphManager;
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] AIAudioController audioController;
    [SerializeField] GameObject[] enemyModel;

    [Header("Behaviour values.")]
    [Tooltip("Maximum range of finding a point to teleport to.")]
    [SerializeField] float maxTeleportRange;
    [Tooltip("Minumum range of finding a point to teleport to.")]
    [SerializeField] float minTeleportRange;

    [Tooltip("Current phase of the enemy.")]
    [SerializeField] int phase = 1;
    public int Phase { get { return phase; } }

    [Tooltip("Delay before destroying the VFX Points.")]
    [SerializeField] float timeBeforeClearPoint = 1f;
    [Tooltip("Delay between phases.")]
    [SerializeField] float cooldownBetweenPhases = 4f;

    [Header("Scene index to load on death.")]
    [Tooltip("Index of the scene to load when player is killed.")]
    [SerializeField] int endGameSceneIndex;

    bool isBeingScannedByPlayer = false; // Is player scannign the enemy tag
    bool startPhase = false; // Start phase bool

    Vector3 teleportPosition; // Position to teleport to

    // Start is called before the first frame update
    void Start()
    {
        audioController = GetComponent<AIAudioController>();
    }

    // Update is called once per frame
    void Update()
    {
        // If not scanned by player do not progress
        if (!CheckIfBeingScanned())
            return;

        // If a phase is currently running do not progress
        if (startPhase)
            return;

        // Start phase coroutines
        if (phase == 1)
            StartCoroutine(Phase1(timeBeforeClearPoint));
        else if (phase == 2)
            StartCoroutine(Phase2(timeBeforeClearPoint));
    }

    /// <summary>
    /// Checks if an object of tag Enemy is being scanned.
    /// </summary>
    /// <returns>True or false.</returns>
    bool CheckIfBeingScanned()
    {
        PointType type =  scanner.GetPointTypeData("Enemy");

        isBeingScannedByPlayer = type.isBeingScanned;

        return isBeingScannedByPlayer;
    }

    /// <summary>
    /// Coroutine for phase1 of the enemy.
    /// </summary>
    /// <param name="time">Time between scan and teleport.</param>
    /// <returns>Coroutine wait time.</returns>
    IEnumerator Phase1(float time)
    {
        startPhase = true;

        yield return new WaitForSeconds(time);

        TeleportAgentToRandomPositionOnNavMesh(minTeleportRange, maxTeleportRange);

        audioController.PlaySpecificClip(audioController.phase1DiscoveredClip);

        yield return new WaitForSeconds(0.1f);

        graphManager.DestroyVFXList();

        phase++;

        Invoke("ResetPhase", cooldownBetweenPhases);
    }

    /// <summary>
    /// Coroutine for phase2 of the enemy.
    /// </summary>
    /// <param name="time">Time between scan and teleport.</param>
    /// <returns>Coroutine wait time.</returns>
    IEnumerator Phase2(float time)
    {
        startPhase = true;

        yield return new WaitForSeconds(time);

        // teleport player

        ChangeTags("Invisible");

        audioController.PlaySpecificClip(audioController.phase2DiscoveredClip);

        TeleportAgentToRandomPositionOnNavMesh(minTeleportRange, maxTeleportRange);

        yield return new WaitForSeconds(0.1f);

        graphManager.DestroyVFXList();

        phase++;

        Invoke("ResetPhase", cooldownBetweenPhases);
    }

    /// <summary>
    /// Resets the startPhase bool.
    /// </summary>
    void ResetPhase()
    {
        startPhase = false;
    }

    /// <summary>
    /// Finds a random position on the navmesh between min and max range.
    /// </summary>
    /// <param name="minRange">Minimun range from origin.</param>
    /// <param name="maxRange">Maximum range from origin.</param>
    /// <returns>Vector3 position.</returns>
    Vector3 GetRandomPositionOnNavMesh(float minRange, float maxRange)
    {
        float randomRange = Random.Range(minRange, maxRange);

        Vector3 randomDirection = Random.insideUnitSphere * randomRange;

        randomDirection += transform.position;
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(randomDirection, out hit, randomRange, 1);
        Vector3 finalPosition = hit.position;

        teleportPosition = finalPosition;

        return teleportPosition;
    }

    /// <summary>
    /// Teleport the agent to a random position on the nav mesh.
    /// </summary>
    void TeleportAgentToRandomPositionOnNavMesh(float minRange, float maxRange)
    {
        Vector3 teleportPos = GetRandomPositionOnNavMesh(minRange, maxRange);

        transform.position = teleportPos;
    }

    /// <summary>
    /// Kills the player by loading the end game scene.
    /// </summary>
    public IEnumerator KillingAttack()
    {
        if (phase == 3)
        {
            audioController.PlaySpecificClip(audioController.phase3KillClip);

            yield return new WaitForSeconds(audioController.phase3KillClip.length);

            levelLoader.LoadLevel(endGameSceneIndex);
        }
    }

    /// <summary>
    /// Changes the tag of all objects to a new tag.
    /// </summary>
    /// <param name="newTag">New tag name.</param>
    void ChangeTags(string newTag)
    {
        foreach(GameObject part in enemyModel)
        {
            part.tag = newTag;
        }
    }
}
