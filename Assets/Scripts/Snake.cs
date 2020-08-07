using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Snake : MonoBehaviour
{
    enum Direction {
        Up,
        Down,
        Left,
        Right,
        None
    };
    public enum Algorithm {
        None,
        AStar,
        BFS,
        DFS,
        Dijkstras
    }

    AudioSource sound;

    Direction direction = Direction.None;
    float speed = 0.5f;
    GameObject lastTail;
    bool movable = true;

    public GameObject Tail;
    public GameObject Food;
    public GameObject Tile;
    public Text ScoreText;
    public Algorithm algorithm;

    static public int score = 0;
    static public bool[,] occupied = new bool[17,35];
    static public bool move = true;
    List<GameObject> tiles = new List<GameObject>();
    static int maxX = 34, maxY = 16;
    Direction[] directions;
    int dirInd = 0;
    


    void Start() {
        lastTail = this.gameObject;
        sound = this.GetComponent<AudioSource>();

        if (algorithm == Algorithm.AStar) {
            StartCoroutine(AStar());
        } else if (algorithm == Algorithm.BFS) {
            StartCoroutine(BFS());
        } else if (algorithm == Algorithm.DFS) {
            StartCoroutine(DFS());
        } else if (algorithm == Algorithm.Dijkstras) {
            StartCoroutine(Dijkstras());
        }
    }

    void Update() {
        if (algorithm == Algorithm.None) {
            ChangeDir();
        }
    }

    void FixedUpdate() {
        if (move) {
            if (algorithm != Algorithm.None) {
                ChangeDirAI();
            }            
            Move();
        }       
    }



    void ChangeDir() {
        if (Input.GetKeyDown("w") && direction != Direction.Down && direction != Direction.Up && movable) {
            direction = Direction.Up;
            movable = false;
        } else if (Input.GetKeyDown("s") && direction != Direction.Down && direction != Direction.Up && movable) {
            direction = Direction.Down;
            movable = false;
        } else if (Input.GetKeyDown("a") && direction != Direction.Left && direction != Direction.Right && movable) {
            direction = Direction.Left;
            movable = false;
        } else if (Input.GetKeyDown("d") && direction != Direction.Left && direction != Direction.Right && movable) {
            direction = Direction.Right;
            movable = false;
        }
    }

    void Move() {
        occupied[GetRowInd(), GetColInd()] = false;
        float nextPos;
        switch (direction) {
            case Direction.Up:
                if ((this.transform.position.y + speed) > 3.5f) {
                    nextPos = -4.5f;
                } else {
                    nextPos = this.transform.position.y + speed;
                }
                this.transform.position = new Vector2(this.transform.position.x, nextPos);
                this.transform.rotation = Quaternion.Euler(this.transform.rotation.x, this.transform.rotation.y, 0f);
                movable = true;
                break;
            case Direction.Down:
                if ((this.transform.position.y - speed) < -4.5f) {
                    nextPos = 3.5f;
                } else {
                    nextPos = this.transform.position.y - speed;
                }
                this.transform.position = new Vector2(this.transform.position.x, nextPos);
                this.transform.rotation = Quaternion.Euler(this.transform.rotation.x, this.transform.rotation.y, 180f);
                movable = true;
                break;
            case Direction.Left:
                if ((this.transform.position.x - speed) < -8.5f) {
                    nextPos = 8.5f;
                } else {
                    nextPos = this.transform.position.x - speed;
                }
                this.transform.position = new Vector2(nextPos, this.transform.position.y);
                this.transform.rotation = Quaternion.Euler(this.transform.rotation.x, this.transform.rotation.y, 90f);
                movable = true;
                break;
            case Direction.Right:
                if ((this.transform.position.x + speed) > 8.5f) {
                    nextPos = -8.5f;
                } else {
                    nextPos = this.transform.position.x + speed;
                }
                this.transform.position = new Vector2(nextPos, this.transform.position.y);
                this.transform.rotation = Quaternion.Euler(this.transform.rotation.x, this.transform.rotation.y, -90f);
                movable = true;
                break;
        }

        occupied[GetRowInd(), GetColInd()] = true;
    }

    void ChangeDirAI() {
        direction = directions[dirInd++];
        if (dirInd >= directions.Length) {
            dirInd = 0;
            if (algorithm == Algorithm.AStar) {
                StartCoroutine(AStar());
            } else if (algorithm == Algorithm.BFS) {
                StartCoroutine(BFS());
            } else if (algorithm == Algorithm.DFS) {
                StartCoroutine(DFS());
            } else if (algorithm == Algorithm.Dijkstras) {
                StartCoroutine(Dijkstras());
            }
        }
    }

    void Grow() {
        GameObject tail = Instantiate(Tail, this.transform.parent);
        tail.transform.rotation = lastTail.transform.rotation;
        tail.transform.position = lastTail.transform.position;
        occupied[(int)(-(tail.transform.position.y - 3.5f) / .5f), (int)((tail.transform.position.x + 8.5f) / .5f)] = true;
        tail.GetComponent<SnakeBody>().Lead = lastTail;
        tail.GetComponent<SnakeBody>().SetNextPos(lastTail.transform.position, lastTail.transform.rotation);
        lastTail = tail;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Food") {
            Grow();
            score += 100;
            ScoreText.text = "SCORE: " + score;
            sound.Play();
        } else if (collision.gameObject.tag == "Tail" && move) {
            SceneManager.LoadScene("Game Over");
        }
    }



    int GetColInd() {
        return (int) ((transform.position.x + 8.5f) / .5f);
    }

    int GetRowInd() {
        return (int) (-(transform.position.y - 3.5f) / .5f);
    }

    Cell[] GetNeighbors(int curX, int curY) {
        List<Cell> neighbors = new List<Cell>();


        if (curX - 1 >= 0) {
            if (!occupied[curY, curX - 1]) {
                neighbors.Add(new Cell(curX - 1, curY));
            }
        } else {
            if (!occupied[curY, maxX]) {
                neighbors.Add(new Cell(maxX, curY));
            }
        }

        if (curY - 1 >= 0) {
            if (!occupied[curY - 1, curX]) {
                neighbors.Add(new Cell(curX, curY - 1));
            }
        } else {
            if (!occupied[maxY, curX]) {
                neighbors.Add(new Cell(curX, maxY));
            }
        }

        if (curX + 1 <= maxX) {
            if (!occupied[curY, curX + 1]) {
                neighbors.Add(new Cell(curX + 1, curY));
            }
        } else {
            if (!occupied[curY, 0]) {
                neighbors.Add(new Cell(0, curY));
            }
        }

        if (curY + 1 <= maxY) {
            if (!occupied[curY + 1, curX]) {
                neighbors.Add(new Cell(curX, curY + 1));
            }
        } else {
            if (!occupied[0, curX]) {
                neighbors.Add(new Cell(curX, 0));
            }
        }

        return neighbors.ToArray();
    }

    Cell[] GetNeighbors(Cell cur) {
        return GetNeighbors(cur.x, cur.y);
    }

    int FoodColInd() {
        return (int)((Food.transform.position.x + 8.5f) / .5f);
    }

    int FoodRowInd() {
        return (int)(-(Food.transform.position.y - 3.5f) / .5f);
    }



    IEnumerator AStar() {
        yield return new WaitForFixedUpdate();
        move = false;

        List<Cell> openList = new List<Cell>();
        List<Cell> closedList = new List<Cell>();
        int x = GetColInd(), y = GetRowInd(), fx = FoodColInd(), fy = FoodRowInd();

        Cell start = new Cell(x, y, 0, manhattan(x, y, fx, fy));
        Cell food = new Cell(fx, fy);
        openList.Add(start);


        while (openList.Count != 0) {
            openList.Sort();
            //Debug.Log(openList.Count);
            Cell current = openList[0];
            openList.RemoveAt(0);
            closedList.Add(current);
            Debug.Log("Current = " + (current.x, current.y));

            if (current.Equals(food)) {
                FormPath(start, current);
                move = true;
                DestroyTiles();
                StopCoroutine("AStar");
                yield break;
            }

            Cell[] neighbors = GetNeighbors(current);
            foreach (Cell neighbor in neighbors) {
                if (closedList.Contains(neighbor)) {
                    continue;
                }
                //Debug.Log((neighbor.x, neighbor.y));
                neighbor.g = current.g + 0.5f; 
                neighbor.h = manhattan(neighbor, food);
                neighbor.SetPrev(current);
                if (openList.Contains(neighbor)) {
                    int ind = openList.IndexOf(neighbor);
                    if (openList[ind] > neighbor) {
                        openList[ind] = neighbor;
                    }
                } else {
                    openList.Add(neighbor);
                }
            }
            DrawTiles(start, current);
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene("Game Over");
        StopCoroutine("AStar");
        yield break;
    }

    IEnumerator BFS()
    {
        yield return new WaitForFixedUpdate();
        move = false;

        List<Cell> Visited = new List<Cell>();
        List<Cell> NotVisited = new List<Cell>();

        int x = GetColInd(), y = GetRowInd();
        Cell start = new Cell(x, y);
        Debug.Log((start.x, start.y));
        Cell food = new Cell(FoodColInd(), FoodRowInd());
        bool foundFood = false;
        Cell foodCell = new Cell(0, 0);

        NotVisited.Add(start);

        while (NotVisited.Count != 0)
        {
            Cell current = NotVisited[0];
            NotVisited.RemoveAt(0);
            Visited.Add(current);

            if (current.Equals(food))
            {
                FormPath(start, current);
                move = true;
                DestroyTiles();
                StopCoroutine("BFS");
                yield break;
            }

            Cell[] neighbours = GetNeighbors(current);
            foreach (Cell neighbour in neighbours)
            {
                if (Visited.Contains(neighbour) || NotVisited.Contains(neighbour))
                {
                    continue;
                }
                else
                {
                    NotVisited.Add(neighbour);
                    neighbour.SetPrev(current);
                }
            }
            DrawTiles(start, current);
            yield return new WaitForEndOfFrame();

        }

        SceneManager.LoadScene("Game Over");
        StopCoroutine("BFS");
        yield break;
    }

    IEnumerator DFS()
    {
        yield return new WaitForFixedUpdate();
        move = false;
        List<Cell> Visited = new List<Cell>();
        List<Cell> NotVisited = new List<Cell>();
        Cell start = new Cell(GetColInd(), GetRowInd());
        Cell food = new Cell(FoodColInd(), FoodRowInd());
        NotVisited.Add(start);

        while (NotVisited.Count != 0)
        {
            Cell current = NotVisited[0];
            NotVisited.RemoveAt(0);
            Visited.Add(current);

            if (current.Equals(food))
            {
                FormPath(start, current);
                move = true;
                DestroyTiles();
                StopCoroutine("DFS");
                yield break;
            }

            Cell[] neighbours = GetNeighbors(current);
            foreach (Cell neighbour in neighbours)
            {
                if (Visited.Contains(neighbour) || NotVisited.Contains(neighbour))
                {
                    continue;
                }
                else
                {
                    NotVisited.Insert(0, neighbour);
                    neighbour.SetPrev(current);
                }
            }

            DrawTiles(start, current);
            yield return new WaitForEndOfFrame();
        }


        SceneManager.LoadScene("Game Over");
        StopCoroutine("DFS");
        yield break;
    }

    IEnumerator Dijkstras() {
        yield return new WaitForFixedUpdate();
        move = false;
        List<Cell> openList = new List<Cell>();
        List<Cell> closedList = new List<Cell>();
        int x = GetColInd(), y = GetRowInd(), fx = FoodColInd(), fy = FoodRowInd();

        Cell start = new Cell(x, y);
        Cell food = new Cell(fx, fy);
        openList.Add(start);


        while (openList.Count != 0)
        {
            openList.Sort();
            //Debug.Log(openList.Count);
            Cell current = openList[0];
            openList.RemoveAt(0);
            closedList.Add(current);
            Debug.Log("Current = " + (current.x, current.y));

            if (current.Equals(food))
            {
                FormPath(start, current);
                move = true;
                DestroyTiles();
                StopCoroutine("Dijkstras");
                yield break;
            }

            Cell[] neighbors = GetNeighbors(current);
            foreach (Cell neighbor in neighbors)
            {
                if (closedList.Contains(neighbor))
                {
                    continue;
                }
                //Debug.Log((neighbor.x, neighbor.y));
                neighbor.SetPrev(current);
                if (openList.Contains(neighbor))
                {
                    int ind = openList.IndexOf(neighbor);
                    if (openList[ind] > neighbor)
                    {
                        openList[ind] = neighbor;
                    }
                }
                else
                {
                    openList.Add(neighbor);
                }
            }
            DrawTiles(start, current);
            yield return new WaitForEndOfFrame();
        }
        SceneManager.LoadScene("Game Over");
        StopCoroutine("Dijkstras");
        yield break;
    }



    void FormPath(Cell from, Cell to) {
        List<Direction> path = new List<Direction>();
        while (to != from) {
            path.Add(to.prev.PointTo(to));
            to = to.prev;
        }
        path.Reverse();
        directions = path.ToArray();
    }

    void DrawTiles(Cell from, Cell to) {
        DestroyTiles();
        tiles.Clear();
        while (to != from) {
            GameObject tile = Instantiate(Tile, new Vector2(to.x * 0.5f - 8.5f, -to.y * 0.5f + 3.5f), Quaternion.identity);
            tiles.Add(tile);
            to = to.prev;
        }
    }

    void DestroyTiles() {
        foreach (GameObject tile in tiles) {
            Destroy(tile);
        }
    }

    int manhattan(int x1, int y1, int x2, int y2) {
        int val1 = Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
        int val2, val3;
        if (x1 < 17) {
            val2 = Mathf.Abs(x1 - x2 + maxX) + Mathf.Abs(y1 - y2);
        } else {
            val2 = Mathf.Abs(x1 - x2 - maxX) + Mathf.Abs(y1 - y2);
        }

        if (y1 < 8) {
            val3 = Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2 + maxY);
        } else {
            val3 = Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2 - maxY);
        }

        if (val1 < val2 && val1 < val3) {
            return val1;
        } else if (val2 < val3) {
            return val2;
        } else {
            return val3;
        }
    }

    int manhattan(Cell cell1, Cell cell2) {
        return manhattan(cell1.x, cell1.y, cell2.x, cell2.y);
    }

    class Cell : IEquatable<Cell>, IComparable<Cell> {
        public int x;
        public int y;
        public float g;
        public float h;
        public Cell prev;

        public Cell(int x, int y, int g, int h) {
            this.x = x;
            this.y = y;
            this.g = g;
            this.h = h;
        }

        public Cell(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public void SetG(int g) {
            this.g = g;
        }

        public void SetH(int h) {
            this.h = h;
        }

        public void SetPrev(Cell prev) {
            this.prev = prev;
        }

        public Direction PointTo(Cell cell) {
            if (this.x == 0 && cell.x == maxX) {
                return Direction.Left;
            } else if (this.x == maxX && cell.x == 0) {
                return Direction.Right;
            } else if (this.y == 0 && cell.y == maxY) {
                return Direction.Up;
            } else if (this.y == maxY && cell.y == 0) {
                return Direction.Down;
            } else if (this.x > cell.x) {
                return Direction.Left;
            } else if (this.x < cell.x) {
                return Direction.Right;
            } else if (this.y < cell.y) {
                return Direction.Down;
            } else {
                return Direction.Up;
            }
        }



        public override bool Equals(object obj) {
            return Equals(obj as Cell);
        }

        public bool Equals(Cell other) {
            return other != null &&
                   x == other.x &&
                   y == other.y;
        }

        public override int GetHashCode() {
            var hashCode = 1502939027;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            return hashCode;
        }

        public int CompareTo(Cell other) {
            if (other == null || this > other) {
                return 1;
            } else if (this < other) {
                return -1;
            } else {
                return 0;
            }
        }

        public static bool operator >(Cell cell1, Cell cell2) {
            if (cell1.g + cell1.h > cell2.g + cell2.h) {
                return true;
            } else {
                return false;
            }
        }

        public static bool operator <(Cell cell1, Cell cell2) {
            if (cell1.g + cell1.h < cell2.g + cell2.h) {
                return true;
            } else {
                return false;
            }
        }
    }

    
}