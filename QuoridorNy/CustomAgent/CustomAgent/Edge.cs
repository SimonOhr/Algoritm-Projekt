using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomAgent
{
    class Edge
    {       
        public Node Parent { get; private set; }
        public Node Child { get; private set; }
        public bool debuggOn;  

        public Edge( Node parent, Node child )
        {
            Parent = parent;
            Child = child;
            if(debuggOn) Console.WriteLine("Added Edge, Parent X = " + Parent.X + " parent Y = " + Parent.Y);
        }
    }
}
