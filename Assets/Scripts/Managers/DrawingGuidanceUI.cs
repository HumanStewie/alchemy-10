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
    [SerializeField] private float leftHiddenX = -1200f;
    [SerializeField] private float rightHiddenX = 1200f;
    [SerializeField] private float leftShownX = -550f;
    [SerializeField] private float rightShownX = 550f;

    [Header("Transition Settings")]
    [SerializeField] private float transitionDuration = 0.25f;
    [SerializeField] private Ease showEase = Ease.OutBack;
    [SerializeField] private Ease hideEase = Ease.InQuad;

    [Header("Video Players")]
    [SerializeField] private VideoPlayer leftVideoPlayer;
    [SerializeField] private VideoPlayer rightVideoPlayer;

    [Header("Playlist Clips (7 Videos)")]
    [SerializeField] private VideoClip[] leftClips;
    [SerializeField] private VideoClip[] rightClips;

    private int currentLeftIndex = 0;
    private int currentRightIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (leftPanel != null)
            leftPanel.anchoredPosition = new Vector2(leftHiddenX, leftPanel.anchoredPosition.y);

        if (rightPanel != null)
            rightPanel.anchoredPosition = new Vector2(rightHiddenX, rightPanel.anchoredPosition.y);

        SetupPlayer(leftVideoPlayer, OnLeftVideoEnded);
        SetupPlayer(rightVideoPlayer, OnRightVideoEnded);
    }

    private void SetupPlayer(VideoPlayer vp, VideoPlayer.EventHandler onEnded)
    {
        if (vp == null) return;
        vp.isLooping = false; // Tắt lặp đơn lẻ để chuyển sang video tiếp theo trong playlist
        vp.playOnAwake = false;
        vp.loopPointReached += onEnded;
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

        PlayClip(leftVideoPlayer, leftClips, currentLeftIndex);
        PlayClip(rightVideoPlayer, rightClips, currentRightIndex);
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

        if (leftVideoPlayer != null && leftVideoPlayer.isPlaying) leftVideoPlayer.Pause();
        if (rightVideoPlayer != null && rightVideoPlayer.isPlaying) rightVideoPlayer.Pause();
    }

    private void PlayClip(VideoPlayer vp, VideoClip[] clips, int index)
    {
        if (vp == null || clips == null || clips.Length == 0) return;

        vp.clip = clips[index % clips.Length];
        vp.time = 0;
        vp.Play();
    }

    private void OnLeftVideoEnded(VideoPlayer source)
    {
        if (leftClips == null || leftClips.Length == 0) return;
        currentLeftIndex = (currentLeftIndex + 1) % leftClips.Length;
        PlayClip(source, leftClips, currentLeftIndex);
    }

    private void OnRightVideoEnded(VideoPlayer source)
    {
        if (rightClips == null || rightClips.Length == 0) return;
        currentRightIndex = (currentRightIndex + 1) % rightClips.Length;
        PlayClip(source, rightClips, currentRightIndex);
    }

    private void OnDestroy()
    {
        if (leftVideoPlayer != null) leftVideoPlayer.loopPointReached -= OnLeftVideoEnded;
        if (rightVideoPlayer != null) rightVideoPlayer.loopPointReached -= OnRightVideoEnded;
    }
}