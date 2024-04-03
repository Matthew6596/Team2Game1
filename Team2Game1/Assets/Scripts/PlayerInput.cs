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
    Renderer prevHit2;

    TileScript tileScript;

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
        if(prevHit!=null) prevHit.material.SetFloat("_Scale", 0f); //reset
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
                if (IsInteractable(hitObj)&&!CardManager.CardMoving&&TurnSystem.PlayerTurnsLeft>0) //If hit obj is interactable
                {
                    InteractWithItem(hitObj);
                }
            }
        }
    }

    //Important: interactable object must be on Interactable layer, and also have a tag
    void InteractWithItem(GameObject hitObj)
    {
        //If a card is being examined, clicking something will unexamine the card
        if (CardManager.cardBeingExamined)
        {
            CardManager.ExamineCard();
            return;
        }

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
                if (CardManager.PlayerHand.Contains(hitObj) || CardManager.BoardCards.Contains(hitObj))
                    CardManager.SelectedCard = hitObj;
            }
            //Player has either board/hand card selected
            else if (CardManager.BoardCards.Contains(CardManager.SelectedCard) || CardManager.PlayerHand.Contains(CardManager.SelectedCard))
                CardClicked(hitObj);
            else if (CardManager.DeckCards.Contains(hitObj))
                CardManager.DrawFromDeck();

        }
        else if (hitObj.CompareTag("PlayerTile") || hitObj.CompareTag("EnemyTile")) 
        {
            tileScript = hitObj.GetComponent<TileScript>();
            if(tileScript.occupied == true)
            {
                string tileName = hitObj.name;
                Debug.Log(tileName);
            }
            else
            {
                if (CardManager.SelectedCard != null && CardManager.PlayerHand.Contains(CardManager.SelectedCard))
                {
                    CardManager.PlayerHand.Remove(CardManager.SelectedCard);
                    CardManager.BoardCards.Add(CardManager.SelectedCard); //this is VERY not correct and is causing errors
                    TurnSystem.PlayerTurnsLeft--;
                    hitObj.GetComponent<TileScript>().card = CardManager.SelectedCard;
                    CardManager.MoveCard(CardManager.SelectedCard, hitObj.transform.position + Vector3.up * 0.3f+Vector3.back*0.8f, CardManager.cameraPos);
                    CardManager.SelectedCard = null;
                    //CardManager.BoardCards[tileNum].transform.position = hitObj.transform.position;
                    hitObj.GetComponent<TileScript>().occupied = true;
                }

                //- I don't think you need this part
                //- I don't think you need this part
                //find which tile it is
                string tileName = hitObj.name;
                Debug.Log(tileName);
                int tileNum = 0;
                switch (tileName)
                {
                    case "0":
                        tileNum = 0;
                        break;
                    case "1":
                        tileNum = 1;
                        break;
                    case "2":
                        tileNum = 2;
                        break;
                    case "3":
                        tileNum = 3;
                        break;
                    case "4":
                        tileNum = 4;
                        break;
                    case "5":
                        tileNum = 5;
                        break;
                    case "6":
                        tileNum = 6;
                        break;
                    case "7":
                        tileNum = 7;
                        break;
                    case "8":
                        tileNum = 8;
                        break;
                    case "9":
                        tileNum = 9;
                        break;
                }
            }
        }
        else
        {
            Debug.LogWarning("Object on interactable layer, but has no special tag!: " + hitObj.name);
        }
    }

    void CardClicked(GameObject hitObj)
    {
        //If selected card and clicked card are on the board, hug
        if (CardManager.BoardCards.Contains(CardManager.SelectedCard) && CardManager.BoardCards.Contains(hitObj))
        {
            CardManager.SelectedCard.GetComponent<CardScript>().Hug(hitObj.GetComponent<CardScript>());
        }
        //If clicked card is in player hand...
        else if (CardManager.PlayerHand.Contains(hitObj))
        {
            //...and is the same as selected card, examine
            if (CardManager.SelectedCard == hitObj)
                CardManager.ExamineCard();

            //if card is not the same as selected card, select it
            else
                CardManager.SelectedCard = hitObj;
        }
        //If clicked card is on the board, and is the same as selected card, deselect
        else if (CardManager.SelectedCard == hitObj)
            CardManager.SelectedCard = null;
        else
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
                prevHit = hitObj.transform.GetChild(0).GetComponent<Renderer>();
                prevHit.material.SetFloat("_Scale", 1.05f); //Show glow
            }
        }
        if(prevHit2!=null) prevHit2.material.SetFloat("_Scale", 0f);
        if (CardManager.SelectedCard != null)
        {
            prevHit2 = CardManager.SelectedCard.transform.GetChild(0).GetComponent<Renderer>();
            prevHit2.material.SetFloat("_Scale", 1.05f); //Show glow
        }
    }
}
