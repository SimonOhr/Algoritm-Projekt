using System;
using System.Collections.Generic;
using CustomAgent;
using Microsoft.Xna.Framework;

namespace Quoridor.AI
{
    class CustomAgent : Agent
    {
        public static bool setDebuggOn = false;
      
        public static void Main()
        {
            new CustomAgent().Start();
        }

        public override Action DoAction(GameData status)
        {
            List<Node> nodeList = new List<Node>();
            List<Edge> edges = new List<Edge>();
            int x1 = status.Self.Position.X;
            int y1 = status.Self.Position.Y;
            //DebuggModeOn(x1, y1); // Note, very very VERY slow

            int id = 0;
            Node[,] nodes = new Node[9, 9];
            for (int i = 0; i < status.Tiles.GetLength(0); i++)
            {
                for (int j = 0; j < status.Tiles.GetLength(1); j++)
                {
                    int x = j;
                    int y = i;
                    nodes[i, j] = new Node(x, y, id++);
                    if (setDebuggOn) nodes[i, j].debuggOn = true;
                    nodeList.Add(nodes[i, j]);
                    if (status.Tiles[i,j].IsOccupied) // under förutsättning att occupied hanterar om en tile innehåller en spelare
                    {
                        nodes[i, j].weight = 0;
                    }                                   
                }
            }

            for (int i = 0; i < status.Tiles.GetLength(0); i++)
            {
                for (int j = 0; j < status.Tiles.GetLength(1); j++)
                {
                    if (i < 8 && j < 8)
                    {
                        if (status.HorizontalWall[i, j])
                        {
                            nodes[i, j].weight = 100;
                            nodes[i + 1, j].weight = 100;
                        }
                        else if (status.VerticalWall[i, j])
                        {
                            nodes[i, j].weight = 100;
                            nodes[i, j + 1].weight = 100;
                        }
                    }
                }
            }
           // AdjacencyList list = new AdjacencyList(81, ref nodes);
            AddEdges(ref edges, ref nodes);

            SetAdj(ref edges, ref nodeList);

            if (setDebuggOn)
            {
                Console.WriteLine("Nodes: " + nodeList.Count);
                Console.WriteLine("Edges: " + edges.Count);
                
                int counter = 0;
                int sum = 0; ;
                foreach (Node n in nodeList)
                {
                    Console.Write(n.adj.Count);
                    counter++;
                    sum += n.adj.Count;
                    if (counter >= 9)
                    {
                        Console.WriteLine("\n");
                        counter = 0;
                    }
                }

               // list.printAdjacencyList();
                if (setDebuggOn) Console.WriteLine(sum);
            }

            //Node start; //Detta kan användas om start sparas efter varje drag, så slipper det finnas en loop som hittar den

            //if (status.Self.Color == Color.Red)
            //{
            //    start = nodes[8, 4];
            //}
            //else
            //{
            //    start = nodes[0, 4];
            //}
            Node start = SetStartNode(status, ref nodes);         
                      
            Node[] path = Dijkstra(ref nodes, status, start, status.Self.Color);
           
            int column = path[1].Y;
            int row = path[1].X;

            //return new PlaceWallAction(5, 5, WallOrientation.Horizontal);
            return new MoveAction(row, column);  // row column var bakvänt           

        }

        private void DebuggModeOn(int x, int y)
        {
            setDebuggOn = true;
            Console.WriteLine("Player posX = " + x + "Player posY = " + y);
        }

        private Node SetStartNode(GameData status, ref Node[,] nodes)
        {
            foreach (Node n in nodes)
            {
                if (n.X == status.Self.Position.X && n.Y == status.Self.Position.Y)
                {
                    return n;
                }
            }
            return null; // ska inte ske om agenten står på en nod
        }


        public void MoveAction(int column, int row)
        {

        }

        public void AddEdges(ref List<Edge> edges, ref Node[,] nodes)
        {           
            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    if (Decrement(j) != -1)
                    {
                        edges.Add(new Edge(nodes[i, j], nodes[i, j - 1]));                       
                    }
                    if (Increment(j) != -1)
                    {
                        edges.Add(new Edge(nodes[i, j], nodes[i, j + 1]));                      
                    }

                    if (Decrement(i) != -1)
                    {
                        edges.Add(new Edge(nodes[i, j], nodes[i - 1, j]));                    
                    }

                    if (Increment(i) != -1)
                    {
                        edges.Add(new Edge(nodes[i, j], nodes[i + 1, j]));                     
                    }
                }
            }
            if (setDebuggOn)
            {
                foreach (Edge e in edges)
                {
                    e.debuggOn = true;
                }
            }
        }
        private int Decrement(int x)
        {
            int value = x;
            value--;
            if (value < 0)
            {
                return -1;
            }
            else
            {
                return value;
            }
        }
        private int Increment(int x)
        {
            int value = x;
            value++;
            if (value > 8)
            {
                return -1;
            }
            else
            {
                return value;
            }
        }

        public void SetAdj(ref List<Edge> edges, ref List<Node> nodeList)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                for (int j = 0; j < nodeList.Count; j++)
                {
                    if (edges[i].Parent.X == nodeList[j].X && edges[i].Parent.Y == nodeList[j].Y) // O~N^2
                    {
                        nodeList[j].adj.Add(edges[i]); 
                        if (setDebuggOn) Console.WriteLine("Added Edge with Parent X = " + edges[i].Parent.X + " Parent Y = " + edges[i].Parent.Y + "To Node At X = " + nodeList[j].X + " And Y = " + nodeList[j].Y);
                          
                    }
                }
            }
          //  edges.Clear();
        }

        private Node[] Dijkstra(ref Node[,] nodes, GameData gameData, Node start, Color color)
        {
            
            Queue<Node> visit = new Queue<Node>();
            Queue<Node> visited = new Queue<Node>();
            Queue<Node> path = new Queue<Node>();

            int targetY;          
            if (color == Color.Red)
            {
                targetY = 0;
            }
            else
            {
                targetY = 8;
            }

            start.distance = 0;
            start.SetStartNode(true);
            start.SetToVisit();
            visit.Enqueue(start);           

            while (visit.Count != 0)
            {
                Node u = visit.Dequeue();
                if (setDebuggOn) Console.WriteLine("Evaluated NodeYX: " + u.Y + " " + u.X);
                EvaluateNeighbors(ref u, ref visit);

                if (u.Y == targetY)
                {
                    Node[] pathArray = new Node[81];
                    Node target = u;

                    if (setDebuggOn)
                    {
                        for (int i = 0; i < visited.Count; i++)
                        {
                            Console.WriteLine("path detected, nodes in queue: " + visited.Dequeue().Y + visited.Dequeue().X);
                            Console.WriteLine("Target found: " + target + " at index " + target.Y + " " + target.X);
                        }
                    }

                    path.Enqueue(target);
                    while (target.previous != null)
                    {
                        target = target.previous;
                        path.Enqueue(target);
                    }                  

                    if (setDebuggOn)
                    {
                        for (int i = 0; i < path.Count; i++)
                        {
                            Node[] tempArray = path.ToArray();
                            Console.WriteLine("path; " + tempArray[i].Y + " " + tempArray[i].X);
                        }
                    }

                    for (int i = path.Count - 1; i >= 0; --i)
                    {
                        pathArray[i] = path.Dequeue();
                    }
                    if (setDebuggOn)
                    {
                        int it = 0;
                        while (pathArray[it] != null)
                        {
                            Console.WriteLine("Path after Reversal, Index " + it + " contains Node at index " + pathArray[it].Y + " " + pathArray[it].X);
                            it++;
                        }
                    }

                    return pathArray;
                }
                if (u != start)
                {
                    u.SetVisited();
                    visited.Enqueue(u);
                }
               
            }
            return null; // ska aldrig hända så länge där finns ett valid target
        }       

        private void EvaluateNeighbors(ref Node u, ref Queue<Node> visit)
        {
            foreach (Edge v in u.adj)
            {
                Node adj = v.Child;
                if (setDebuggOn) Console.WriteLine("Found NodeYX " + adj.Y + " " + adj.X + " Adj to " + u.Y + " " + u.X);
                if (u.weight + adj.weight > 2)
                {
                    Console.WriteLine("wall found");
                }
                int edgeDistance = u.weight + adj.weight;
                int newDistance = u.distance + edgeDistance;
                if ( newDistance < adj.distance )
                {
                    if (setDebuggOn) Console.WriteLine("AdjNode edgeDistance " + edgeDistance + " adj.distance = " + adj.distance);
                    adj.distance = edgeDistance;
                    adj.previous = u;
                    adj.SetToVisit();
                    visit.Enqueue(adj);
                   if (setDebuggOn) Console.WriteLine("AdjNode edgeDistance " + edgeDistance + " new adj.distance = " + adj.distance);
                }
            }
        }       
    }
}