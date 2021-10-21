using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerControllerBase
{
    public bool canLook = true, dialogInput, cinemaInput;
    [SerializeField]
    protected float interactRadius = 1.5f, interactDistance = 2.0f;
    protected Vector3 mousePosition;
    protected CharacterPlayer player;
    public override void Possess(Pawn pawn)
    {
        base.Possess(pawn);
        player = controlledPawn as CharacterPlayer;
    }
    public override void Unpossess()
    {
        player = null;
        base.Unpossess();
    }
    public Vector3 GetMousePosition()
    {
        return mousePosition;
    }
    public CharacterPlayer Player => player;
    protected virtual void GameControll()
    {
        mousePosition = GameInstance.Instance.Camera.Instance.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        if (Input.GetKeyDown(KeyCode.Alpha9))
            GameInstance.Instance.InitiateSaveGame();
        if (Input.GetKeyDown(KeyCode.Alpha0))
            GameInstance.Instance.InitiateLoadGame();
        #region Interact
        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector3 direction = (mousePosition - controlledPawn.transform.position).normalized;
           var cast = Physics2D.BoxCast(controlledPawn.transform.position, Vector2.one * interactRadius, 0, direction, interactDistance, LayerMask.GetMask("Interact") | LayerMask.GetMask("Pawn"));

            //Collider2D collider = Physics2D.OverlapCircle(controlledPawn.transform.position, interactRadius, LayerMask.GetMask("Interact"));
            //if (collider != null)
            if (cast.collider != null)
            {
                //IInteractiveExecutor iia = collider.GetComponent<IInteractiveExecutor>();
                IInteractiveExecutor iia = cast.collider.GetComponent<IInteractiveExecutor>();
                if (iia != null)
                {
                    iia.Execute();
                }
            }
        }
        #endregion
        #region Look
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 move = new Vector2(h, v);
        if (move != Vector2.zero)
            player.MoveInput(move);
        if (canLook)
        {
            Vector3 direction = (mousePosition - player.InputRotationComponent.Arms.position).normalized;
            player.InputRotationComponent.SetRotation(direction);
        }
        #endregion
        #region Weapon Left
        if (Input.GetMouseButtonDown(0))
            player.WeaponHolderComponent.SetFire(true);
        else
        {
            if (Input.GetMouseButton(0))
                player.WeaponHolderComponent.Shoot(player.WeaponHolderComponent.GetShootPoint(), player.CharacterOrientation.Orientation);
            else
            {
                if (Input.GetMouseButtonUp(0))
                    player.WeaponHolderComponent.SetFire(false);
            }
        }
        #endregion
        #region Weapon Right
        if (Input.GetMouseButtonDown(1))
            player.WeaponHolderComponent.SetFireDominator(true);
        else
        {
            if (Input.GetMouseButton(1))
                player.WeaponHolderComponent.ShootDominator(mousePosition, Vector3.right);
            else
            {
                if (Input.GetMouseButtonUp(1))
                    player.WeaponHolderComponent.SetFireDominator(false);
            }
        }
        #endregion
        #region Weapon Slots
        if (Input.GetKeyDown(KeyCode.Alpha1))
            player.WeaponHolderComponent.ChangeSlot(0);
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha2))
                player.WeaponHolderComponent.ChangeSlot(1);
            else
            {
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    player.WeaponHolderComponent.ChangeSlot(2);
                else
                {
                    if (Input.GetKeyDown(KeyCode.Alpha4))
                        player.WeaponHolderComponent.ChangeSlot(3);
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.Alpha5))
                            player.WeaponHolderComponent.ChangeSlot(-1);
                    }
                }

            }
        }
        #endregion
    }
    protected virtual void DialogControll()
    {
        var type = GameInstance.Instance.Dialog.ElementType;
        switch (type)
        {
            case DialogSystem.XmlDialogElementType.phrase:
                if (Input.GetMouseButtonDown(0) ||
                    Input.GetKeyDown(KeyCode.Return) ||
                    Input.GetKeyDown(KeyCode.F) ||
                    Input.GetKeyDown(KeyCode.Space))
                    GameInstance.Instance.Dialog.Skip(!cinemaInput);
                break;
            case DialogSystem.XmlDialogElementType.option:
                if (Input.GetKeyDown(KeyCode.Return) ||
                    Input.GetMouseButtonDown(2) ||
                     Input.GetKeyDown(KeyCode.F) ||
                    Input.GetKeyDown(KeyCode.Space))
                    GameInstance.Instance.Dialog.Enter();
                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                    GameInstance.Instance.Dialog.Select(-1);
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    GameInstance.Instance.Dialog.Select(1);
                break;
        }
    }
    protected virtual void CinemaControll()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            GameInstance.Instance.Cinema.Skip();
    }
    protected virtual void Update()
    {
        #region Cinema Controll
        cinemaInput = GameInstance.Instance.Cinema.OverrideInput;
        if (cinemaInput)
        {
            CinemaControll();
        }
        #endregion 
        #region Dialog Controll
        dialogInput = GameInstance.Instance.Dialog.OverrideInput;
        if (dialogInput)
        {
            DialogControll();
        }
        #endregion;
        #region Game Controll
        if (!dialogInput && !cinemaInput)
        {
            
            if (gameInput)
            {
                GameControll();
            }

        }
        #endregion
    }
    protected virtual void LateUpdate()
    {
        if (!dialogInput && !cinemaInput && gameInput)
            GameInstance.Instance.Camera.Movement.Follow(player.transform);
    }
    public override void SetStart(Vector3 position, Quaternion rotation)
    {
        controlledPawn.transform.position = position;
        controlledPawn.transform.rotation = rotation;
    }
    public override JSONObject Save( JSONObject jsonObject)
    {
        base.Save( jsonObject);
        jsonObject.Add("gameInput", new JSONBool(gameInput));
        jsonObject.Add("dialogInput", new JSONBool(dialogInput));
        jsonObject.Add("cinemaInput", new JSONBool(cinemaInput));
        jsonObject.Add("canLook", new JSONBool(canLook));
        return jsonObject;
    }
    public override JSONObject Load( JSONObject jsonObject)
    {
        base.Load( jsonObject);
        gameInput = jsonObject["gameInput"].AsBool;
        dialogInput = jsonObject["dialogInput"].AsBool;
        cinemaInput = jsonObject["cinemaInput"].AsBool;
        canLook = jsonObject["canLook"].AsBool;
        return jsonObject;
    }
}
