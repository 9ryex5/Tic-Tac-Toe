using UnityEngine;

public class Settings : MonoBehaviour
{
    public static Settings S;

    public Color colorPlayer1;
    public Color colorPlayer2;

    private void Awake()
    {
        S = this;
    }
}
