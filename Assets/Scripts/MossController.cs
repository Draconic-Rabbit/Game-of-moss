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

    private void Awake()
    {
        mossTexture = new Texture2D(1, 1);
        mossTexture.wrapMode = TextureWrapMode.Clamp;
        mossTexture.filterMode = FilterMode.Point;
        mossTexture.Apply();
    }

    public void initGrid(Point gridSize)
    {
        grid = new Grid(gridSize);
        mossTexture = new Texture2D(gridSize.x * cellDisplaySize, gridSize.y * cellDisplaySize);
        initialized = true;
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
    }

    public void StopMossExpansion()
    {
        StopCoroutine(mossExpansionCoroutine);
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
            yield return new WaitForSeconds(mossExpansionSpeed);
        }
    }

    public int getMossCoverPercent()
    {
        int cellsCount = grid.GridSize.x * grid.GridSize.y;
        int mossCount = 0;
        foreach (Cell cell in grid.Cells)
        {
            if (cell.IsAlive) mossCount++;
        }
        int mossCover = Mathf.CeilToInt((float)mossCount / cellsCount * 100);
        return mossCover;
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
                        if (!testCell.IsAlive && Random.value > mossExpansionChance)
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


}
