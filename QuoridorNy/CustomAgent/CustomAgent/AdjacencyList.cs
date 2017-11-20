using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomAgent
{
    class AdjacencyList
    {
        Node[,] nodes;
        LinkedList<Tuple<int, int>>[] adjacencyList;

        public AdjacencyList(int vertices, ref Node[,]nodes)
        {
            adjacencyList = new LinkedList<Tuple<int, int>>[vertices];

            for (int i = 0; i < adjacencyList.Length; i++)
            {
                adjacencyList[i] = new LinkedList<Tuple<int, int>>();
            }
        }
        public void AddEdgeAtBegin(int startVertex, int endVertex, int weight)
        {
            adjacencyList[startVertex].AddLast(new Tuple<int, int>(endVertex, weight));
        }
        public LinkedList<Tuple<int, int>> this[int index]
        {
            get
            {
                LinkedList<Tuple<int, int>> edgeList
                               = new LinkedList<Tuple<int, int>>(adjacencyList[index]);

                return edgeList;
            }
        }

        public LinkedList<Tuple<int,int>> name(int index)
        {
            LinkedList<Tuple<int, int>> edgeList
                              = new LinkedList<Tuple<int, int>>(adjacencyList[index]);
            return edgeList;
        }
        public void printAdjacencyList()
        {
            int i = 0;

            foreach (LinkedList<Tuple<int, int>> list in adjacencyList)
            {
               // Console.Write("adjacencyList[" + i + "] -> ");

                foreach (Tuple<int, int> edge in list)
                {
                    Console.Write(edge.Item1 + "(" + edge.Item2 + ")");
                }

                ++i;
                Console.WriteLine();
            }
        }
    }
}
