using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeBase : MonoBehaviour
{
    [SerializeField]
    protected GameStateBase gameState;
    [SerializeField]
    protected PlayerControllerBase playerController;
    [SerializeField]
    protected Pawn playerPawn;
    [SerializeField]
    protected Navigation navigation;
    [SerializeField]
    protected UISystem ui;
    [SerializeField]
    protected DialogSystem dialog;
    [SerializeField]
    protected CinemaSystem cinema;
    [SerializeField]
    protected CameraActor mainCamera;
    [SerializeField]
    protected MusicHolder musicHolder;


    public GameStateBase GameState => gameState;
    public PlayerControllerBase PlayerController => playerController;
    public Pawn PlayerPawn => playerPawn;
    public Navigation Navigation => navigation;
    public DialogSystem Dialog => dialog;
    public CinemaSystem Cinema => cinema;
    public UISystem UI => ui;
    public CameraActor MainCamera => mainCamera;
    public MusicHolder MusicHolder => musicHolder;
}
