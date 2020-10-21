using UnityEngine;
using System.Collections;

//<summary>
//Game object, that creates maze and instantiates it in scene
//</summary>
public class MazeSpawner : MonoBehaviour
{
    public enum MazeGenerationAlgorithm
    {
        PureRecursive,
        RecursiveTree,
        RandomTree,
        OldestTree,
        RecursiveDivision,
    }

    public MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
    public bool FullRandom = false;
    public int RandomSeed = 12345;
    public GameObject Floor = null;
    public GameObject Ceiling = null;
    public GameObject Wall = null;
    public GameObject Pillar = null;
    public int Rows = 5;
    public int Columns = 5;
    public float CellWidth = 5;
    public float CellHeight = 5;
    public bool AddGaps = true;
    public GameObject GoalPrefab = null;
    public GameObject ZombiePrefab = null;
    public GameObject SpiderPrefab = null;
    public static Vector3 GoalPosition;

    private BasicMazeGenerator mMazeGenerator = null;

    void Start()
    {
        if (!FullRandom)
        {
            Random.seed = RandomSeed;
        }
        switch (Algorithm)
        {
            case MazeGenerationAlgorithm.PureRecursive:
                mMazeGenerator = new RecursiveMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveTree:
                mMazeGenerator = new RecursiveTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RandomTree:
                mMazeGenerator = new RandomTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.OldestTree:
                mMazeGenerator = new OldestTreeMazeGenerator(Rows, Columns);
                break;
            case MazeGenerationAlgorithm.RecursiveDivision:
                mMazeGenerator = new DivisionMazeGenerator(Rows, Columns);
                break;
        }
        mMazeGenerator.GenerateMaze();

        int typeOfBot = 0;


        for (int row = 0; row < Rows; row++)
        {
            int randomColumn = Random.Range(3, 5);
            int randomColumn1 = Random.Range(8, 10);

            for (int column = 0; column < Columns; column++)
            {
                float x = column * (CellWidth + (AddGaps ? .2f : 0));
                float z = row * (CellHeight + (AddGaps ? .2f : 0));
                MazeCell cell = mMazeGenerator.GetMazeCell(row, column);
                GameObject tmp;
                tmp = Instantiate(Floor, new Vector3(x, 0, z - 0.9f), Quaternion.Euler(-90, 0, 0)) as GameObject;
                tmp.transform.parent = transform;

                tmp = Instantiate(Ceiling, new Vector3(x, 3.01f, z - 1), Quaternion.Euler(-90, 0, 0)) as GameObject;
                tmp.transform.parent = transform;
                tmp = Instantiate(Ceiling, new Vector3(x, 3.01f, z + 1), Quaternion.Euler(-90, 0, 0)) as GameObject;
                tmp.transform.parent = transform;

                if (cell.WallRight)
                {
                    tmp = Instantiate(Wall, new Vector3(x + CellWidth / 2, 1.5f, z) + Wall.transform.position, Quaternion.Euler(-90, 270, 0)) as GameObject;// right
                    tmp.transform.parent = transform;
                }
                if (cell.WallFront)
                {
                    tmp = Instantiate(Wall, new Vector3(x, 1.5f, z + CellHeight / 2) + Wall.transform.position, Quaternion.Euler(-90, 180, 0)) as GameObject;// front
                    tmp.transform.parent = transform;
                }
                if (cell.WallLeft)
                {
                    tmp = Instantiate(Wall, new Vector3(x - CellWidth / 2, 1.5f, z) + Wall.transform.position, Quaternion.Euler(-90, 90, 0)) as GameObject;// left
                    tmp.transform.parent = transform;
                }
                if (cell.WallBack)
                {
                    tmp = Instantiate(Wall, new Vector3(x, 1.5f, z - CellHeight / 2) + Wall.transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;// back
                    tmp.transform.parent = transform;
                }
                if (row == Rows - 1 && column == Columns - 1)
                {
                    tmp = Instantiate(GoalPrefab, new Vector3(x, 1, z), Quaternion.Euler(90, 0, 0)) as GameObject;
                    tmp.transform.parent = transform;
                    GoalPosition = tmp.transform.position;
                }
                if (typeOfBot % 3 == 0)
                {
                    if (randomColumn == column)
                    {
                        tmp = Instantiate(ZombiePrefab, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0)) as GameObject;
                        tmp.transform.parent = transform;
                    }
                    if (randomColumn1 == column)
                    {
                        tmp = Instantiate(SpiderPrefab, new Vector3(x, 0, z), Quaternion.Euler(0, 0, 0)) as GameObject;
                        tmp.transform.parent = transform;
                    }

                }
            }
            typeOfBot++;
        }

        if (Pillar != null)
        {
            for (int row = 0; row < Rows + 1; row++)
            {
                for (int column = 0; column < Columns + 1; column++)
                {
                    float x = column * (CellWidth + (AddGaps ? .2f : 0));
                    float z = row * (CellHeight + (AddGaps ? .2f : 0));
                    GameObject tmp = Instantiate(Pillar, new Vector3(x - CellWidth / 2, 1.5f, z - CellHeight / 2), Quaternion.Euler(-90, 0, 0)) as GameObject;
                    tmp.transform.parent = transform;
                }
            }
        }
    }
}
