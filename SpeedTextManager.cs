using UnityEngine;
using TMPro;

public class SpeedTextManager : MonoBehaviour
{
    //* Attach this script to the UserInterface game object.

    [SerializeField] GameObject speedTextObject;
    [SerializeField] Rigidbody2D playerRigidbody;
    [SerializeField] TextMeshProUGUI speedText;

    void Update()
    {
        speedText.text = $"Speed: {Mathf.Abs(playerRigidbody.velocity.x)}";
    }

    public void SpeedTextActivator(bool active)
    {
        speedTextObject.SetActive(active);
    }
}
