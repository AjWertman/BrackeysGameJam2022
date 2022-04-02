using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    [SerializeField] float pauseTime = 1f;
    [SerializeField] float scrollSpeed = 5f;

    [SerializeField] Button mainMenuButton = null;
    [SerializeField] Button quitButton = null;

    ScrollRect scrollRect = null;

    bool hasStarted = false;
    bool areButtonsActive = false;
    bool isDone = false;

    IEnumerator scrollingDownCoroutine = null;

    private void Awake()
    {
        scrollRect = GetComponentInChildren<ScrollRect>();
        mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene(0));
        quitButton.onClick.AddListener(() => Application.Quit());

        mainMenuButton.interactable = false;
        quitButton.interactable = false;
    }

    private void Start()
    {
        scrollingDownCoroutine = ScrollingDownBehavior();
        StartCoroutine(scrollingDownCoroutine);
    }

    private void Update()
    {
        if (!hasStarted) return;

        float scrollRectY = scrollRect.normalizedPosition.y;

        if (scrollRectY <= 0 && !areButtonsActive)
        {
            mainMenuButton.interactable = true;
            quitButton.interactable = true;
            StartCoroutine(BackToTheTop());
            areButtonsActive = true;
        }
    }

    private IEnumerator BeginCreditsScrolling()
    {
        yield return new WaitForSeconds(pauseTime);
        yield return ScrollingDownBehavior();
    }

    public IEnumerator ScrollingDownBehavior()
    {
        if (isDone) yield break;
        scrollRect.normalizedPosition = new Vector3(0, 1);
        hasStarted = true;

        while (scrollRect.normalizedPosition.y > 0)
        {
            float scrollRectY = scrollRect.normalizedPosition.y;

            scrollRectY -= Time.deltaTime / scrollSpeed;
            scrollRect.normalizedPosition = new Vector2(0, scrollRectY);
            yield return null;
        }
    }

    private IEnumerator BackToTheTop()
    {
        StopCoroutine(scrollingDownCoroutine);
        yield return ScrollingUpBehavior();
        isDone = true;
    }

    public IEnumerator ScrollingUpBehavior()
    {
        scrollRect.normalizedPosition = new Vector3(0, 0);

        while (scrollRect.normalizedPosition.y < 1f)
        {
            float scrollRectY = scrollRect.normalizedPosition.y;

            scrollRectY += Time.deltaTime / 1;
            scrollRect.normalizedPosition = new Vector2(0, scrollRectY);
            yield return null;
        }       
    }
}
