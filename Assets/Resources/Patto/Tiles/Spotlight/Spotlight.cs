using UnityEngine;
using static DancePad;

public class Spotlight : Tile
{
    bool doZoomIn;
    float zoomInSpeed = 3;
    float maxZoom = 3;
    float originalCameraSize;
    Vector3 originalCameraPos;
    CameraFollow cameraFollow;

    public GameObject vignette;
    SpriteRenderer vignetteRenderer;
    float vignetteSpeed = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalCameraSize = Camera.main.orthographicSize;
        originalCameraPos = Camera.main.transform.position;
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
        vignetteRenderer = vignette.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (doZoomIn && Camera.main.orthographicSize > maxZoom)
        {
            Camera.main.orthographicSize -= zoomInSpeed * Time.deltaTime;
            Vector2 newPos = Vector2.Lerp((Vector2)Camera.main.transform.position, (Vector2)transform.position, 0.3f);
            Camera.main.transform.position = new Vector3(newPos.x, newPos.y, originalCameraPos.z);

            float vignetteAlpha = Mathf.Clamp(vignetteRenderer.color.a + vignetteSpeed * Time.deltaTime, 0, 0.9f);

            vignetteRenderer.color = new Color(0, 0, 0, vignetteAlpha);
        }
    }

    public override void tileDetected(Tile otherTile)
    {
        doZoomIn = true;
        cameraFollow.enabled = false;
    }

    public override void tileNoLongerDetected(Tile otherTile)
    {
        doZoomIn = false;
        Camera.main.orthographicSize = originalCameraSize;
        Camera.main.transform.position = originalCameraPos;
        cameraFollow.enabled = true;
        vignetteRenderer.color = new Color(0, 0, 0, 0);
    }
}
