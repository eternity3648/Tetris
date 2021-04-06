using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;
using DG.Tweening;


public class TetrisGrid : MonoBehaviour
{
    public int sizeX, sizeY;
    public Cell[,] cells;
    public GameObject BorderLinePrefab;
    public GameObject CubePrefab, TrailPrefab;
    public Sprite[] cubeSprites;
    public Material cubeCopyMat;
    public GameObject figurePrefab;
    public Vector3 cellSize;
    public float slowFigureSpeed;
    public float fastFigureSpeed;
    public float superFastFigureSpeed;
    public float delayBeforeFigureLanding;
    public float startFastHorizontalMovementDelat;
    public float fallAnimationDelay;
    public GameObject currentFigure;
    public Figure figScript;
    public GameObject fallFigureCopy;
    public float figureFallDelayDecreaseCurrentTime = 0;
    public float speedCoeff = 1.0f;
    public int nextFigureIndex;
    public float figureFallDelay;
    public float minFigureFallDelay;
    public float figureFallDelayDecreaseValue;
    public float figureFallDelayDecreaseTime;

    private bool figureFastFalling = false;
    private float currentFigureFallTime = 0;
    private float currentFigureSpeed;
    private Vector3 startPositon;
    private Vector2 currentFigureCoord;
    private Cell currentCell;
    private float currentDelayBeforeFigureLanding;
    private TweenCallback<int> OnLineDestroy;
    private TweenCallback<GameObject> OnFigureCreate;
    private TweenCallback OnFigureFastFall;
    private TweenCallback OnGameStart;
    private bool pause = false;
    private Action OnFigureLand;

    private enum Sides
    {
        Left = 1,
        Top = 2,
        Right = 3,
        Bottom = 4
    }

    public void Start()
    {
        foreach (Transform child in this.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        FigureTypes.Init();

        cells = new Cell[sizeX, sizeY];
        startPositon = new Vector3(-cellSize.x * (sizeX / 2), cellSize.y * (sizeX / 2));
        currentDelayBeforeFigureLanding = 0;
        speedCoeff = 1.0f;
        figureFallDelayDecreaseCurrentTime = 0;
        nextFigureIndex = -1;

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                cells[x, y] = new Cell();
                Vector3 cellCenterPosition = GetCellPosition(new Vector2(x, y));

                if (x == sizeX - 1)
                {
                    DrawBorder(Sides.Right, cellCenterPosition);
                }
                if (y == 0)
                {
                    DrawBorder(Sides.Top, cellCenterPosition);
                }
                if (x == 0)
                {
                    DrawBorder(Sides.Left, cellCenterPosition);
                }
                if (y == sizeY - 1)
                {
                    DrawBorder(Sides.Bottom, cellCenterPosition);
                }
            }
        }

        OnGameStart();
    }

    public void SetPause(bool value)
    {
        pause = value;
    }

    public void Update()
    {
        float delta = Time.deltaTime;
        figureFallDelayDecreaseCurrentTime += delta;
        currentFigureFallTime += delta;

        if (!pause && !figureFastFalling)
        {
            if (figureFallDelayDecreaseCurrentTime >= figureFallDelayDecreaseTime)
            {
                figureFallDelay -= figureFallDelayDecreaseValue;
                if (figureFallDelay < minFigureFallDelay) figureFallDelay = minFigureFallDelay;
                figureFallDelayDecreaseCurrentTime -= figureFallDelayDecreaseTime;
            }

            Vector3 previousFigurePosition = currentFigure.transform.localPosition;
            Vector3 posDiff = new Vector3(0, -cellSize.y + 0.0001f, 0);
            Vector3 figurePosition = previousFigurePosition + posDiff;
            Vector2 prevCoord = GetCellСoordByPosition(previousFigurePosition);
            Vector2 coord = GetCellСoordByPosition(figurePosition);

            if (currentDelayBeforeFigureLanding == 0 && (currentFigure != null && currentFigureFallTime > figureFallDelay && delta < 0.2f))
            {
                currentFigureFallTime = 0;

                if (CheckIfFigureCanExistInCoord(figScript, coord))
                {
                    currentFigureCoord = coord;
                    currentFigure.transform.localPosition = figurePosition;
                    currentDelayBeforeFigureLanding = 0;
                }
                else if (currentFigureCoord == new Vector2(1000, 1000))
                {
                    Destroy(currentFigure);
                    currentFigure = null;
                }
                else if (currentDelayBeforeFigureLanding == 0)
                {
                    if (CheckIfFigureCanBeRotatedOrMoved())
                        currentDelayBeforeFigureLanding = delayBeforeFigureLanding;
                    else
                        LandFigure(prevCoord);
                }
            }
            else if (currentDelayBeforeFigureLanding != 0)
            {
                currentDelayBeforeFigureLanding -= delta;
                if (currentDelayBeforeFigureLanding <= 0)
                {
                    if (!CheckIfFigureCanExistInCoord(figScript, coord))
                        LandFigure(prevCoord);
                }
            }
        }
    }
    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
    }

    public void ClearCells()
    {
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                if (!cells[x, y].IsFree())
                    cells[x, y].DestroyCube();
            }
        }
    }

    public void LaunchStartFigure()
    {
        Vector3 startPosition = GetFigureStartPosition();
        currentFigureSpeed = slowFigureSpeed;

        int figureIndex;
        if (nextFigureIndex == -1)
            figureIndex = FigureTypes.GetRangdomIndex();
        else
            figureIndex = nextFigureIndex;

        nextFigureIndex = FigureTypes.GetRangdomIndex();
        int[,] figureMatrix = FigureTypes.GetFigureByIndex(figureIndex);
        currentFigure = CreateFigure(figureMatrix, figureIndex);
        figScript = currentFigure.GetComponent<Figure>();

        currentCell = GetCellByPosition(startPosition);
        if (CheckIfCellIsFreeForBlock(currentCell))
        {
            currentFigure.transform.localPosition = startPosition;
            currentFigureCoord = new Vector2(1000, 1000);
            CheckIfFigureCanExistInCoord(figScript, GetCellСoordByPosition(startPosition));
            currentFigureCoord = GetCellСoordByPosition(startPosition);
            UpdateFallFigureTip();
        }

        currentDelayBeforeFigureLanding = 0;
        figureFallDelayDecreaseCurrentTime = 0;
        figureFastFalling = false;

        OnFigureCreate(CreateFigure(FigureTypes.GetFigureByIndex(nextFigureIndex), nextFigureIndex, false));
    }

    public void LaunchSavedStartFigure(Vector3 startPosition, int figureIndex, int rotationCount, int nextFigureIndex)
    {
        currentFigureSpeed = slowFigureSpeed;

        int[,] figureMatrix = FigureTypes.GetFigureByIndex(figureIndex);
        currentFigure = CreateFigure(figureMatrix, figureIndex);
        currentFigure.transform.localPosition = startPosition;
        currentFigureCoord = GetCellСoordByPosition(startPosition);
        figScript = currentFigure.GetComponent<Figure>();

        for (int i = 0; i < rotationCount; i++)
            figScript.Rotate();

        currentCell = GetCellByPosition(startPosition);
        UpdateFallFigureTip();

        OnFigureCreate(CreateFigure(FigureTypes.GetFigureByIndex(nextFigureIndex), nextFigureIndex, false));
    }

    public void SetOnFigureCreate(TweenCallback<GameObject> callb)
    {
        OnFigureCreate = callb;
    }

    public void SetOnFigureFastFall(TweenCallback callb)
    {
        OnFigureFastFall = callb;
    }

    public void SetOnGameStart(TweenCallback callb)
    {
        OnGameStart = callb;
    }

    // direction true - right, false - left
    public void MoveFigure(bool direction)
    {
        if (!figureFastFalling)
        {
            Vector3 positionShift = new Vector3(cellSize.x, 0);
            if (!direction) { positionShift = -positionShift; }
            Vector2 coord = GetCellСoordByPosition(currentFigure.transform.localPosition + positionShift);

            if (CheckIfFigureCanExistInCoord(figScript, coord))
            {
                currentFigure.transform.localPosition += positionShift;
                currentFigureCoord = GetCellСoordByPosition(currentFigure.transform.localPosition);
                UpdateFallFigureTip();
                if (currentDelayBeforeFigureLanding != 0) currentDelayBeforeFigureLanding = delayBeforeFigureLanding;
            }
        }
    }

    public void ApplyFigureSuperFastFall()
    {
        if (!figureFastFalling)
        {
            figureFastFalling = true;
            Vector3 pos = currentFigure.transform.localPosition;
            Vector3 prevPos = pos;
            float fallCellTime = 0.015f;

            Vector2 coord = GetCellСoordByPosition(pos);
            while (CheckIfFigureCanExistInCoord(figScript, coord))
            {
                prevPos = pos;
                pos += new Vector3(0, -cellSize.y + 0.0001f, 0);
                coord = GetCellСoordByPosition(pos);
            }

            void OnFinish()
            {
                LandFigure(GetCellСoordByPosition(prevPos));
            }

            if (prevPos != currentFigure.transform.localPosition)
            {
                int count = (int)Math.Abs((currentFigure.transform.localPosition.y - prevPos.y) / cellSize.y);
                currentFigure.transform.DOLocalMoveY(prevPos.y, fallCellTime * count).OnComplete(OnFinish);

                GameObject[] blocks = currentFigure.GetComponent<Figure>().blocks;
                List<float> uniqueXCoords = new List<float>();
                List<float> uniqueYCoords = new List<float>();
                float[] yCount = new float[4];
                float[] yCountX = new float[4];

                for (int i = 1; i < blocks.Length; i++)
                {
                    Vector3 pos1 = blocks[i].transform.localPosition;
                    if (!uniqueXCoords.Contains(pos1.x)) uniqueXCoords.Add(pos1.x);
                    if (!uniqueYCoords.Contains(pos1.y))
                    {
                        uniqueYCoords.Add(pos1.y);
                        yCount[uniqueYCoords.IndexOf(pos1.y)] = 1;
                        yCountX[uniqueYCoords.IndexOf(pos1.y)] = pos1.x;
                    }
                    else
                    {
                        yCount[uniqueYCoords.IndexOf(pos1.y)]++;
                    };
                }

                float maxYValue = -1000;
                for (int i = 0; i < uniqueYCoords.Count; i++)
                {
                    float pos1 = uniqueYCoords[i];
                    if (pos1 > maxYValue && yCount[i] == yCount.Max())
                    {
                        maxYValue = pos1;
                    }
                }


                Vector3 trailPos = new Vector3(
                    (uniqueXCoords.Min() + (uniqueXCoords.Max() - uniqueXCoords.Min()) / 2),
                    maxYValue,
                    0
                );

                trailPos = trailPos - new Vector3(0, cellSize.y / 2);
                GameObject trail = Instantiate(TrailPrefab, currentFigure.transform);
                trail.transform.localPosition = trailPos;
                AnimationCurve curve = new AnimationCurve();
                curve.AddKey(0.0f, 0.659f * uniqueXCoords.Count);
                trail.GetComponent<TrailRenderer>().widthCurve = curve;

                ParticleSystem ps = trail.GetComponent<ParticleSystem>();
                var sh = ps.shape;
                sh.scale = new Vector3(0.3f * uniqueXCoords.Count, 1f, 1f);


                //List<Vector2> coords = currentFigure.GetComponent<Figure>().GetBlockCoordsRelativeToCoord(coord);

                //coords.ForEach(delegate (Vector2 blockCoord)
                //{
                //    CreateBlockInCell(blockCoord, figScript.GetIndex());
                //});
                //for (int i = 1; i < blocks.Length; i++)
                //{
                //    Vector2 coord1 = currentFigure.GetComponent<Figure>().GetBlockCoord(i);
                //    if (coord1.y == highestI)
                //        blocks[i].GetComponent<TrailRenderer>().enabled = true;
                //}
            }
        }
    }

    public bool CanFigureSpeedBeChanged()
    {
        return currentFigureSpeed != superFastFigureSpeed;
    }

    public void SetOnLinesDestroy(TweenCallback<int> OnDestroy)
    {
        OnLineDestroy = OnDestroy;
    }

    public void RotateFigure()
    {
        if (!figureFastFalling)
        {
            GameObject testFigure = CreateFigure(figScript.GetMatrix(), figScript.GetIndex());
            Figure testFigScript = testFigure.GetComponent<Figure>();
            testFigScript.Rotate();
            Vector2 coord = GetCellСoordByPosition(currentFigure.transform.localPosition);
            int coordShiftRange = 1;
            if (figScript.blockMatrix.GetLength(0) == 4)
                coordShiftRange = 2;

            for (int i = 0; i <= coordShiftRange; i++)
            {
                Vector2 testLeftCoord = coord + new Vector2(-i, 0);
                if (CheckIfFigureCanExistInCoord(testFigScript, testLeftCoord))
                {
                    currentFigure.transform.localPosition += new Vector3(-i * cellSize.x, 0);
                    figScript.Rotate();
                    UpdateFallFigureTip();
                    break;
                }
                Vector2 testRightCoord = coord + new Vector2(i, 0);
                if (CheckIfFigureCanExistInCoord(testFigScript, testRightCoord))
                {
                    currentFigure.transform.localPosition += new Vector3(i * cellSize.x, 0);
                    figScript.Rotate();
                    UpdateFallFigureTip();
                    break;
                }
            }
            Destroy(testFigure);
        }
    }

    public void MoveFigureDownOnOneCell()
    {
        currentFigureFallTime = figureFallDelay;
    }

    public void SetOnFigureLand(Action func)
    {
        OnFigureLand = func;
    }

    private void LandFigure(Vector2 coord)
    {
        int coordX = (int)coord.x;
        int coordY = (int)coord.y;

        for (int y = coordY; y >= 0; y--)
        {
            coord = new Vector2(coordX, y);
            if (CheckIfFigureCanExistInCoord(figScript, coord))
            {
                currentDelayBeforeFigureLanding = 0;
                currentFigureFallTime = 0;
                List<Vector2> coords = figScript.GetBlockCoordsRelativeToCoord(coord);

                coords.ForEach(delegate (Vector2 blockCoord)
                {
                    CreateBlockInCell(blockCoord, figScript.GetIndex());
                });

                GameObject[] blocks = currentFigure.GetComponent<Figure>().blocks;
                for (int i = 1; i < blocks.Length; i++)
                {
                    blocks[i].SetActive(false);
                }

                GameObject fig = currentFigure;
                void DestroyFigure()
                {
                    Destroy(fig);
                }

                Sequence mySequence = DOTween.Sequence();
                mySequence.AppendInterval(1).OnComplete(DestroyFigure);
                currentFigure = null;
                CheckForBlocksRemoving();
                break;
            }
        }

        OnFigureLand?.Invoke();
        // TODO, call OnGameLost when figure is not landed after code above
    }

    private GameObject CreateFigure(int[,] matrix, int figureIndex, bool attachToGrid = true)
    {
        GameObject figure;
        if (attachToGrid)
        {
            figure = Instantiate(figurePrefab, this.transform);
        }
        else
        {
            figure = Instantiate(figurePrefab);
        }
        Figure script = figure.GetComponent<Figure>();
        script.Set(matrix, CubePrefab, cellSize, figureIndex);
        return figure;
    }


    private void DrawBorder(Sides side, Vector3 centerPosition)
    {
        GameObject borderLine = Instantiate(BorderLinePrefab, this.transform);
        Transform transform = borderLine.transform;
        Vector3 borderPosition = new Vector3();
        Vector3 angles = transform.rotation.eulerAngles;

        if (side == Sides.Top)
        {
            borderPosition = centerPosition + new Vector3(0, cellSize.y * 0.5f);
        }
        else if (side == Sides.Right)
        {
            borderPosition = centerPosition + new Vector3(cellSize.x * 0.5f, 0);
            angles.z += 90;
        }
        else if (side == Sides.Bottom)
        {
            borderPosition = centerPosition - new Vector3(0, cellSize.y * 0.5f);
        }
        else if (side == Sides.Left)
        {
            borderPosition = centerPosition - new Vector3(cellSize.x * 0.5f, 0);
            angles.z += 90;
        }

        transform.rotation = Quaternion.Euler(angles);
        transform.localPosition = borderPosition;
        borderLine.SetActive(true);
    }

    public void CreateBlockInCell(Vector2 coord, int figureIndex)
    {
        GameObject figureCube = Instantiate(CubePrefab, this.transform);
        Color color = FigureTypes.GetСolor(figureIndex);
        figureCube.GetComponent<SpriteRenderer>().color = color;
        figureCube.GetComponent<SpriteRenderer>().material.color = color;
        figureCube.transform.localPosition = GetCellPosition(coord);
        figureCube.SetActive(true);
        Cell cell = GetCellByCoord(coord);
        cell.occupyingCube = figureCube;
        cell.cubeIndex = figureIndex;
    }

    private void UpdateFallFigureTip()
    {
        Vector2 figureCopyCoord = currentFigureCoord;
        if (fallFigureCopy != null) Destroy(fallFigureCopy);
        fallFigureCopy = CreateFigure(figScript.GetMatrix(), figScript.GetIndex(), true);
        Figure fallFigureCopyScript = fallFigureCopy.GetComponent<Figure>();

        while (CheckIfFigureCanExistInCoord(fallFigureCopyScript, figureCopyCoord + new Vector2(0, 1))) {
            figureCopyCoord += new Vector2(0, 1);
        }

        fallFigureCopy.transform.localPosition = GetCellPosition(figureCopyCoord);

        for (int i = 0; i < fallFigureCopyScript.blocks.Length; i++)
        {
            if (fallFigureCopyScript.blocks[i] != null)
            {
                fallFigureCopyScript.blocks[i].GetComponent<SpriteRenderer>().material = cubeCopyMat;
                Material mat = fallFigureCopyScript.blocks[i].GetComponent<SpriteRenderer>().material;
                Color oldColor = mat.color;
                Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 0.2f);
                mat.SetColor("_Color", newColor);
            }
        }
    }

    private bool CheckIfFigureCanBeRotatedOrMoved()
    {
        bool result = false;
        GameObject testFigure = CreateFigure(figScript.GetMatrix(), figScript.GetIndex());
        Figure testFigScript = testFigure.GetComponent<Figure>(); ;
        Vector2 coord = GetCellСoordByPosition(currentFigure.transform.localPosition);

        if (CheckIfFigureCanExistInCoord(testFigScript, coord + new Vector2(1, 0)) || CheckIfFigureCanExistInCoord(testFigScript, coord + new Vector2(-1, 0)))
            result = true;

        testFigScript.Rotate();
        if (CheckIfFigureCanExistInCoord(testFigScript, coord))
            result = true;

        Destroy(testFigure);

        return result;
    }

    private Vector3 GetCellPosition(Vector2 coord)
    {
        Vector3 position = startPositon + new Vector3(cellSize.x * (coord.x + 0.5f), -cellSize.y * (coord.y + 0.5f));
        return position;
    }

    private Vector2 GetCellСoordByPosition(Vector3 position)
    {
        position -= startPositon;
        return new Vector2(Convert.ToInt32((position.x / cellSize.x) - 0.5f), -Convert.ToInt32((position.y / cellSize.y)));
    }

    private Cell GetCellByPosition(Vector3 position)
    {
        return GetCellByCoord(GetCellСoordByPosition(position));
    }

    private Cell GetCellByCoord(Vector2 coord)
    {
        if (coord.x < 0 || coord.x >= sizeX || coord.y < 0 || coord.y >= sizeY)
        {
            return null;
        }
        else
        {
            //print(coord.x);
            //print(coord.y);
            return cells[Convert.ToInt32(coord.x), Convert.ToInt32(coord.y)];
        }
    }

    private Vector3 GetFigureStartPosition()
    {
        Vector2 startCoord = new Vector2(Math.Abs(sizeX / 2) - 2, 0);
        return GetCellPosition(startCoord);
    }

    private bool CheckIfCellIsFreeForBlock(Cell cell)
    {
        return cell != null && cell.IsFree();
    }

    private bool CheckIfFigureCanExistInCoord(Figure script, Vector2 coord)
    {
        List<Vector2> coords = script.GetBlockCoordsRelativeToCoord(coord);
        bool canExist = true;
        coords.ForEach(delegate (Vector2 blockCoord)
        {
            if (canExist)
            {
                Cell cell = GetCellByCoord(blockCoord);
                canExist = CheckIfCellIsFreeForBlock(cell);
            }
        });

        return canExist;
    }

    private void CheckForBlocksRemoving()
    {
        List<int> removedLinesIndices = new List<int>();
        for (int y = 0; y < sizeY; y++)
        {
            bool is_line_filled = true;
            for (int x = 0; x < sizeX; x++)
            {
                if (cells[x, y].IsFree())
                {
                    is_line_filled = false;
                    break;
                }
            }
            if (is_line_filled)
            {
                RemoveBlocksFromLine(y);
                removedLinesIndices.Add(y);
            }
        }
        ShiftBlocks(removedLinesIndices);
        OnLineDestroy(removedLinesIndices.Count);
    }

    private void RemoveBlocksFromLine(int y)
    {
        for (int x = 0; x < sizeX; x++)
        {
            cells[x, y].DestroyCube();
        }
    }

    private void ShiftBlocks(List<int> removedLinesIndices)
    {
        bool isAnimationFirst = false;

        for (int y = sizeY - 1; y >= 0; y--)
        {
            for (int x = sizeX - 1; x >= 0; x--)
            {
                int removedLinesCounterUnder = 0;
                removedLinesIndices.ForEach(delegate (int y1)
                {
                    if (y1 > y)
                    {
                        removedLinesCounterUnder++;
                    }
                });

                Cell cell = GetCellByCoord(new Vector2(x, y));
                if (!cell.IsFree() && removedLinesCounterUnder > 0)
                {
                    int cubeIndex = cell.cubeIndex;
                    cell.DestroyCube(false);
                    Vector2 newCoord = new Vector2(x, y + removedLinesCounterUnder);
                    CreateBlockInCell(newCoord, cubeIndex);
                    TweenCallback OnFinish = LaunchStartFigure;
                    if (isAnimationFirst) OnFinish = null;
                    isAnimationFirst = true;
                    AddCubeFallAnimation(GetCellByCoord(newCoord).occupyingCube, removedLinesCounterUnder, OnFinish);
                }
            }
        }

        if (!isAnimationFirst)
        {
            LaunchStartFigure();
        }
    }
    private void AddCubeFallAnimation(GameObject cube, int linesFallCount, TweenCallback OnFinish)
    {
        Transform tran = cube.transform;
        Vector3 targetPosition = tran.localPosition;
        Vector3 startPosition = targetPosition + new Vector3(0, cellSize.y * linesFallCount);
        tran.localPosition = startPosition;
        Sequence mySequence = DOTween.Sequence();
        mySequence.AppendInterval(fallAnimationDelay);
        mySequence.Append(tran.DOLocalMoveY(targetPosition.y, fallAnimationDelay).OnComplete(OnFinish));
    }
}

public class Cell
{
    public GameObject occupyingCube;
    public int cubeIndex;

    public bool IsFree()
    {
        return (occupyingCube == null);
    }

    public void DestroyCube(bool animation = true, Action OnFinish = null)
    {
        GameObject cube = occupyingCube;
        Material material = cube.GetComponent<SpriteRenderer>().material;
        occupyingCube = null;
        cubeIndex = -1;

        void Destroy()
        {
            GameObject.Destroy(cube);
            OnFinish?.Invoke();
        }

        void ChangeMaterialFade(float value)
        {
            material.SetFloat("_Fade", value);
        }

        if (animation)
            DOTween.To(ChangeMaterialFade, 1, 0, 0.6f).OnComplete(Destroy);
        else
            Destroy();
    }
}
