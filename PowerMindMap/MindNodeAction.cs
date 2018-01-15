using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MindNoderPort
{
    public class MindNodeAction
    {
        public int id;
        public String name;
        public Stack<MindNode> involvedNodes = new Stack<MindNode>();

        public NodeLabel involvedLabel = null;
        public String text;
        public CalcPoint sourceSize = new CalcPoint(-1,-1);

        public Stack<CalcPoint> startpoint = new Stack<CalcPoint>();

        public MindNodeAction(int newid, String newName, Stack<MindNode> Nodes)
        {
            this.involvedNodes = Nodes;
            this.id = newid;
            this.name = newName;
        }
        public MindNodeAction(int newid, String newName, MindNode node)
        {
            this.involvedNodes.Push(node);
            this.id = newid;
            this.name = newName;
        }
        public MindNodeAction(int newid, String newName, NodeLabel involvedLabel, String tex)
        {
            this.involvedLabel = involvedLabel;
            this.id = newid;
            this.name = newName;
            this.text = tex;
        }
        public MindNodeAction(int newid, String newName, MindNode node,String tex)
        {
            this.involvedNodes.Push(node);
            this.id = newid;
            this.name = newName;
            this.text = tex;
        }
        public MindNodeAction(int newid, String newName)
        {
            this.id = newid;
            this.name = newName;
        }
        public MindNodeAction(MindNode node)
        {
            this.involvedNodes.Push(node);
        }
        public MindNodeAction(int newid, String newName, MindNode node1, MindNode node2)
        {
            this.involvedNodes.Push(node1);
            this.involvedNodes.Push(node2);
            this.id = newid;
            this.name = newName;
        }
        public MindNodeAction(MindNodeAction ac, int newid, String newName)
        {
            this.id = newid;
            this.name = newName;
        }

    }
}
