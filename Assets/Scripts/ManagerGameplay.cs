using UnityEngine;

public class ManagerGameplay : MonoBehaviour
{
    private Settings S;

    private Color colorPreviewPlayer1;
    private Color colorPreviewPlayer2;
    public Camera myCamera;

    public GameObject previewPiece;
    public Sprite spritePiece1;
    public Sprite spritePiece2;
    private SpriteRenderer rendererPreviewPiece;
    public GameObject prefabPiece1;
    public GameObject prefabPiece2;
    public Transform parentPieces;

    public int boardSize;
    private bool isPlaying;
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
        S = Settings.S;

        rendererPreviewPiece = previewPiece.GetComponent<SpriteRenderer>();
        colorPreviewPlayer1 = new Color(S.colorPlayer1.r, S.colorPlayer1.g, S.colorPlayer1.b, 0.6f);
        colorPreviewPlayer2 = new Color(S.colorPlayer2.r, S.colorPlayer2.g, S.colorPlayer2.b, 0.6f);
        ButtonRestart();
    }

    private void Update()
    {
        if (!isPlaying) return;

        float x = myCamera.ScreenToWorldPoint(Input.mousePosition).x;
        float y = myCamera.ScreenToWorldPoint(Input.mousePosition).y;
        Vector2Int currentCell = new Vector2Int(Mathf.RoundToInt(x), Mathf.RoundToInt(y));

        previewPiece.SetActive(false);

        if (currentCell.x >= 0 && currentCell.x < boardSize && currentCell.y >= 0 && currentCell.y < boardSize)
        {
            if (board[currentCell.x, currentCell.y] == PieceState.EMPTY) previewPiece.SetActive(true);
            previewPiece.transform.position = new Vector2(currentCell.x, currentCell.y);
            rendererPreviewPiece.sprite = player2Turn ? spritePiece2 : spritePiece1;
            rendererPreviewPiece.color = player2Turn ? colorPreviewPlayer2 : colorPreviewPlayer1;

            if (Input.GetMouseButtonDown(0))
                Play(previewPiece.transform.position);
        }
    }

    public void ButtonRestart()
    {
        board = new PieceState[boardSize, boardSize];

        for (int i = 0; i < parentPieces.childCount; i++)
            Destroy(parentPieces.GetChild(i).gameObject);

        player2Turn = false;
        ManagerUI.MUI.PlayerTurn(false);
        ManagerUI.MUI.Restart();
        isPlaying = true;
    }

    private void Play(Vector2 _pos)
    {
        int toPlayX = (int)_pos.x;
        int toPlayY = (int)_pos.y;

        if (board[toPlayX, toPlayY] != PieceState.EMPTY) return;

        SpriteRenderer sr = Instantiate(player2Turn ? prefabPiece2 : prefabPiece1, _pos, Quaternion.identity, parentPieces).GetComponent<SpriteRenderer>();
        sr.color = player2Turn ? S.colorPlayer2 : S.colorPlayer1;
        board[toPlayX, toPlayY] = player2Turn ? PieceState.PLAYER2 : PieceState.PLAYER1;

        CheckWin(new Vector2Int(toPlayX, toPlayY));
    }

    private void CheckWin(Vector2Int _playedCell)
    {
        if (CalculateLineLength(_playedCell) >= 3)
        {
            ManagerUI.MUI.Endgame(player2Turn ? 2 : 1);
            isPlaying = false;
            return;
        }

        if (IsDraw())
        {
            ManagerUI.MUI.Endgame(0);
            isPlaying = false;
        }
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

        //Horizontal
        currentCell = _playedCell;
        lineLength = 1;

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

        //Diagonal Up
        currentCell = _playedCell;
        lineLength = 1;

        while (currentCell.x + 1 < boardSize && currentCell.y + 1 < boardSize)
        {
            currentCell.x++;
            currentCell.y++;
            if (board[currentCell.x, currentCell.y] == (player2Turn ? PieceState.PLAYER2 : PieceState.PLAYER1)) lineLength++;
        }

        currentCell = _playedCell;

        while (currentCell.x - 1 >= 0 && currentCell.y - 1 >= 0)
        {
            currentCell.x--;
            currentCell.y--;
            if (board[currentCell.x, currentCell.y] == (player2Turn ? PieceState.PLAYER2 : PieceState.PLAYER1)) lineLength++;
        }

        maxLineLenght = Mathf.Max(maxLineLenght, lineLength);

        //Diagonal Down
        currentCell = _playedCell;
        lineLength = 1;

        while (currentCell.x + 1 < boardSize && currentCell.y - 1 >= 0)
        {
            currentCell.x++;
            currentCell.y--;
            if (board[currentCell.x, currentCell.y] == (player2Turn ? PieceState.PLAYER2 : PieceState.PLAYER1)) lineLength++;
        }

        currentCell = _playedCell;

        while (currentCell.x - 1 >= 0 && currentCell.y + 1 < boardSize)
        {
            currentCell.x--;
            currentCell.y++;
            if (board[currentCell.x, currentCell.y] == (player2Turn ? PieceState.PLAYER2 : PieceState.PLAYER1)) lineLength++;
        }

        maxLineLenght = Mathf.Max(maxLineLenght, lineLength);

        return maxLineLenght;
    }
}
