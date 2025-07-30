using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookUIController : MonoBehaviour
{
    public void TurnRight() => GameManager.manager.TurnRight();
    public void TurnLeft() => GameManager.manager.TurnLeft();
    public void DisableBook()
    {
        gameObject.SetActive(false);
        GameManager.manager.isBookOpen = false;
        GameManager.manager.text.gameObject.SetActive(true);
        GameManager.manager.gameHints.SetActive(true);
        GameManager.manager.bookHints.SetActive(false);
    }
}
