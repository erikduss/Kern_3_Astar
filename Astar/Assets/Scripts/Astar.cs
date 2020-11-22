using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        //step 1 -> create open list
        List<Node> openList = new List<Node>();
        List<Node> ClosedList = new List<Node>();

        List<Vector2Int> FinalPath = new List<Vector2Int>();

        //step 2 -> add all available nodes to the open list.
        foreach (Cell cell in grid)
        {
            //int tempGscore = CalculateDistance(startPos, cell.gridPosition);
            int tempGscore = int.MaxValue;
            int tempHscore = CalculateDistance(cell.gridPosition, endPos);
            Node tempParent = null;
            Vector2Int tempPos = cell.gridPosition;

            openList.Add(new Node(tempPos, tempParent, tempGscore, tempHscore));
        }

        Node currentNode = openList.Find(x => x.position == startPos);
        currentNode.GScore = 0;

        while (openList.Count > 0)
        {
            openList.Remove(currentNode);
            ClosedList.Add(currentNode);

            foreach(Node neighbourNode in GetNeighbourList(currentNode, openList, grid))
            {
                float tentativeGScore = currentNode.GScore + CalculateDistance(currentNode.position, neighbourNode.position);

                neighbourNode.parent = currentNode;
                neighbourNode.GScore = tentativeGScore;
                neighbourNode.HScore = CalculateDistance(neighbourNode.position, endPos);
            }

            Node lowestnode = GetLowestFScoreNode(GetNeighbourList(currentNode, openList, grid));

            if (lowestnode != null)
            {
                currentNode = lowestnode;
            }
                

            if (currentNode.position == endPos)
            {
                return CalculatePath(currentNode);
            }
        }

        return null;
    }

    private List<Node> GetNeighbourList(Node currentNode, List<Node> openNodes, Cell[,] grid)
    {
        List<Node> neighbourList = new List<Node>();

        //The left node is accessible if there is no wall on the left side of the current tile
        if (!grid[currentNode.position.x, currentNode.position.y].HasWall(Wall.LEFT))
        {
            //left
            Node leftNode = openNodes.Find(x => x.position == new Vector2Int((currentNode.position.x - 1), currentNode.position.y));
            if(leftNode != null)
            {
            
                neighbourList.Add(leftNode);
            }
        }

        //The right node is accessible if there is no wall on the right side of the current tile
        if (!grid[currentNode.position.x, currentNode.position.y].HasWall(Wall.RIGHT))
        {
            //right
            Node rightNode = openNodes.Find(x => x.position == new Vector2Int((currentNode.position.x + 1), currentNode.position.y));
            if (rightNode != null)
            {
                neighbourList.Add(rightNode);
            }
        }

        //The above node is accessible if there is no wall on above the current tile
        if (!grid[currentNode.position.x, currentNode.position.y].HasWall(Wall.UP))
        {
            //up
            Node upNode = openNodes.Find(x => x.position == new Vector2Int(currentNode.position.x, (currentNode.position.y + 1)));
            if (upNode != null)
            {
            
                neighbourList.Add(upNode);
            }
        }

        //The down node is accessible if there is no wall on the bottom side of the current tile
        if (!grid[currentNode.position.x, currentNode.position.y].HasWall(Wall.DOWN))
        {
            //down
            Node downNode = openNodes.Find(x => x.position == new Vector2Int(currentNode.position.x, (currentNode.position.y - 1)));
            if (downNode != null)
            {
            
                neighbourList.Add(downNode);
            }
        }

        return neighbourList;
    }

    private List<Vector2Int> CalculatePath(Node endNode)
    {
        List<Node> path = new List<Node>();
        path.Add(endNode);

        Node currentNode = endNode;

        while (currentNode.parent != null)
        {
            path.Add(currentNode.parent);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        List<Vector2Int> pathPositions = new List<Vector2Int>();

        foreach(Node pos in path)
        {
            pathPositions.Add(pos.position);
        }

        return pathPositions;
    }

    private int CalculateDistance(Vector2Int a, Vector2Int b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return 14 * Mathf.Min(xDistance, yDistance) + 10 * remaining;
    }

    private Node GetLowestFScoreNode(List<Node> nodeList)
    {
        Node lowestNode = nodeList[0];
        
        for(int i =0; i < nodeList.Count; i++)
        {
            if(nodeList[i].FScore < lowestNode.FScore)
            {
                lowestNode = nodeList[i];
            }
        }

        return lowestNode;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
