using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

class Program
{
    static void Main()
    {
        char[][] matrix = new char[][] { new char[] {'-', 'S', '-', '-', 'X'},
                                         new char[] {'-', 'X', 'X', '-', '-'},
                                         new char[] {'-', '-', '-', 'X', '-'},
                                         new char[] {'X', '-', 'X', 'E', '-'},
                                         new char[] {'-', '-', '-', '-', 'X'}};

        //looking for shortest path from 'S' at (0,1) to 'E' at (3,3)
        //obstacles marked by 'X'
        int fromX = 0, fromY = 1, toX = 3, toY = 3;
        matrixNode endNode = AStar(matrix, fromX, fromY, toX, toY);

        //looping through the Parent nodes until we get to the start node
        Stack<matrixNode> path = new Stack<matrixNode>();

        while (endNode.x != fromX || endNode.y != fromY)
        {
            path.Push(endNode);
            endNode = endNode.parent;
        }
        path.Push(endNode);

        Console.WriteLine("The shortest path from  " +
                          "(" + fromX + "," + fromY + ")  to " +
                          "(" + toX + "," + toY + ")  is:  \n");

        char[][] moddedMap = (char[][])matrix.Clone();

        while (path.Count > 0)
        {
            matrixNode node = path.Pop();
            moddedMap[node.x][node.y] = '*';
            Console.WriteLine("(" + node.x + "," + node.y + ")");
        }
        for (int i = 0; i < moddedMap.Count(); i++)
        {
            for (int k = 0; k < moddedMap[i].Count(); k++)
            {
                Console.Write(moddedMap[i][k] + " ");
            }
            Console.WriteLine("");
        }

    }

  

    public class matrixNode
    {
        public int fr = 0, to = 0, sum = 0;
        public int x, y;
        public matrixNode parent;
    }

    public static matrixNode AStar(char[][] matrix, int fromX, int fromY, int toX, int toY)
    {
        //the keys for nodeGreen and nodeRed are x.ToString() + y.ToString() of the matrixNode 
        Dictionary<string, matrixNode> nodeGreen = new Dictionary<string, matrixNode>(); //open 
        Dictionary<string, matrixNode> nodeRed = new Dictionary<string, matrixNode>(); //closed 

        matrixNode startNode = new matrixNode { x = fromX, y = fromY };
        string key = startNode.x.ToString() + startNode.x.ToString();
        nodeGreen.Add(key, startNode);

        Func<KeyValuePair<string, matrixNode>> smallestGreen = () =>
        {
            KeyValuePair<string, matrixNode> smallest = nodeGreen.ElementAt(0);

            foreach (KeyValuePair<string, matrixNode> item in nodeGreen)
            {
                if (item.Value.sum < smallest.Value.sum)
                    smallest = item;
                else if (item.Value.sum == smallest.Value.sum
                        && item.Value.to < smallest.Value.to)
                    smallest = item;
            }

            return smallest;
        };


        //add these values to current nodes x and y values to get the left/up/right/bottom neighbors
        List<KeyValuePair<int, int>> fourNeighbors = new List<KeyValuePair<int, int>>()
                                            { new KeyValuePair<int, int>(-1,0),
                                              new KeyValuePair<int, int>(0,1),
                                              new KeyValuePair<int, int>(1, 0),
                                              new KeyValuePair<int, int>(0,-1),
                                              //And Diagonally moving
                                              new KeyValuePair<int, int>(-1,1), //top left
                                              new KeyValuePair<int, int>(-1,-1),//bot left
                                              new KeyValuePair<int, int>(1,1),//top right
                                              new KeyValuePair<int, int>(1,-1),//bot right
                                            };
        //Node layout
        //-1,1   0,1   1,1
        //-1,0   0,0   1,0
        //-1,-1  0,-1  1,-1

        int maxX = matrix.GetLength(0);
        if (maxX == 0)
            return null;
        int maxY = matrix[0].Length;

        while (true)
        {
            if (nodeGreen.Count == 0)
                return null;

            KeyValuePair<string, matrixNode> current = smallestGreen();
            if (current.Value.x == toX && current.Value.y == toY)
                return current.Value;

            nodeGreen.Remove(current.Key);
            nodeRed.Add(current.Key, current.Value);

            foreach (KeyValuePair<int, int> plusXY in fourNeighbors)
            {
                int nbrX = current.Value.x + plusXY.Key;
                int nbrY = current.Value.y + plusXY.Value;
                string nbrKey = nbrX.ToString() + nbrY.ToString();
                if (nbrX < 0 || nbrY < 0 || nbrX >= maxX || nbrY >= maxY
                    || matrix[nbrX][nbrY] == 'X' //obstacles marked by 'X'
                    || nodeRed.ContainsKey(nbrKey))
                    continue;

                if (nodeGreen.ContainsKey(nbrKey))
                {
                    matrixNode curNbr = nodeGreen[nbrKey];
                    int from = Math.Abs(nbrX - fromX) + Math.Abs(nbrY - fromY);
                    if (from < curNbr.fr)
                    {
                        curNbr.fr = from;
                        curNbr.sum = curNbr.fr + curNbr.to;
                        curNbr.parent = current.Value;
                    }
                }
                else
                {
                    matrixNode curNbr = new matrixNode { x = nbrX, y = nbrY };
                    curNbr.fr = Math.Abs(nbrX - fromX) + Math.Abs(nbrY - fromY);
                    curNbr.to = Math.Abs(nbrX - toX) + Math.Abs(nbrY - toY);
                    curNbr.sum = curNbr.fr + curNbr.to;
                    curNbr.parent = current.Value;
                    nodeGreen.Add(nbrKey, curNbr);
                }
            }
        }
    } 
}
