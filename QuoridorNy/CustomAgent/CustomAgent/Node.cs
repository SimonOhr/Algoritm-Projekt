using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomAgent
{
    class Node
    {
        public int id;
        public int distance = int.MaxValue;
        public int weight = 1;
        public Node previous;
        public int X { get; private set; }
        public int Y { get; private set; }
        public List<Edge> adj = new List<Edge>();
        private bool isStartNode;
        private bool hasBeenVisited;
        private bool setToBeVisited;
        private bool isPath;
        public bool debuggOn;
        public Node( int x, int y, int id )
        {
            X = x;
            Y = y;
            this.id = id;
            if (debuggOn)Console.WriteLine("Added Node X = " + X + "Node Y = " + Y);          
        }

        public void SetStartNode(bool setStartNode)
        {
            isStartNode = setStartNode;
        }
        public void SetVisited()
        {
            hasBeenVisited = true;
            setToBeVisited = false;
        }
        public void SetToVisit()
        {
            setToBeVisited = true;
        }
        public void IsPath()
        {
            isPath = true;
        }
    }
}
