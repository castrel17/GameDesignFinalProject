using UnityEngine;

public class HoldNoteController : MonoBehaviour
{
    public float startBeat;
    public float endBeat;
    public Vector2 startPos;
    public Vector2 endPos;

    public HoldNoteStart startNote;
    public HoldNoteEnd endNote;

    private DemoSongManager songManager;

    public Transform connectorSprite; 

    void Start()
    {
        songManager = GetComponentInParent<DemoSongManager>();

        if (startNote != null)
            startNote.Initialize(startBeat, startPos, endPos, songManager);

        if (endNote != null)
            endNote.Initialize(endBeat, startPos, endPos, songManager);

        Debug.Log($"[HoldNoteController] Initialized with StartBeat={startBeat}, EndBeat={endBeat}");
    }

    void Update()
    {
        if (connectorSprite != null && startNote != null && endNote != null)
        {
            Vector3 start = startNote.transform.localPosition;
            Vector3 end = endNote.transform.localPosition;

            Vector3 midPoint = (start + end) / 2f;
            connectorSprite.localPosition = midPoint;

            Vector3 direction = end - start;
            float length = direction.magnitude;
            connectorSprite.localScale = new Vector3(1f, length / 15.7f, 1f);
        }

        if (startNote.evaluated && startNote.isMissedStart)
        {
            Destroy(gameObject);
            return;
        }

        if (startNote.evaluated && endNote.evaluated)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(float startBeat, float endBeat, Vector2 startPos, Vector2 endPos, DemoSongManager songManager)
    {
        this.startBeat = startBeat;
        this.endBeat = endBeat;
        this.startPos = startPos;
        this.endPos = endPos;
        this.songManager = songManager;

        if (startNote != null)
            startNote.Initialize(startBeat, startPos, endPos, songManager);

        if (endNote != null)
            endNote.Initialize(endBeat, startPos, endPos, songManager);
    }
}