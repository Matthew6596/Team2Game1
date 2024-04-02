using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public enum RotationAxis
    {
        MouseXY = 0, MouseX = 1, MouseY = 2
    }
    public RotationAxis axis = RotationAxis.MouseXY;
    public float sensitivityX = 10f, sentitivityY = 10f;
    public float minVertical = -80f, maxVertical = 80f;

    float mouseX, mouseY, verticalRotation = 0;

    Renderer prevHit;

    static bool IsInteractable(GameObject hitObj) {
        return hitObj.layer == LayerMask.NameToLayer("Interactable");
    }

    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Camera Look
        switch (axis)
        {
            case RotationAxis.MouseXY: //Both XY
                verticalRotation -= mouseY * sentitivityY * Time.deltaTime;
                verticalRotation = Mathf.Clamp(verticalRotation, minVertical, maxVertical);

                float deltaRotation = mouseX * sensitivityX * Time.deltaTime;
                float hozRotation = transform.localEulerAngles.y + deltaRotation;

                transform.localEulerAngles = new Vector3(verticalRotation, hozRotation, 0);
                break;
            case RotationAxis.MouseX: //Only X
                transform.Rotate(0, mouseX * sensitivityX * Time.deltaTime, 0);
                break;
            case RotationAxis.MouseY: //Only Y
                verticalRotation -= mouseY * sentitivityY * Time.deltaTime;
                verticalRotation = Mathf.Clamp(verticalRotation, minVertical, maxVertical);

                transform.localEulerAngles = new Vector3(verticalRotation, transform.localEulerAngles.y, 0);
                break;
        }

        //Glow outline / look feedback
        if(prevHit!=null) prevHit.materials[1].SetFloat("_Scale", 0.01f); //reset
        DoOutlineGlow();
        
    }

    public void PlayerLook(InputAction.CallbackContext ctx)
    {
        //Debug.Log(ctx.ReadValue<Vector2>());
        mouseX = ctx.ReadValue<Vector2>().x;
        mouseY = ctx.ReadValue<Vector2>().y;
    }

    public void PlayerClick(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                GameObject hitObj = hit.collider.gameObject;
                if (IsInteractable(hitObj)) //If hit obj is interactable
                {
                    InteractWithItem(hitObj);
                }
            }
        }
    }

    //Important: interactable object must be on Interactable layer, and also have a tag
    void InteractWithItem(GameObject hitObj)
    {
        if (hitObj.CompareTag("Deck"))
        {
            CardManager.DrawFromDeck();
        }
        else if (hitObj.CompareTag("Card"))
        {
            if(hitObj.GetComponent<CardScript>()==null)
                Debug.LogWarning("Object has Card tag, but no CardScript!: " + hitObj.name);

            //Player has no card currently selected
            if (CardManager.SelectedCard == null)
            {
                //Select the clicked card if valid
                if(CardManager.PlayerHand.Contains(hitObj)||CardManager.BoardCards.Contains(hitObj))
                    CardManager.SelectedCard = hitObj;
            }
            //Player has either board/hand card selected
            else if(CardManager.BoardCards.Contains(CardManager.SelectedCard)|| CardManager.PlayerHand.Contains(CardManager.SelectedCard))
                CardClicked(hitObj);

        }
        else if (hitObj.CompareTag("")) {

        }
        else
        {
            Debug.LogWarning("Object on interactable layer, but has no special tag!: " + hitObj.name);
        }
    }

    void CardClicked(GameObject hitObj)
    {
        //If selected card and clicked card are on the board, hug
        if (CardManager.BoardCards.Contains(CardManager.SelectedCard)&& CardManager.BoardCards.Contains(hitObj))
        {
            CardManager.SelectedCard.GetComponent<CardScript>().Hug(hitObj.GetComponent<CardScript>());
        }
        //If player clicked the same card, deselct it
        else if (CardManager.SelectedCard == hitObj)
            CardManager.SelectedCard = null;
        //If clicked card is in player hand, select it
        else if (CardManager.PlayerHand.Contains(hitObj))
            CardManager.SelectedCard = hitObj;
    }

    void DoOutlineGlow()
    {
        prevHit = null;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (IsInteractable(hitObj)) //If hit obj is interactable
            {
                prevHit = hitObj.GetComponent<Renderer>();
                prevHit.materials[1].SetFloat("_Scale", 1.05f); //Show glow
            }
        }
    }
}
