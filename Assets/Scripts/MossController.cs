using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MossController : MonoBehaviour
{
    private bool initialized = false;

    public Grid grid;
    int cellDisplaySize = 15; // = 960px / 64

    Texture2D mossTexture;
    [SerializeField] Texture2D mossNiceTexture;
    [SerializeField] MeshRenderer mr;
    bool refreshTexture = true;

    Coroutine mossExpansionCoroutine;
    [SerializeField] public float mossExpansionChance = 0.75f;
    [SerializeField] public float mossExpansionSpeed = 0.25f;
    Coroutine mossExpansionModifier;
    float currentMossExpansionChance;
    float currentmossExpansionSpeed;

    private void Start()
    {
        currentMossExpansionChance = mossExpansionChance;
        currentmossExpansionSpeed = mossExpansionSpeed;
    }


    public void initGrid(Point gridSize)
    {
        grid = new Grid(gridSize);
        PrepareMossTexture(gridSize.x * cellDisplaySize, gridSize.y * cellDisplaySize);
        initialized = true;
    }

    private void PrepareMossTexture(int width, int height)
    {
        mossTexture = new Texture2D(width, height);
        mossTexture.wrapMode = TextureWrapMode.Clamp;
        mossTexture.filterMode = FilterMode.Point;
        // mossTexture.alphaIsTransparency = true;
        mossTexture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) return;

        if (refreshTexture) RefreshMossTexture();
    }

    public void StartMossExpansion()
    {
        mossExpansionCoroutine = StartCoroutine(ExpandMossCoroutine());
        mossExpansionModifier = StartCoroutine(ModifyParamsCoroutine());
    }

    public void StopMossExpansion()
    {
        StopCoroutine(mossExpansionCoroutine);
        StopCoroutine(mossExpansionModifier);
    }

    internal bool mossStatusAtWorldPos(Vector3 worldPos)
    {
        Point point = this.grid.WorldToGridPoint(worldPos);
        if (grid.checkPointInGrid(point))
        {
            return grid.Cells[point.x, point.y].IsAlive;
        }
        return false;
    }

    public void KillMossAt(Point targetPos)
    {
        if (this.grid.checkPointInGrid(targetPos))
        {
            grid.Cells[targetPos.x, targetPos.y].IsAlive = false;
            refreshTexture = true;
        }
    }

    public void SpawnMossAt(Point targetPos)
    {
        if (this.grid.checkPointInGrid(targetPos))
        {
            grid.Cells[targetPos.x, targetPos.y].IsAlive = true;
            refreshTexture = true;
        }
    }

    IEnumerator ExpandMossCoroutine()
    {
        while (true)
        {
            CalcutateMossExpandion();
            refreshTexture = true;
            yield return new WaitForSeconds(currentmossExpansionSpeed);
        }
    }

    IEnumerator ModifyParamsCoroutine()
    {
        // 2 second big expansion to let moss a chance
        currentMossExpansionChance = 0.79f;
        currentmossExpansionSpeed = 0.22f;
        yield return new WaitForSeconds(2f);
        while (true)
        {
            SetNewMossParams();
            yield return new WaitForSeconds(4f);
        }
    }

    void SetNewMossParams()
    {
        int mossCover = getMossCoverPercent();
        // Moss loose, let player finish
        if (mossCover <= 2)
        {
            currentMossExpansionChance = mossExpansionChance;
            currentmossExpansionSpeed = mossExpansionSpeed;
            return;
        }
        // High Expansion Defense mode
        if (mossCover <= 4)
        {
            currentMossExpansionChance = mossExpansionChance + Random.Range(0.10f, 0.15f);
            currentmossExpansionSpeed = mossExpansionSpeed - Random.Range(0.05f, 0.10f);
            return;
        }
        // High Expansion : Finish Him
        if (mossCover > 55)
        {
            currentMossExpansionChance = mossExpansionChance + Random.Range(0.1f, 0.22f);
            currentmossExpansionSpeed = mossExpansionSpeed - Random.Range(0.15f, 0.2f);
            return;
        }

        currentMossExpansionChance = mossExpansionChance;
        currentmossExpansionSpeed = mossExpansionSpeed;
    }

    public int getMossCoverPercent()
    {
        int cellsCount = grid.GridSize.x * grid.GridSize.y;
        int mossCount = GetMossCellCount();
        int mossCover = Mathf.CeilToInt((float)mossCount / cellsCount * 100);
        return mossCover;
    }

    private int GetMossCellCount()
    {
        int mossCount = 0;
        foreach (Cell cell in grid.Cells)
        {
            if (cell.IsAlive) mossCount++;
        }
        return mossCount;
    }

    private void CalcutateMossExpandion()
    {
        Grid nextGrid = grid.Clone();
        foreach (Cell cell in grid.Cells)
        {
            if (cell.IsAlive)
            {
                Point[] adjacentPoints = new Point[] {
                    new Point(cell.position.x, cell.position.y + 1),
                    new Point(cell.position.x, cell.position.y - 1),
                    new Point(cell.position.x - 1, cell.position.y),
                    new Point(cell.position.x + 1, cell.position.y)
                };

                foreach (Point point in adjacentPoints)
                {
                    if (this.grid.checkPointInGrid(point))
                    {
                        Cell testCell = grid.Cells[point.x, point.y];
                        if (!testCell.IsAlive && Random.value < currentMossExpansionChance)
                        {
                            nextGrid.Cells[point.x, point.y] = new Cell(new Point(point.x, point.y), true);
                        }
                    }
                }
            }
        }
        grid = nextGrid;
    }

    private void RefreshMossTexture()
    {
        // MossTexturePlainColor();
        MossTextureNice();
    }

    private void MossTexturePlainColor()
    {
        int colorsSize = cellDisplaySize * cellDisplaySize;
        Color[] aliveColors = new Color[colorsSize];
        Array.Fill<Color>(aliveColors, Color.green);
        Color[] deadColors = new Color[colorsSize];
        Array.Fill<Color>(deadColors, Color.gray);
        Color[] colors = new Color[colorsSize];
        foreach (Cell cell in grid.Cells)
        {
            if (cell.IsAlive)
            {
                colors = aliveColors;
            }
            else
            {
                colors = deadColors;
            }

            mossTexture.SetPixels(cell.position.x * cellDisplaySize, cell.position.y * cellDisplaySize, cellDisplaySize, cellDisplaySize, colors);
        }
        mossTexture.Apply();
        mr.material.mainTexture = mossTexture;
        refreshTexture = false;
    }

    private void MossTextureNice()
    {
        int colorsSize = cellDisplaySize * cellDisplaySize;
        Color[] aliveColors = mossNiceTexture.GetPixels();
        Color[] deadColors = new Color[colorsSize];
        Array.Fill<Color>(deadColors, new Color32(0, 0, 0, 0));
        Color[] colors = new Color[colorsSize];
        foreach (Cell cell in grid.Cells)
        {
            if (cell.IsAlive)
            {
                colors = aliveColors;
            }
            else
            {
                colors = deadColors;
            }

            mossTexture.SetPixels(cell.position.x * cellDisplaySize, cell.position.y * cellDisplaySize, cellDisplaySize, cellDisplaySize, colors);
        }
        mossTexture.Apply();
        mr.material.mainTexture = mossTexture;
        refreshTexture = false;

    }


}
