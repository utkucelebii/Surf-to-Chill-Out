using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    private CurvedWorld curvedWorld;

    // Gameplay
    private float chunkSpawnZ;
    private Queue<Chunk> activeChunks = new Queue<Chunk>();
    private List<Chunk> chunkPool = new List<Chunk>();

    //Configuration fields
    [SerializeField] private int firstChunkSpawnPosition = -10;
    [SerializeField] private int chunkOnScreen = 5;
    [SerializeField] private float despawnDistance = 5.0f;

    [SerializeField] private List<GameObject> chunkPrefab;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private List<Material> materials = new List<Material>();
    private int materialChangeCounter;
    private int randomMaterialIndex = 0;
    private int totalChunk;
    private bool curveChangeble;
    private float targetCurve;

    [SerializeField] private List<GameObject> coinMineChunks;

    private void Awake()
    {
        curvedWorld = GetComponent<CurvedWorld>();
        chunkPrefab = Resources.LoadAll<GameObject>("Levels").Where(i => i.name != "CoinMineChunk").ToList();
        ResetTheGame();
    }

    private void Start()
    {
        if (!cameraTransform)
            cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        ScanPosition();
    }

    private void FixedUpdate()
    {
        if (totalChunk % 20 == 0)
        {
            changeCurveDirectionX();
        }
        if (curveChangeble)
        {
            curvedWorld.Curvature = Vector3.Lerp(curvedWorld.Curvature, new Vector3(0, curvedWorld.Curvature.y, targetCurve), Time.deltaTime * 10);
        }
    }
    IEnumerator test(float target)
    {
        targetCurve = target;
        totalChunk = 1;
        curveChangeble = true;
        yield return new WaitUntil(() => curvedWorld.Curvature.z == targetCurve);
        curveChangeble = false;
    }
    private void changeCurveDirectionX()
    {
        int i = Random.Range(0, 5);
        if (i == 0)
        {
            StartCoroutine(test(1.75f));
        }
        else if (i == 1)
        {
            StartCoroutine(test(-1.75f));
        }
        else
        {
            StartCoroutine(test(0));
        }
    }

    private void ScanPosition()
    {
        float cameraZ = cameraTransform.position.z;
        Chunk lastChunk = activeChunks.Peek();

        if(cameraZ > lastChunk.transform.position.z +lastChunk.chunkLength + despawnDistance)
        {
            SpawnNewChunk();
            DeleteLastChunk();
        }
    }

    private void SpawnNewChunk()
    {
        totalChunk++;

        if(totalChunk % 15 == 0)
        {
            int randomCoinChunk = Random.Range(0, coinMineChunks.Count);
            Chunk chunky = chunkPool.Find(x => !x.gameObject.activeSelf && x.name == (coinMineChunks[randomCoinChunk].name + "(Clone)"));

            if (!chunky)
            {
                GameObject go = null;
                go = Instantiate(coinMineChunks[randomCoinChunk], transform);

                chunky = go.GetComponent<Chunk>();
            }
            chunky.transform.position = new Vector3(0, 0, chunkSpawnZ);
            chunkSpawnZ += chunky.chunkLength;

            activeChunks.Enqueue(chunky);
            chunky.ShowChunk();

            return;
        }

        int randomIndex = Random.Range(0, chunkPrefab.Count);

        Chunk chunk = chunkPool.Find(x => !x.gameObject.activeSelf && x.name == (chunkPrefab[randomIndex].name + "(Clone)"));

        if (!chunk)
        {
            GameObject go = null;
            if(transform.childCount < 2)
            {
                go = Instantiate(chunkPrefab[0], transform);
            }
            else
            {
                go = Instantiate(chunkPrefab[randomIndex], transform);
            }
            
            chunk = go.GetComponent<Chunk>();
        }

        if (materialChangeCounter > 5)
        {
            materialChangeCounter = 0;
            randomMaterialIndex = Random.Range(0, materials.Count);
        }
        chunk.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = materials[randomMaterialIndex];
        chunk.transform.GetChild(1).GetComponent<MeshRenderer>().material = materials[randomMaterialIndex];

        chunk.transform.position = new Vector3(0, 0, chunkSpawnZ);
        chunkSpawnZ += chunk.chunkLength;

        activeChunks.Enqueue(chunk);
        chunk.ShowChunk();
        materialChangeCounter++;
    }

    private void DeleteLastChunk()
    {
        Chunk chunk = activeChunks.Dequeue();
        chunk.HideChunk();
        chunkPool.Add(chunk);
    }

    public void ResetTheGame()
    {
        chunkSpawnZ = firstChunkSpawnPosition;

        for (int i = activeChunks.Count; i != 0 ; i--)
        {
            DeleteLastChunk();
        }

        for (int i = 0; i < chunkOnScreen; i++)
        {
            SpawnNewChunk();
        }
    }
}
