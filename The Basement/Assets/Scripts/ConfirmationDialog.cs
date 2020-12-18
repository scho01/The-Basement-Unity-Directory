using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationDialog : MonoBehaviour
{
    private static ConfirmationDialog instance;
    private System.Action storedActionOnConfirm;
    public Text dialogText;
    public static void Show(string dialogMessage, System.Action actionOnConfirm)
    {
        instance.storedActionOnConfirm = actionOnConfirm;
        instance.dialogText.text = dialogMessage;
        instance.gameObject.SetActive(true);
    }

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Y))
            OnConfirmButton();
        else if (Input.GetKey(KeyCode.N))
            OnCancelButton();
    }

    public void OnConfirmButton()
    {
        if (storedActionOnConfirm != null)
        {
            storedActionOnConfirm();
            storedActionOnConfirm = null;
            gameObject.SetActive(false);
        }
    }

    public void OnCancelButton()
    {
        storedActionOnConfirm = null;
        gameObject.SetActive(false);
    }
}
