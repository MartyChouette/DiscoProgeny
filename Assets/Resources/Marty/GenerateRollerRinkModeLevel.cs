using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRollerRinkModeLevel : Room
{
    [Header("roller rink settings")]
    // center rink and size
    public GameObject centerRinkPrefab;
    public float rinkHeight = 20f;
    public float rinkWidth = 30f;

    [Header("room settings")]
    // single use rooms for around the rink
    public GameObject[] roomPrefabs;

    [Header("crowd prefabs")]
    // static crowd prefab placed around the rink
    public GameObject staticCrowdPrefab;
    // dynamic crowd prefab for placement
    public GameObject dynamicCrowdPrefab;

    // room is assumed to be 10x10
    public float roomSize = 10f;
    // space between the rink and the adjacent rooms and between rooms on one side
    public float extraSpacing = 2.5f;

    [Header("rink offset")]
    // offset needed to not spawn rink not at 0 0 i didnt know how else to achieve this
    public Vector2 rinkOffset;

    [Header("crowd generation settings")]
    // grid settings for bounding the dynamic crowd placement
    public float gridCellSize = 1f;
    public int gridWidth = 50;
    public int gridHeight = 50;

    // number of dynamic groups to place
    public int maxGroupCount = 20;

    // list of areas to leave clear for rink rooms static crowds etc
    private List<Rect> noSpawnZones = new List<Rect>();
    // level offset used for centering
    private Vector3 levelOffset;
    // container for all level objects
    private GameObject layoutContainer;
    // ensure level is only generated once
    private bool layoutGenerated = false;

    private List<GameObject> dynamicCrowds = new List<GameObject>();


    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        if (layoutGenerated)
            return;
        layoutGenerated = true;

        layoutContainer = new GameObject("rollerrinklayout_");
        layoutContainer.transform.parent = transform;
        layoutContainer.transform.localPosition = Vector3.zero;

        levelOffset = new Vector3(rinkOffset.x, rinkOffset.y, 0);

        // place the rink
        Vector3 rinkPosition = levelOffset;
        Instantiate(centerRinkPrefab, rinkPosition, Quaternion.identity, layoutContainer.transform);
        noSpawnZones.Add(new Rect(rinkPosition.x - rinkWidth / 2, rinkPosition.y - rinkHeight / 2, rinkWidth, rinkHeight));

        // compute room positions relative to the rink
        float halfRinkHeight = rinkHeight / 2f;
        float halfRinkWidth = rinkWidth / 2f;
        float roomOffsetY = halfRinkHeight + extraSpacing + roomSize / 2f;
        float roomOffsetX = halfRinkWidth + extraSpacing + roomSize / 2f;

        float totalHorizontal = roomSize * 2 + extraSpacing;
        float topBottomLeftX = -totalHorizontal / 2f + roomSize / 2f;
        float topBottomRightX = totalHorizontal / 2f - roomSize / 2f;

        float totalVertical = roomSize * 2 + extraSpacing;
        float leftRightBottomY = -totalVertical / 2f + roomSize / 2f;
        float leftRightTopY = totalVertical / 2f - roomSize / 2f;

        // define eight possible room positions with rotations so that bottom faces toward the rink
        var topLeft = (pos: new Vector3(topBottomLeftX, roomOffsetY, 0), rot: Quaternion.Euler(0, 0, 0));
        var topRight = (pos: new Vector3(topBottomRightX, roomOffsetY, 0), rot: Quaternion.Euler(0, 0, 0));
        var bottomLeft = (pos: new Vector3(topBottomLeftX, -roomOffsetY, 0), rot: Quaternion.Euler(0, 0, 180));
        var bottomRight = (pos: new Vector3(topBottomRightX, -roomOffsetY, 0), rot: Quaternion.Euler(0, 0, 180));
        var rightTop = (pos: new Vector3(roomOffsetX, leftRightTopY, 0), rot: Quaternion.Euler(0, 0, -90));
        var rightBottom = (pos: new Vector3(roomOffsetX, leftRightBottomY, 0), rot: Quaternion.Euler(0, 0, -90));
        var leftTop = (pos: new Vector3(-roomOffsetX, leftRightTopY, 0), rot: Quaternion.Euler(0, 0, 90));
        var leftBottom = (pos: new Vector3(-roomOffsetX, leftRightBottomY, 0), rot: Quaternion.Euler(0, 0, 90));

        List<(Vector3, Quaternion)> availablePositions = new List<(Vector3, Quaternion)> {
            topLeft, topRight,
            bottomLeft, bottomRight,
            rightTop, rightBottom,
            leftTop, leftBottom
        };

        // shuffle available positions
        for (int i = availablePositions.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = availablePositions[i];
            availablePositions[i] = availablePositions[j];
            availablePositions[j] = temp;
        }

        // instantiate room prefabs in available slots (rooms get rotated so bottom is toward the rink)
        int numToInstantiate = Mathf.Min(availablePositions.Count, roomPrefabs.Length);
        for (int i = 0; i < numToInstantiate; i++)
        {
            Vector3 pos = availablePositions[i].Item1 + levelOffset;
            Instantiate(roomPrefabs[i], pos, availablePositions[i].Item2, layoutContainer.transform);
            noSpawnZones.Add(new Rect(pos.x - roomSize / 2, pos.y - roomSize / 2, roomSize, roomSize));
        }

        // if extra slot exists, place a group of 2-3 static crowds (crowds are not rotated)
        if (roomPrefabs.Length < availablePositions.Count && staticCrowdPrefab != null)
        {
            Vector3 pos = availablePositions[roomPrefabs.Length].Item1 + levelOffset;
            int groupSize = Random.Range(2, 4);
            for (int i = 0; i < groupSize; i++)
            {
                Vector3 offset = new Vector3(Random.Range(-roomSize / 4f, roomSize / 4f), Random.Range(-roomSize / 4f, roomSize / 4f), 0);
                Instantiate(staticCrowdPrefab, pos + offset, Quaternion.identity, layoutContainer.transform);
            }
            noSpawnZones.Add(new Rect(pos.x - roomSize / 2, pos.y - roomSize / 2, roomSize, roomSize));
        }

        // place dynamic crowds randomly and start drifting them
        PlaceDynamicCrowds();
        StartCoroutine(CrowdDrift());
    }

    // place dynamic crowds using random distribution within a bounding box
    private void PlaceDynamicCrowds()
    {
        Vector3 minBounds = levelOffset - new Vector3(gridWidth * gridCellSize / 2f, gridHeight * gridCellSize / 2f, 0);
        Vector3 maxBounds = levelOffset + new Vector3(gridWidth * gridCellSize / 2f, gridHeight * gridCellSize / 2f, 0);
        int maxAttempts = 10;

        for (int i = 0; i < maxGroupCount; i++)
        {
            int attempts = 0;
            bool placed = false;
            while (attempts < maxAttempts && !placed)
            {
                attempts++;
                float randX = Random.Range(minBounds.x, maxBounds.x);
                float randY = Random.Range(minBounds.y, maxBounds.y);
                Vector3 pos = new Vector3(randX, randY, levelOffset.z);
                bool valid = true;
                foreach (Rect rect in noSpawnZones)
                {
                    if (rect.Contains(new Vector2(pos.x, pos.y)))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    GameObject crowd = Instantiate(dynamicCrowdPrefab, pos, Quaternion.identity, layoutContainer.transform);
                    dynamicCrowds.Add(crowd);
                    placed = true;
                }
            }
        }
    }

    // coroutine to drift dynamic crowds slowly
    private IEnumerator CrowdDrift()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            foreach (GameObject crowd in dynamicCrowds)
            {
                if (crowd != null)
                {
                    Vector3 offset = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0);
                    crowd.transform.position += offset;
                }
            }
        }
    }
}
