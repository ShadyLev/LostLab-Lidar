using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private static DungeonGenerator _instance;
    public static DungeonGenerator Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }


    public class Cell
    {
        public bool visited = false; // Shows if the cell has been visisted by the algorythm
        public bool[] status = new bool[4]; // Shows if the cell is opened in any direction
    }

    [System.Serializable]
    public class Rule
    {
        public GameObject room;
        public Vector2Int minPosition;
        public Vector2Int maxPosition;

        public bool obligatory;

        public int ProbabilityOfSpawning(int x, int y)
        {
            // 0 - cannot spawn
            // 1 - can spawn
            // 2 - has to spawn at the position

            if (x >= minPosition.x && x <= maxPosition.x && y >= minPosition.y && y <= maxPosition.y)
            {
                return obligatory ? 2 : 1;

            }

            return 0;
        }
    }

    // Size of the dungeon
    public Vector2Int size;

    // Starting position
    public int startPos = 0;

    // List of cells in the dungeon
    List<Cell> board;

    // Offset for creating new rooms so they do not overlap
    public Vector2 offset;

    // Room prefabs
    public Rule[] roomPrefabs;

    // Bool to check if finished
    public bool isFinished = false;

    public int mediumDifficultyStart;
    public int hardDifficultyStart;

    public int itiration = 1;

    // Start is called before the first frame update
    void Start()
    {
        MazeGenerator();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Generates the dungeon
    /// </summary>
    void GenerateDungeon()
    {
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Cell currentCell = board[(i + j * size.x)];

                if (currentCell.visited)
                {
                    int randomRoom = -1;

                    List<int> avalibleRooms = new List<int>();

                    // Check if room can or has to be spawned
                    for (int k = 0; k < roomPrefabs.Length; k++)
                    {
                        int p = roomPrefabs[k].ProbabilityOfSpawning(i, j);

                        if(p == 2) // Has to spawn at this position
                        {
                            randomRoom = k;
                            break;
                        }
                        else if(p == 1) // Can be spawned so add to list of avalible rooms
                        {
                            avalibleRooms.Add(k);
                        }
                    }

                    if(randomRoom == -1)
                    {
                        if(avalibleRooms.Count > 0)
                        {
                            randomRoom = avalibleRooms[Random.Range(0, avalibleRooms.Count)];
                        }
                        else
                        {
                            randomRoom = 0;
                        }
                    }

                    var newRoom = Instantiate(roomPrefabs[randomRoom].room, new Vector3(i * offset.x, 0, -j * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
                    newRoom.UpdateRoom(currentCell.status);

                    newRoom.name += " " + i + "-" + j;
                }
            }
        }
        isFinished = true;
    }

    /// <summary>
    /// Generates the maze
    /// </summary>
    void MazeGenerator()
    {
        board = new List<Cell>();

        // Go through the board size
        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                board.Add(new Cell()); // Add a new cell at each location
            }
        }

        // Keeps track of the cell we are at
        int currentCell = startPos;

        // Keeps track of the path we are making
        Stack<int> path = new Stack<int>();

        // Keeps track of which loop we are at ( for the while loop ) to avoid indefinete loopoing
        int k = 0;

        while(k < 1000)
        {
            k++;

            board[currentCell].visited = true;

            if(currentCell == board.Count - 1)
            {
                break;
            }

            // Check the cell's neighbors;
            List<int> neighbors = CheckNeighbors(currentCell);

            if(neighbors.Count == 0)
            {
                if(path.Count == 0)
                {
                    break;
                }
                else
                {
                    currentCell = path.Pop();
                }
            }
            else
            {
                path.Push(currentCell);

                int newCell = neighbors[Random.Range(0, neighbors.Count)];

                if(newCell > currentCell)
                {
                    // Path going down or right
                    if(newCell - 1 == currentCell)
                    {
                        board[currentCell].status[2] = true; // Open the right door
                        currentCell = newCell;
                        board[currentCell].status[3] = true; // Open the left door
                    }
                    else
                    {
                        board[currentCell].status[1] = true; // Open the down door
                        currentCell = newCell;
                        board[currentCell].status[0] = true; // Open the up door
                    }
                }
                else
                {
                    // Path going up or left
                    if (newCell + 1 == currentCell)
                    {
                        board[currentCell].status[3] = true; // Open the left door
                        currentCell = newCell;
                        board[currentCell].status[2] = true; // Open the right door
                    }
                    else
                    {
                        board[currentCell].status[0] = true; // Open the up door
                        currentCell = newCell;
                        board[currentCell].status[1] = true; // Open the down door
                    }
                }
            }
        }
        
        GenerateDungeon();
    }

    List<int> CheckNeighbors(int cell)
    {
        List<int> neighbors = new List<int>();

        // Check up neighbor
        if(cell - size.x >= 0 && !board[Mathf.FloorToInt(cell - size.x)].visited) // Check if the cell exists and has not been visited
        {
            neighbors.Add(Mathf.FloorToInt(cell - size.x));
        }

        // Check down neighbor
        if (cell + size.x < board.Count && !board[Mathf.FloorToInt(cell + size.x)].visited) // Check if the cell exists and has not been visited
        {
            neighbors.Add(Mathf.FloorToInt(cell + size.x));
        }

        // Check right neighbor
        if ((cell+1) % size.x != 0 && !board[Mathf.FloorToInt(cell + 1)].visited) // Check if the cell exists and has not been visited
        {
            neighbors.Add(Mathf.FloorToInt(cell + 1));
        }

        // Check left neighbor
        if (cell % size.x != 0 && !board[Mathf.FloorToInt(cell - 1)].visited) // Check if the cell exists and has not been visited
        {
            neighbors.Add(Mathf.FloorToInt(cell - 1));
        }

        return neighbors;
    }

    public void DestroyCurrectLevel()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void IncreaseLevel()
    {

        DestroyCurrectLevel();

        if(itiration % 2 == 0)
            size = new Vector2Int(size.x + 1, size.y + 1);
        else
            size = new Vector2Int(size.x + 1, size.y);


        // Move the end room to the last square position
        roomPrefabs[1].minPosition = new Vector2Int(size.x - 1, size.y - 1);
        roomPrefabs[1].maxPosition = new Vector2Int(size.x, size.y);

        for (int i = 2; i < roomPrefabs.Length; i++)
        {
            roomPrefabs[i].maxPosition = new Vector2Int(size.x - 1, size.y - 1);
        }

        itiration++;
        MazeGenerator();
    }
}
