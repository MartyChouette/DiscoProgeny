using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GenerateRollerRinkModeLevel : Room
{
    [Header("Roller Rink Settings")]
    // center rink and size
    public GameObject centerRinkPrefab;
    public float rinkHeight = 20f;
    public float rinkWidth = 30f;

    [Header("Room Settings")]
    // single use rooms for around the rink
    public GameObject[] roomPrefabs;

    // filler crowds
    public GameObject crowdPrefab;

    // room is assumed to be 10x10
    public float roomSize = 10f;

    // space between the rink and the adjacent rooms (and between rooms on one side)
    public float extraSpacing = 2.5f;

    [Header("Rink Offset")]
    // offset needed to not spawn rink NOT at 0,0 (i didnt know how else to achieve this)
    public Vector2 rinkOffset;

    

    // make sure level has not be made before
    private bool layoutGenerated = false;

    // Override fillRoom() for rollerrink
    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        // ensure generate layout once
        if (layoutGenerated)
            return;
        layoutGenerated = true;

        // layout container at the room's center
        GameObject layoutContainer = new GameObject("RollerRinkLayout_");
        layoutContainer.transform.parent = transform;
        layoutContainer.transform.localPosition = Vector3.zero; 

        // offset vector 
        Vector3 offsetVector = new Vector3(rinkOffset.x, rinkOffset.y, 0);

        
        // put rink at the computed offset
        Vector3 rinkPosition = offsetVector;
        Instantiate(centerRinkPrefab, rinkPosition, Quaternion.identity, layoutContainer.transform);

        // slot positions Relative to rink ---
        float halfRinkHeight = rinkHeight / 2f;   // e.g., 10
        float halfRinkWidth = rinkWidth / 2f;      // e.g., 15

        // top and bottom slots, offset Y = halfRinkHeight + extraSpacing + roomSize/2
        float roomOffsetY = halfRinkHeight + extraSpacing + roomSize / 2f;
        // left and right slots, offset X = halfRinkWidth + extraSpacing + roomSize/2
        float roomOffsetX = halfRinkWidth + extraSpacing + roomSize / 2f;

        // top/bottom sides, arrange two slots horizontally
        float totalHorizontal = roomSize * 2 + extraSpacing;
        float topBottomLeftX = -totalHorizontal / 2f + roomSize / 2f;
        float topBottomRightX = totalHorizontal / 2f - roomSize / 2f;

        // left/right sides, arrange two slots vertically
        float totalVertical = roomSize * 2 + extraSpacing;
        float leftRightBottomY = -totalVertical / 2f + roomSize / 2f;
        float leftRightTopY = totalVertical / 2f - roomSize / 2f;

        //  eight  positions (relative to rink center with rotations so that the prefabs "bottom" faces toward the rink
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

        // shuffle the available positions.
        for (int i = availablePositions.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = availablePositions[i];
            availablePositions[i] = availablePositions[j];
            availablePositions[j] = temp;
        }

        // instantiate room prefabs in surrounding slots
        int numToInstantiate = Mathf.Min(availablePositions.Count, roomPrefabs.Length);
        for (int i = 0; i < numToInstantiate; i++)
        {
            // add offsetVector to each surrounding slot
            Vector3 pos = availablePositions[i].Item1 + offsetVector;
            Quaternion rot = availablePositions[i].Item2;
            GameObject selectedRoom = roomPrefabs[i];
            Instantiate(selectedRoom, pos, rot, layoutContainer.transform);
        }

        // instantiate a  placeholder if there are more slots than prefabs
        if (roomPrefabs.Length < availablePositions.Count && crowdPrefab != null)
        {
            Vector3 pos = availablePositions[roomPrefabs.Length].Item1 + offsetVector;
            Quaternion rot = availablePositions[roomPrefabs.Length].Item2;
            Instantiate(crowdPrefab, pos, rot, layoutContainer.transform);
        }
    }
}
