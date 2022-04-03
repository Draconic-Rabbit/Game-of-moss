using UnityEngine;

public struct Grid
{
    private Cell[,] cells;
    private Point gridSize;

    public Grid(Point gridSize)
    {
        this.gridSize = gridSize;
        cells = new Cell[(int)gridSize.x, (int)gridSize.y];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Cell cell = new Cell(new Point(x, y));
                cells[x, y] = cell;
            }
        }
    }

    public Cell[,] Cells { get => cells; set => cells = value; }
    public Point GridSize { get => gridSize; set => gridSize = value; }

    public bool checkPointInGrid(Point p)
    {
        return (p.x >= 0 && p.x < gridSize.x && p.y >= 0 && p.y < gridSize.y);
    }

    // Have to clone Cell[,], ref vs value
    public Grid Clone()
    {
        Grid clone = new Grid(this.gridSize);
        clone.Cells = (Cell[,])Cells.Clone();
        return clone;
    }

    public Point WorldToGridPoint(Vector3 worldPosition)
    {
        int gridResolution = gridSize.x / 16;
        Vector2 positiveWorldPosition = new Vector2(worldPosition.x + 8, worldPosition.y + 5);
        Vector2 cellPosition = new Vector2(positiveWorldPosition.x * gridResolution, positiveWorldPosition.y * gridResolution);
        return new Point(Mathf.FloorToInt(cellPosition.x), Mathf.FloorToInt(cellPosition.y));
    }
}