using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ValidatedChoicePicker : Room
{

    public GameObject[] roomChoices;

    public override Room createRoom(ExitConstraint requiredExits)
    {
        List<GameObject> roomsThatMeetConstraints = new List<GameObject>();

        foreach (GameObject room in roomChoices)
        {
            ValidatedRoom validatedRoom = room.GetComponent<ValidatedRoom>();
            if (validatedRoom.MeetsContraints(requiredExits))
                roomsThatMeetConstraints.Add(validatedRoom.gameObject);
        }

        GameObject roomPrefab = GlobalFuncs.randElem(roomsThatMeetConstraints);
        return roomPrefab.GetComponent<Room>().createRoom(requiredExits);
    }

    private static bool IsInBounds(Vector2Int node)
    {
        if (node.x >= 0 && node.x < LevelGenerator.ROOM_WIDTH &&
            node.y >= 0 && node.y < LevelGenerator.ROOM_HEIGHT)
        {
            return true;
        }
        else
            return false;
    }
}
