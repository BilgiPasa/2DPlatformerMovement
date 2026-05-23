using System.Collections;
using UnityEngine;

public class DoubleJumpPowerUpManager : MonoBehaviour
{
    //* Attach this script to the Player game object.
    //* In the editor, add a tag named "DoubleJumpPowerUp" and assign it to the DoubleJumpPowerUp object.

    [SerializeField] GameObject doubleJumpPowerUpObject, doubleJumpTextObject;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DoubleJumpPowerUp"))
        {
            PlayerManager.canDoubleJump = true;
            Destroy(doubleJumpPowerUpObject);
            StartCoroutine(DoubleJumpTextManager());
        }
    }

    IEnumerator DoubleJumpTextManager()
    {
        doubleJumpTextObject.SetActive(true);
        yield return new WaitForSeconds(2);
        doubleJumpTextObject.SetActive(false);
    }
}
