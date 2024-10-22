using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public PlayableDirector timeline;
    public GameObject objectToEnable;
    public KeyCode skipKey = KeyCode.Escape;

    private bool isCutscenePlaying = true;

    void Update()
    {

        if (isCutscenePlaying && Input.GetKeyDown(skipKey))
        {
            SkipCutscene();
        }
    }

    void SkipCutscene()
    {
        // Stop the timeline
        if (timeline != null)
        {
            timeline.Stop();
        }

        // Enable Loading Screen
        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }

        isCutscenePlaying = false;
    }
}
