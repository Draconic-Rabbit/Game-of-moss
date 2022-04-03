using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlameThrower : MonoBehaviour
{
    Point gridSize = new Point(64, 40);
    bool isFiring = false;
    IEnumerator firingCoroutine;

    [SerializeField] float damageInterval = 0.1f;
    [SerializeField] float flameDistance = 1.5f;
    [SerializeField] float flameRadius = 0.5f;

    MossController mossController;
    [SerializeField] ParticleSystem flame;

    [SerializeField] GameObject areaDebugDisplay;

    [SerializeField] AudioClip lightingAudioClip;
    [SerializeField] AudioPlayer audioPlayer;


    private void Awake()
    {
        flame.Stop();
        mossController = FindObjectOfType<MossController>();
        audioPlayer = FindObjectOfType<AudioPlayer>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnFire(InputValue value)
    {
        if (!GetComponent<PlayerMovement>().controlEnabled) { return; }
        isFiring = value.isPressed;
    }



    // Update is called once per frame
    void Update()
    {
        if (isFiring && firingCoroutine == null)
        {
            firingCoroutine = FireContinuously();
            StartCoroutine(firingCoroutine);
        }
        else if (!isFiring && firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
            flame.Stop();
        }
    }

    IEnumerator FireContinuously()
    {
        audioPlayer.PlayLightFTclip();
        yield return new WaitForSeconds(2.5f);
        audioPlayer.PlayFireFTClip();
        flame.Play();
        while (true)
        {
            Vector2 origin = transform.position;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 direction = mousePos - origin;
            Vector2 targetPos = origin + direction.normalized * flameDistance;
            // areaDebugDisplay.transform.position = targetPos; // Debug Helper

            List<Point> affectedCells = FlameThrowAt(targetPos);
            foreach (Point affectedCell in affectedCells)
            {
                mossController.KillMossAt(affectedCell);
            }

            yield return new WaitForSeconds(damageInterval);
        }
    }

    private List<Point> FlameThrowAt(Vector3 worldPosition)
    {
        float worldResolution = 1f / (gridSize.x / 16);
        List<Point> touchedPoint = new List<Point>();

        float minX = worldPosition.x - flameRadius;
        float maxX = worldPosition.x + flameRadius;
        float minY = worldPosition.y - flameRadius;
        float maxY = worldPosition.y + flameRadius;

        for (float x = minX; x <= maxX; x += worldResolution)
        {
            for (float y = minY; y <= maxY; y += worldResolution)
            {
                Point p = WorldToGridPoint(new Vector3(x, y, 0));
                Vector3 CenterOfGridPointInWorld = GridPointCenterToWorldPoint(p);
                float dist = Vector3.Distance(worldPosition, CenterOfGridPointInWorld);
                if (dist < flameRadius)
                {
                    touchedPoint.Add(p);
                }
            }
        }

        return touchedPoint;
    }


    private Point WorldToGridPoint(Vector3 worldPosition)
    {
        int gridResolution = gridSize.x / 16;
        Vector2 positiveWorldPosition = new Vector2(worldPosition.x + 8, worldPosition.y + 5);
        Vector2 cellPosition = new Vector2(positiveWorldPosition.x * gridResolution, positiveWorldPosition.y * gridResolution);
        return new Point(Mathf.FloorToInt(cellPosition.x), Mathf.FloorToInt(cellPosition.y));
    }

    private Vector3 GridPointCenterToWorldPoint(Point gridPoint)
    {
        float worldResolution = 1f / (gridSize.x / 16);
        float x = (gridPoint.x + 0.5f) * worldResolution - 8;
        float y = (gridPoint.y + 0.5f) * worldResolution - 5;
        return new Vector3(x, y, 0);
    }
}
