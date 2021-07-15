using UnityEngine;
using UnityEngine.UI;

public class ManagerUI : MonoBehaviour
{
    public static ManagerUI MUI;

    public Text textPlayerTurn;

    private void Awake()
    {
        MUI = this;
    }

    public void PlayerTurn(bool _player2)
    {
        textPlayerTurn.text = _player2 ? "Player 2" : "Player 1";
    }
}
