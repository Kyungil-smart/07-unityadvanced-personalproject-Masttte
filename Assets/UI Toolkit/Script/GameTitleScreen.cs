using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class GameTitleScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] UIDocument uiDocument;

    [Header("애니메이션 타이밍")]
    [SerializeField] float introDelay = 0.3f;              // 씬 시작 후 애니메이션 개시까지 대기 시간
    [SerializeField] float titleFadeDuration = 1f;         // 타이틀 텍스트(좌·우) 페이드인 시간
    [SerializeField] float emblemAnimDuration = 1f;        // 엠블럼 회전·등장 시간
    [SerializeField] float subtitleFadeDuration = 0.8f;    // 서브타이틀 페이드인 시간
    [SerializeField] float menuItemFadeDuration = 0.5f;    // 메뉴 항목 하나의 페이드인 시간
    [SerializeField] float menuItemStagger = 0.1f;         // 메뉴 항목 순차 등장 간격
    [SerializeField] float gradientRotationDuration = 10f; // 배경 그라디언트 색상 한 사이클 전환 시간

    VisualElement root;
    VisualElement mergeShimmer;
    VisualElement gradientBackground;
    VisualElement particleContainer;

    List<VisualElement> menuItems = new List<VisualElement>();

    float time;
    Color[] gradientColors = new Color[]
    {
        new Color(1f, 0.42f, 0.62f, 1f),
        new Color(0.77f, 0.27f, 0.41f, 1f),
        new Color(1f, 0.63f, 0.48f, 1f),
        new Color(0.60f, 0.85f, 0.78f, 1f),
        new Color(0.42f, 0.36f, 0.91f, 1f)
    };

    Texture2D ditherPattern;
    Texture2D diagonalPattern;
    Texture2D pixelParticle;
    private void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        ditherPattern = TextureGenerator.GenerateDitherPattern();
        diagonalPattern = TextureGenerator.GenerateDiagonalPattern();
        pixelParticle = TextureGenerator.GeneratePixelParticle();
    }

    private void OnEnable()
    {
        InitializeUI();
        SetupMenuItems();
        StartCoroutine(PlayIntroAnimation());
    }

    private void InitializeUI()
    {
        mergeShimmer = root.Q<VisualElement>("merge-shimmer");
        gradientBackground = root.Q<VisualElement>("gradient-background");
        particleContainer = root.Q<VisualElement>("particle-container");

        var ditherBg = root.Q<VisualElement>("dither-background");
        if (ditherBg != null)
            ditherBg.style.backgroundImage = new StyleBackground(ditherPattern);

        CreateParticles();
    }

    private void SetupMenuItems()
    {
        menuItems.Clear();

        var allMenuItems = root.Query<VisualElement>(className: "split-menu-item").ToList();
        foreach (var menuItem in allMenuItems)
        {
            menuItems.Add(menuItem);

            // 왼쪽 할당
            var leftHalf = menuItem.Q<VisualElement>(className: "menu-left");
            menuItem.RegisterCallback<MouseEnterEvent>(evt =>
            {
                leftHalf.style.backgroundImage = new StyleBackground(diagonalPattern);
            });
            leftHalf.RegisterCallback<ClickEvent>(evt =>
            {
                leftHalf.style.backgroundImage = new StyleBackground(ditherPattern);
            });

            // 오른쪽 할당
            var rightHalf = menuItem.Q<VisualElement>(className: "menu-right");
            rightHalf.RegisterCallback<ClickEvent>(evt => HandleMenuItemClick(rightHalf.name));

        }
    }
    private void HandleMenuItemClick(string itemId)
    {
        switch (itemId)
        {
            case "dream":
                SceneManager.LoadScene(1);
                break;
            case "remember":
                int clearStage = PlayerPrefs.GetInt("ClearStage", 0);
                if (clearStage >= 1)
                    SceneManager.LoadScene(clearStage + 1);
                else
                    SceneManager.LoadScene(1);
                break;
            case "shape":
                if (Screen.fullScreen)
                {
                    Screen.SetResolution(1920, 1080, false);
                }
                else
                {
                    Resolution native = Screen.currentResolution;
                    Screen.SetResolution(native.width, native.height, FullScreenMode.FullScreenWindow);
                }
                break;
            case "wake":
                DOTween.KillAll();
                Application.Quit();
                break;
            default:
                Debug.Log("버걱스");
                break;
        }
    }

    private void CreateParticles()
    {
        for (int i = 0; i < 30; i++)
        {
            var particle = new VisualElement();
            particle.AddToClassList("pixel-particle");
            particle.style.position = Position.Absolute;
            particle.style.width = 4;
            particle.style.height = 4;
            particle.style.backgroundImage = new StyleBackground(pixelParticle);
            particle.style.left = Length.Percent(Random.Range(0f, 100f));
            particle.style.top = Length.Percent(Random.Range(0f, 100f));

            particleContainer.Add(particle);
        }
    }

    private IEnumerator PlayIntroAnimation()
    {
        var titleLeft = root.Q<VisualElement>("title-left");
        var titleRight = root.Q<VisualElement>("title-right");
        var centerEmblem = root.Q<VisualElement>("center-emblem");
        var subtitle = root.Q<VisualElement>("subtitle-section");

        titleLeft.style.opacity = 0;
        titleRight.style.opacity = 0;
        centerEmblem.style.opacity = 0;
        centerEmblem.style.scale = new Scale(Vector3.zero);
        centerEmblem.style.rotate = new Rotate(new Angle(-180));
        subtitle.style.opacity = 0;

        foreach (var item in menuItems)
        {
            item.style.opacity = 0;
            item.style.scale = new Scale(new Vector3(0.9f, 0.9f, 1f));
        }

        yield return new WaitForSeconds(introDelay);

        StartCoroutine(FadeIn(titleLeft, titleFadeDuration));
        StartCoroutine(FadeIn(titleRight, titleFadeDuration));

        yield return new WaitForSeconds(0.2f);

        StartCoroutine(AnimateEmblem(centerEmblem, emblemAnimDuration));

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(FadeIn(subtitle, subtitleFadeDuration));

        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < menuItems.Count; i++)
        {
            StartCoroutine(FadeInScale(menuItems[i], menuItemFadeDuration));
            yield return new WaitForSeconds(menuItemStagger);
        }
    }

    private IEnumerator FadeIn(VisualElement element, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            element.style.opacity = Mathf.Lerp(0, 1, elapsed / duration);
            yield return null;
        }
        element.style.opacity = 1;
    }

    private IEnumerator FadeInScale(VisualElement element, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            element.style.opacity = Mathf.Lerp(0, 1, t);
            float scale = Mathf.Lerp(0.9f, 1f, Easing.OutBack(t));
            element.style.scale = new Scale(new Vector3(scale, scale, 1f));
            yield return null;
        }
        element.style.opacity = 1;
        element.style.scale = new Scale(Vector3.one);
    }

    private IEnumerator AnimateEmblem(VisualElement emblem, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float easedT = Easing.OutElastic(t);

            emblem.style.opacity = t;
            emblem.style.scale = new Scale(Vector3.one * easedT);
            emblem.style.rotate = new Rotate(new Angle(-180 * (1 - easedT)));
            yield return null;
        }

        emblem.style.opacity = 1;
        emblem.style.scale = new Scale(Vector3.one);
        emblem.style.rotate = new Rotate(new Angle(0));
    }

    private void Update()
    {
        time += Time.deltaTime;

        UpdateGradientBackground();
        UpdateShimmer();
        UpdateParticles();
    }

    private void UpdateGradientBackground()
    {
        float t = (time % gradientRotationDuration) / gradientRotationDuration;
        int index = Mathf.FloorToInt(t * gradientColors.Length);
        int nextIndex = (index + 1) % gradientColors.Length;
        float localT = (t * gradientColors.Length) % 1f;

        Color color = Color.Lerp(gradientColors[index], gradientColors[nextIndex], localT);
        gradientBackground.style.backgroundColor = color;
    }

    private void UpdateShimmer()
    {
        float shimmerPos = Mathf.PingPong(time / 3f, 1f);
        mergeShimmer.style.top = Length.Percent(shimmerPos * 100f);

        //float opacity = Mathf.Sin(shimmerPos * Mathf.PI);
        //mergeShimmer.style.opacity = opacity;
    }

    private void UpdateParticles()
    {
        for (int i = 0; i < particleContainer.childCount; i++)
        {
            var particle = particleContainer[i];
            float offset = i * 0.1f;
            float particleTime = time + offset;

            float moveY = Mathf.PingPong(particleTime / 3f, 1f);

            float currentTopValue;
            if (particle.style.top.value.unit == LengthUnit.Percent)
            {
                currentTopValue = particle.style.top.value.value;
            }
            else
            {
                currentTopValue = Random.Range(0f, 100f);
            }

            float newTop = (currentTopValue - moveY * 5f);
            if (newTop < 0) newTop += 100f;
            particle.style.top = Length.Percent(newTop);

            float opacity = Mathf.Sin(moveY * Mathf.PI);
            particle.style.opacity = opacity;

            float scale = Mathf.Lerp(0f, 1.5f, opacity);
            particle.style.scale = new Scale(new Vector3(scale, scale, 1f));
        }
    }
}

public static class Easing
{
    public static float OutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    public static float OutElastic(float t)
    {
        float c4 = (2f * Mathf.PI) / 3f;
        return t == 0f ? 0f : t == 1f ? 1f :
            Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
    }
}
