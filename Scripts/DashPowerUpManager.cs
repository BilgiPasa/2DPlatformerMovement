using System.Collections;
using UnityEngine;

public class DashPowerUpManager : MonoBehaviour
{
    //* Attach this script to the Player game object.
    //* In the editor, add a tag named "DashPowerUp" and assign it to the Dash Power Up object.

    [SerializeField] GameObject dashPowerUpObject, dashTextObject;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DashPowerUp"))
        {
            PlayerManager.canDash = true;
            Destroy(dashPowerUpObject);
            StartCoroutine(DashTextManager());
        }
    }

    IEnumerator DashTextManager()
    {
        dashTextObject.SetActive(true);
        yield return new WaitForSeconds(2);
        dashTextObject.SetActive(false);
    }
}
