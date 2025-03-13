using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Numerics;

public class ValidatedRoom : Room
{
    public bool hasUpExit, hasDownExit, hasLeftExit, hasRightExit;
    public bool hasUpDownPath, hasUpLeftPath, hasUpRightPath, hasRightLeftPath, hasRightDownPath, hasLeftDownPath;

    public bool Search(Vector2Int startingNode, Vector2Int targetNode)
    {
        List<Vector2Int> openSet = new List<Vector2Int>();
        List<Vector2Int> closedSet = new List<Vector2Int>();
        Vector2Int currentNode;

        if (IsInBounds(startingNode) && IsEmpty(startingNode))
            openSet.Add(startingNode);

        while (openSet.Count > 0)
        {
            currentNode = openSet[0];
            openSet.RemoveAt(0);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
                return true;

            //left
            Vector2Int neighbor = new Vector2Int(currentNode.x - 1, currentNode.y);
            if (IsInBounds(neighbor) && IsEmpty(neighbor) && !openSet.Contains(neighbor) && !closedSet.Contains(neighbor))
                openSet.Add(neighbor);
            //right
            neighbor = new Vector2Int(currentNode.x + 1, currentNode.y);
            if (IsInBounds(neighbor) && IsEmpty(neighbor) && !openSet.Contains(neighbor) && !closedSet.Contains(neighbor))
                openSet.Add(neighbor);
            //up
            neighbor = new Vector2Int(currentNode.x, currentNode.y + 1);
            if (IsInBounds(neighbor) && IsEmpty(neighbor) && !openSet.Contains(neighbor) && !closedSet.Contains(neighbor))
                openSet.Add(neighbor);
            //down
            neighbor = new Vector2Int(currentNode.x, currentNode.y - 1);
            if (IsInBounds(neighbor) && IsEmpty(neighbor) && !openSet.Contains(neighbor) && !closedSet.Contains(neighbor))
                openSet.Add(neighbor);
        }
        return false;
    }

    public bool IsInBounds(Vector2Int node)
    {
        if (node.x >= 0 && node.x < LevelGenerator.ROOM_WIDTH &&
            node.y >= 0 && node.y < LevelGenerator.ROOM_HEIGHT)
        {
            return true;
        }

        return false;
    }

    void ValidateExits()
    {
        LoadData();

        Vector2Int leftExit = new Vector2Int(0, LevelGenerator.ROOM_HEIGHT / 2);
        Vector2Int rightExit = new Vector2Int(LevelGenerator.ROOM_WIDTH - 1, LevelGenerator.ROOM_HEIGHT / 2);
        Vector2Int upExit = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, LevelGenerator.ROOM_HEIGHT - 1);
        Vector2Int downExit = new Vector2Int(LevelGenerator.ROOM_WIDTH / 2, 0);

        hasLeftExit = IsEmpty(leftExit);
        hasRightExit = IsEmpty(rightExit);
        hasUpExit = IsEmpty(upExit);
        hasDownExit = IsEmpty(downExit);

        hasUpLeftPath = Search(upExit, leftExit);
        hasUpRightPath = Search(upExit, rightExit);
        hasUpDownPath = Search(upExit, downExit);
        hasRightLeftPath = Search(rightExit, leftExit);
        hasRightDownPath = Search(rightExit, downExit);
        hasLeftDownPath = Search(leftExit, downExit);
    }

    public bool MeetsContraints(ExitConstraint requiredExits)
    {
        ValidateExits();

        if (requiredExits.upExitRequired && !hasUpExit)
            return false;
        if (requiredExits.downExitRequired && !hasDownExit)
            return false;
        if (requiredExits.leftExitRequired && !hasLeftExit)
            return false;
        if (requiredExits.rightExitRequired && !hasRightExit)
            return false;

        if (requiredExits.upExitRequired && requiredExits.downExitRequired && !hasUpDownPath)
            return false;
        if (requiredExits.upExitRequired && requiredExits.leftExitRequired && !hasUpLeftPath)
            return false;
        if (requiredExits.upExitRequired && requiredExits.rightExitRequired && !hasUpRightPath)
            return false;
        if (requiredExits.rightExitRequired && requiredExits.leftExitRequired && !hasRightLeftPath)
            return false;
        if (requiredExits.rightExitRequired && requiredExits.downExitRequired && !hasRightDownPath)
            return false;
        if (requiredExits.leftExitRequired && requiredExits.downExitRequired && !hasLeftDownPath)
            return false;

        return true;
    }
    //public override Room createRoom(ExitConstraint req
    //uiredExits)
    //{
    //    return base.createRoom(requiredExits);
    //}
}
