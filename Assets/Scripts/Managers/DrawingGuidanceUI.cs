using DG.Tweening;
using UnityEngine;
using UnityEngine.Video;

public class DrawingGuidanceUI : MonoBehaviour
{
    public static DrawingGuidanceUI Instance;

    [Header("Panels")]
    [SerializeField] private RectTransform leftPanel;
    [SerializeField] private RectTransform rightPanel;

    [Header("Panel Slide Positions (Anchored X)")]
    [SerializeField] private float leftHiddenX = -1400f;
    [SerializeField] private float rightHiddenX = 1400f;
    [SerializeField] private float leftShownX = -600f;
    [SerializeField] private float rightShownX = 600f;

    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 0.25f;
    [SerializeField] private Ease showEase = Ease.OutBack;
    [SerializeField] private Ease hideEase = Ease.InQuad;

    [Header("Video Players (All Play Simultaneously)")]
    [SerializeField] private VideoPlayer[] leftVideoPlayers;  
    [SerializeField] private VideoPlayer[] rightVideoPlayers; 

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Position offscreen on boot
        if (leftPanel != null)
            leftPanel.anchoredPosition = new Vector2(leftHiddenX, leftPanel.anchoredPosition.y);

        if (rightPanel != null)
            rightPanel.anchoredPosition = new Vector2(rightHiddenX, rightPanel.anchoredPosition.y);

        ConfigurePlayers(leftVideoPlayers);
        ConfigurePlayers(rightVideoPlayers);
    }

    private void ConfigurePlayers(VideoPlayer[] players)
    {
        if (players == null) return;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null) continue;
            players[i].isLooping = true;
            players[i].playOnAwake = false;
            players[i].Prepare();
        }
    }

    public void ShowGuidance()
    {
        if (leftPanel != null)
        {
            leftPanel.DOKill();
            leftPanel.DOAnchorPosX(leftShownX, transitionDuration).SetEase(showEase);
        }

        if (rightPanel != null)
        {
            rightPanel.DOKill();
            rightPanel.DOAnchorPosX(rightShownX, transitionDuration).SetEase(showEase);
        }

        PlayAll(leftVideoPlayers);
        PlayAll(rightVideoPlayers);
    }

    public void HideGuidance()
    {
        if (leftPanel != null)
        {
            leftPanel.DOKill();
            leftPanel.DOAnchorPosX(leftHiddenX, transitionDuration).SetEase(hideEase);
        }

        if (rightPanel != null)
        {
            rightPanel.DOKill();
            rightPanel.DOAnchorPosX(rightHiddenX, transitionDuration).SetEase(hideEase);
        }

        PauseAll(leftVideoPlayers);
        PauseAll(rightVideoPlayers);
    }

    private void PlayAll(VideoPlayer[] players)
    {
        if (players == null) return;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null) continue;
            players[i].time = 0;
            players[i].Play();
        }
    }

    private void PauseAll(VideoPlayer[] players)
    {
        if (players == null) return;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null && players[i].isPlaying)
            {
                players[i].Pause();
            }
        }
    }
}