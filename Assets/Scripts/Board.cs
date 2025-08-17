using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.UIElements; // Unused

/// <summary>
/// Board class manages the generation and functionality of the Sudoku board,
/// including grid creation, puzzle generation, button creation, and checking for completion.
/// </summary>
public class Board : MonoBehaviour
{
    // The complete solved Sudoku grid.
    private int[,] grid = new int[9, 9];

    // The puzzle grid with some cells removed according to difficulty.
    private int[,] puzzle = new int[9, 9];

    // Cached references to instantiated SudokuCell components for fast lookup.
    private SudokuCell[,] cells = new SudokuCell[9, 9];

    // Number of cells to remove based on difficulty. It is set from PlayerSettings.
    private int difficulty = 15;

    // Transforms for each Sudoku square (3x3 blocks) in the board.
    [SerializeField, Tooltip("Parent transform for square 0,0 (top-left 3x3 block)")]
    private Transform square00;
    [SerializeField, Tooltip("Parent transform for square 0,1 (top-center 3x3 block)")]
    private Transform square01;
    [SerializeField, Tooltip("Parent transform for square 0,2 (top-right 3x3 block)")]
    private Transform square02;
    [SerializeField, Tooltip("Parent transform for square 1,0 (middle-left 3x3 block)")]
    private Transform square10;
    [SerializeField, Tooltip("Parent transform for square 1,1 (center 3x3 block)")]
    private Transform square11;
    [SerializeField, Tooltip("Parent transform for square 1,2 (middle-right 3x3 block)")]
    private Transform square12;
    [SerializeField, Tooltip("Parent transform for square 2,0 (bottom-left 3x3 block)")]
    private Transform square20;
    [SerializeField, Tooltip("Parent transform for square 2,1 (bottom-center 3x3 block)")]
    private Transform square21;
    [SerializeField, Tooltip("Parent transform for square 2,2 (bottom-right 3x3 block)")]
    private Transform square22;

    // Prefab for a single Sudoku cell.
    [SerializeField, Tooltip("Prefab for the Sudoku cell")]
    private GameObject SudokuCell_Prefab;

    // GameObject to display when the puzzle is solved.
    [SerializeField, Tooltip("Win menu GameObject shown when puzzle is completed")]
    private GameObject winMenu;

    // UI element for displaying lose message.
    [SerializeField, Tooltip("UI element to display a lose message")]
    private GameObject loseText;

    /// <summary>
    /// Start is called before the first frame update.
    /// Initializes the board, sets difficulty from PlayerSettings, and creates the grid, puzzle, and buttons.
    /// </summary>
    private void Start()
    {
        // Check for required GameObjects.
        if (winMenu == null)
        {
            Debug.LogError("Win menu GameObject is not assigned in the Inspector.");
        }
        if (loseText == null)
        {
            Debug.LogError("Lose text GameObject is not assigned in the Inspector.");
        }

        // Initially hide the win menu.
        winMenu.SetActive(false);

        // Set the difficulty based on PlayerSettings.
        difficulty = PlayerSettings.difficulty;

        // Create the solved grid and the puzzle.
        CreateGrid();
        CreatePuzzle();

        // Create the Sudoku cell buttons on the board.
        CreateButtons();
    }

    /// <summary>
    /// Update is called once per frame.
    /// Currently not used but kept for potential future use.
    /// </summary>
    private void Update()
    {
        // No updates required per frame for now.
    }

    /// <summary>
    /// Outputs the grid values to the console for debugging.
    /// </summary>
    /// <param name="g">The grid to output</param>
    private void ConsoleOutputGrid(int[,] g)
    {
        string output = "";
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                output += g[i, j] + " ";
            }
            output += "\n";
        }
        // Uncomment the next line for debugging output.
        // Debug.Log(output);
    }

    /// <summary>
    /// Checks if a given column already contains the specified value.
    /// </summary>
    private bool ColumnContainsValue(int col, int value)
    {
        for (int i = 0; i < 9; i++)
        {
            if (grid[i, col] == value)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if a given row already contains the specified value.
    /// </summary>
    private bool RowContainsValue(int row, int value)
    {
        for (int i = 0; i < 9; i++)
        {
            if (grid[row, i] == value)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if the 3x3 block containing the specified cell already contains the value.
    /// </summary>
    private bool SquareContainsValue(int row, int col, int value)
    {
        // Determine the starting indices for the 3x3 block.
        int startRow = (row / 3) * 3;
        int startCol = (col / 3) * 3;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (grid[startRow + i, startCol + j] == value)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Checks whether a value can be placed at the given cell without conflicts.
    /// </summary>
    private bool CheckAll(int row, int col, int value)
    {
        if (ColumnContainsValue(col, value))
        {
            return false;
        }
        if (RowContainsValue(row, value))
        {
            return false;
        }
        if (SquareContainsValue(row, col, value))
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if the grid is completely filled.
    /// </summary>
    private bool IsValid()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (grid[i, j] == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Creates a complete Sudoku grid by filling in values and solving the puzzle.
    /// </summary>
    private void CreateGrid()
    {
        // Create lists of numbers 1 to 9 for initial random assignment.
        List<int> rowList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        List<int> colList = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        // Place a random value in the top-left cell and remove it from the lists.
        int value = rowList[Random.Range(0, rowList.Count)];
        grid[0, 0] = value;
        rowList.Remove(value);
        colList.Remove(value);

        // Fill the first column (except the top cell) with the remaining random values.
        for (int i = 1; i < 9; i++)
        {
            value = rowList[Random.Range(0, rowList.Count)];
            grid[i, 0] = value;
            rowList.Remove(value);
        }

        // Fill the first row (except the first cell) with the remaining random values.
        for (int i = 1; i < 9; i++)
        {
            value = colList[Random.Range(0, colList.Count)];
            if (i < 3)
            {
                // Ensure the 3x3 square does not contain duplicate values.
                while (SquareContainsValue(0, 0, value))
                {
                    value = colList[Random.Range(0, colList.Count)]; // reroll if duplicate found
                }
            }
            grid[0, i] = value;
            colList.Remove(value);
        }

        // Place random values in the diagonal cells of the remaining squares.
        for (int i = 6; i < 9; i++)
        {
            value = Random.Range(1, 10);
            while (SquareContainsValue(0, 8, value) || SquareContainsValue(8, 0, value) || SquareContainsValue(8, 8, value))
            {
                value = Random.Range(1, 10);
            }
            grid[i, i] = value;
        }

        // Output the current grid state for debugging.
        ConsoleOutputGrid(grid);

        // Solve the Sudoku to complete the grid.
        SolveSudoku();
    }

    /// <summary>
    /// Solves the Sudoku grid using a backtracking algorithm.
    /// </summary>
    /// <returns>True if the grid is solved, otherwise false.</returns>
    private bool SolveSudoku()
    {
        int row = -1;
        int col = -1;

        // Find the first empty cell; if none, puzzle is solved
        bool foundEmpty = false;
        for (int i = 0; i < 9 && !foundEmpty; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (grid[i, j] == 0)
                {
                    row = i;
                    col = j;
                    foundEmpty = true;
                    break;
                }
            }
        }
        if (!foundEmpty)
        {
            return true;
        }

        // Try numbers 1 through 9 in the empty cell.
        for (int candidate = 1; candidate <= 9; candidate++)
        {
            if (CheckAll(row, col, candidate))
            {
                grid[row, col] = candidate;
                // Recursively attempt to solve the rest of the grid.
                if (SolveSudoku())
                {
                    return true;
                }
                // Reset the cell if the current number doesn't lead to a solution.
                grid[row, col] = 0;
            }
        }
        return false;
    }

    /// <summary>
    /// Generates the puzzle by copying the solved grid and removing a number of cells based on difficulty.
    /// </summary>
    private void CreatePuzzle()
    {
        // Copy the solved grid to the puzzle.
        System.Array.Copy(grid, puzzle, grid.Length);

        // Remove cells randomly based on difficulty.
        for (int i = 0; i < difficulty; i++)
        {
            int row = Random.Range(0, 9);
            int col = Random.Range(0, 9);

            // Ensure we only remove cells that have not been removed yet.
            while (puzzle[row, col] == 0)
            {
                row = Random.Range(0, 9);
                col = Random.Range(0, 9);
            }
            puzzle[row, col] = 0;
        }

        // Ensure the puzzle has at least 8 different numbers on the board.
        // This loop adjusts the puzzle to help guarantee a unique solution.
        List<int> onBoard = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        RandomizeList(onBoard);

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                // Remove found numbers from the list.
                for (int k = 0; k < onBoard.Count - 1; k++)
                {
                    if (onBoard[k] == puzzle[i, j])
                    {
                        onBoard.RemoveAt(k);
                    }
                }
            }
        }

        // Reinstate at least one occurrence of remaining numbers if necessary.
        while (onBoard.Count - 1 > 1)
        {
            int row = Random.Range(0, 9);
            int col = Random.Range(0, 9);

            if (grid[row, col] == onBoard[0])
            {
                puzzle[row, col] = grid[row, col];
                onBoard.RemoveAt(0);
            }
        }

        // Output the generated puzzle for debugging.
        ConsoleOutputGrid(puzzle);
    }

    /// <summary>
    /// Randomizes the order of elements in a list.
    /// </summary>
    /// <param name="l">List of integers to randomize.</param>
    private void RandomizeList(List<int> l)
    {
        for (var i = 0; i < l.Count - 1; i++)
        {
            int rand = Random.Range(i, l.Count);
            int temp = l[i];
            l[i] = l[rand];
            l[rand] = temp;
        }
    }

    /// <summary>
    /// Creates the Sudoku cell buttons and assigns them to the appropriate 3x3 square parent transforms.
    /// </summary>
    private void CreateButtons()
    {
        // Check if the SudokuCell_Prefab is assigned.
        if (SudokuCell_Prefab == null)
        {
            Debug.LogError("SudokuCell_Prefab is not assigned in the Inspector.");
            return;
        }

        // Loop through the grid positions to instantiate buttons.
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                GameObject newButton = Instantiate(SudokuCell_Prefab);
                SudokuCell sudokuCell = newButton.GetComponent<SudokuCell>();

                // Initialize the cell with its position, value from the puzzle, and a string id.
                sudokuCell.SetValues(i, j, puzzle[i, j], i + "," + j, this);

                // Cache the reference for O(1) access later
                cells[i, j] = sudokuCell;

                // Name the GameObject based on its grid position.
                newButton.name = i.ToString() + j.ToString();

                // Set the parent transform based on which 3x3 block the cell belongs to.
                if (i < 3)
                {
                    if (j < 3)
                    {
                        newButton.transform.SetParent(square00, false);
                    }
                    else if (j > 2 && j < 6)
                    {
                        newButton.transform.SetParent(square01, false);
                    }
                    else if (j >= 6)
                    {
                        newButton.transform.SetParent(square02, false);
                    }
                }
                else if (i >= 3 && i < 6)
                {
                    if (j < 3)
                    {
                        newButton.transform.SetParent(square10, false);
                    }
                    else if (j > 2 && j < 6)
                    {
                        newButton.transform.SetParent(square11, false);
                    }
                    else if (j >= 6)
                    {
                        newButton.transform.SetParent(square12, false);
                    }
                }
                else if (i >= 6)
                {
                    if (j < 3)
                    {
                        newButton.transform.SetParent(square20, false);
                    }
                    else if (j > 2 && j < 6)
                    {
                        newButton.transform.SetParent(square21, false);
                    }
                    else if (j >= 6)
                    {
                        newButton.transform.SetParent(square22, false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Updates the puzzle grid with the new value at the specified cell.
    /// </summary>
    public void UpdatePuzzle(int row, int col, int value)
    {
        puzzle[row, col] = value;
    }

    /// <summary>
    /// Checks if the puzzle is completed correctly.
    /// If the puzzle matches the solved grid, the win menu is activated.
    /// Otherwise, the lose text is displayed.
    /// </summary>
    public void CheckComplete()
    {
        // Highlight incorrect cells using cached references
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                SudokuCell cell = cells[i, j];
                if (cell != null)
                {
                    bool isError = (puzzle[i, j] != 0 && puzzle[i, j] != grid[i, j]);
                    cell.SetErrorState(isError);
                }
            }
        }
        if (CheckGrid())
        {
            winMenu.SetActive(true);
        }
        else
        {
            if (loseText != null)
            {
                loseText.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Lose text GameObject is not assigned in the Inspector.");
            }
        }
    }

    /// <summary>
    /// Compares the puzzle grid with the solved grid.
    /// </summary>
    /// <returns>True if both grids match, otherwise false.</returns>
    private bool CheckGrid()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (puzzle[i, j] != grid[i, j])
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Fills a correct value in a random empty cell as a hint.
    /// </summary>
    public void GiveHint()
    {
        // Find all empty cells
        List<(int, int)> emptyCells = new List<(int, int)>();
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (puzzle[i, j] == 0)
                {
                    emptyCells.Add((i, j));
                }
            }
        }
        if (emptyCells.Count == 0)
            return; // No empty cells left

        // Pick a random empty cell and fill with the correct value
        var (row, col) = emptyCells[Random.Range(0, emptyCells.Count)];
        int correctValue = grid[row, col];
        SudokuCell targetCell = cells[row, col];
        if (targetCell != null)
        {
            targetCell.UpdateValue(correctValue);
        }
    }
}
