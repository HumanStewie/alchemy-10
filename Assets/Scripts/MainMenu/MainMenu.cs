using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button SettingButton;
    [SerializeField] private Button CreditButton;
    [SerializeField] private Button ExitButton;
    [SerializeField] private Button BackButton;
    [SerializeField] private Button BackButton2;

    [Header("Panels")]
    [SerializeField] private RectTransform MenuPanel;
    [SerializeField] private RectTransform CreditPanel;
    [SerializeField] private RectTransform SettingPanel;
    [SerializeField] private float transitionDuration = 0.4f;

    [Header("Screen Fade Transition")]
    [SerializeField] private Image black;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private string playSceneName = "Play";

    private bool isTransitioning = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Button clicks
        if (ExitButton != null) ExitButton.onClick.AddListener(Exit);
        if (PlayButton != null) PlayButton.onClick.AddListener(OnPlayClicked);
        if (CreditButton != null) CreditButton.onClick.AddListener(CrediPanel);
        if (BackButton != null) BackButton.onClick.AddListener(ChangePanel);
        if (BackButton2 != null) BackButton2.onClick.AddListener(ChangePanel);
        if (SettingButton != null) SettingButton.onClick.AddListener(SettinPanel);

        SetupButtonHover(PlayButton);
        SetupButtonHover(SettingButton);
        SetupButtonHover(CreditButton);
        SetupButtonHover(ExitButton);
        SetupButtonHover(BackButton);
        SetupButtonHover(BackButton2);

        if (black != null)
        {
            black.gameObject.SetActive(false);
            Color initialColor = black.color;
            initialColor.a = 0f;
            black.color = initialColor;
        }
    }

    public void OnPlayClicked()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        if (black != null)
        {
            black.gameObject.SetActive(true);
            black.DOFade(1f, fadeDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => SceneManager.LoadScene(playSceneName));
        }
        else
        {
            SceneManager.LoadScene(playSceneName);
        }
    }

    private void SetupButtonHover(Button btn)
    {
        if (btn == null) return;

        EventTrigger trigger = btn.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((_) =>
        {
            btn.transform.DOKill();
            btn.transform.DOScale(1.15f, 0.15f).SetEase(Ease.OutQuad);
        });
        trigger.triggers.Add(enterEntry);

        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit; 
        exitEntry.callback.AddListener((_) =>
        {
            btn.transform.DOKill();
            btn.transform.DOScale(1f, 0.15f).SetEase(Ease.OutQuad);
        });
        trigger.triggers.Add(exitEntry);
    }

    void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ChangePanel()
    {
        MenuPanel.DOAnchorPos(new Vector2(0f, 0f), transitionDuration).SetEase(Ease.OutCubic);
        CreditPanel.DOAnchorPos(new Vector2(1920f, 0f), transitionDuration).SetEase(Ease.OutCubic);
        SettingPanel.DOAnchorPos(new Vector2(-1920f, 0f), transitionDuration).SetEase(Ease.OutCubic);
    }

    public void SettinPanel()
    {
        MenuPanel.DOAnchorPos(new Vector2(1920f, 0f), transitionDuration).SetEase(Ease.OutCubic);
        CreditPanel.DOAnchorPos(new Vector2(3840f, 0f), transitionDuration).SetEase(Ease.OutCubic);
        SettingPanel.DOAnchorPos(new Vector2(0f, 0f), transitionDuration).SetEase(Ease.OutCubic);
    }

    public void CrediPanel()
    {
        MenuPanel.DOAnchorPos(new Vector2(-1920f, 0f), transitionDuration).SetEase(Ease.OutCubic);
        CreditPanel.DOAnchorPos(new Vector2(0f, 0f), transitionDuration).SetEase(Ease.OutCubic);
        SettingPanel.DOAnchorPos(new Vector2(-3840f, 0f), transitionDuration).SetEase(Ease.OutCubic);
    }
}