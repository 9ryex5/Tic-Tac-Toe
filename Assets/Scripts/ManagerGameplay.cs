using UnityEngine;

public class ManagerGameplay : MonoBehaviour
{
    public Color colorPlayer1;
    public Color colorPlayer2;
    private Color colorPreviewPlayer1;
    private Color colorPreviewPlayer2;
    public Camera myCamera;

    public GameObject previewPiece;
    private SpriteRenderer rendererPreviewPiece;
    public GameObject prefabPiece;
    public Transform parentPieces;

    public int boardSize;
    private PieceState[,] board;
    private bool player2Turn;

    private enum PieceState
    {
        EMPTY,
        PLAYER1,
        PLAYER2
    }

    private void Start()
    {
        rendererPreviewPiece = previewPiece.GetComponent<SpriteRenderer>();
        colorPreviewPlayer1 = new Color(colorPlayer1.r, colorPlayer1.g, colorPlayer1.b, 0.6f);
        colorPreviewPlayer2 = new Color(colorPlayer2.r, colorPlayer2.g, colorPlayer2.b, 0.6f);
        Restart();
    }

    private void Update()
    {
        float x = myCamera.ScreenToWorldPoint(Input.mousePosition).x;
        float y = myCamera.ScreenToWorldPoint(Input.mousePosition).y;
        Vector2Int currentCell = new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));

        previewPiece.SetActive(false);

        if (currentCell.x >= 0 && currentCell.x < boardSize && currentCell.y >= 0 && currentCell.y < boardSize)
        {
            if (board[currentCell.x, currentCell.y] == PieceState.EMPTY) previewPiece.SetActive(true);
            previewPiece.transform.position = new Vector2(currentCell.x, currentCell.y);
            rendererPreviewPiece.color = player2Turn ? colorPreviewPlayer2 : colorPreviewPlayer1;

            if (Input.GetMouseButtonDown(0))
                Play(previewPiece.transform.position);
        }
    }

    private void Restart()
    {
        board = new PieceState[boardSize, boardSize];

        for (int i = 0; i < parentPieces.childCount; i++)
            Destroy(parentPieces.GetChild(i).gameObject);

        player2Turn = false;
        ManagerUI.MUI.PlayerTurn(false);
    }

    private void Play(Vector2 _pos)
    {
        int toPlayX = (int)_pos.x;
        int toPlayY = (int)_pos.y;

        if (board[toPlayX, toPlayY] != PieceState.EMPTY) return;

        SpriteRenderer sr = Instantiate(prefabPiece, _pos, Quaternion.identity, parentPieces).GetComponent<SpriteRenderer>();
        sr.color = player2Turn ? colorPlayer2 : colorPlayer1;
        board[toPlayX, toPlayY] = player2Turn ? PieceState.PLAYER2 : PieceState.PLAYER1;

        CheckWin(new Vector2Int(toPlayX, toPlayY));
    }

    private void CheckWin(Vector2Int _playedCell)
    {
        if (CalculateLineLength(_playedCell) >= 3)
        {
            Debug.Log(player2Turn ? "Player 2 Win" : "Player 1 Win");
            return;
        }

        if (IsDraw())
            Restart();
        else
            NextTurn();
    }

    private bool IsDraw()
    {
        for (int i = 0; i < boardSize; i++)
            for (int j = 0; j < boardSize; j++)
                if (board[i, j] == PieceState.EMPTY)
                    return false;

        return true;
    }

    private void NextTurn()
    {
        player2Turn = !player2Turn;
        ManagerUI.MUI.PlayerTurn(player2Turn);
    }

    private int CalculateLineLength(Vector2Int _playedCell)
    {
        int lineLength = 1;
        Vector2Int currentCell = _playedCell;
        int maxLineLenght = 0;

        //Vertical
        while (currentCell.y + 1 < boardSize)
        {
            currentCell.y++;
            if (board[currentCell.x, currentCell.y] == (player2Turn ? PieceState.PLAYER2 : PieceState.PLAYER1)) lineLength++;
        }

        currentCell = _playedCell;

        while (currentCell.y - 1 >= 0)
        {
            currentCell.y--;
            if (board[currentCell.x, currentCell.y] == (player2Turn ? PieceState.PLAYER2 : PieceState.PLAYER1)) lineLength++;
        }

        maxLineLenght = Mathf.Max(maxLineLenght, lineLength);
        currentCell = _playedCell;
        lineLength = 1;

        //Horizontal
        while (currentCell.x + 1 < boardSize)
        {
            currentCell.x++;
            if (board[currentCell.x, currentCell.y] == (player2Turn ? PieceState.PLAYER2 : PieceState.PLAYER1)) lineLength++;
        }

        currentCell = _playedCell;

        while (currentCell.x - 1 >= 0)
        {
            currentCell.x--;
            if (board[currentCell.x, currentCell.y] == (player2Turn ? PieceState.PLAYER2 : PieceState.PLAYER1)) lineLength++;
        }

        maxLineLenght = Mathf.Max(maxLineLenght, lineLength);
        lineLength = 1;
        currentCell = _playedCell;
        return maxLineLenght;
    }
}
