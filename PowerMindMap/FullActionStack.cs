using MindNoderPort;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerMindMap
{
    public class FullActionStack : ActionStackInterface
    {
        private List<MindNode> undoactions = new List<MindNode>();
        private List<MindNode> redoactions = new List<MindNode>();
        private int limit = 50;

        public void AddAction()
        {
            MindNode node = new MindNode(0, 0, 0, 0, 0, false);
            String nodeString = "";
            nodeString = GlobalNodeHandler.masterNode.GetRepresentationXString();
            node.FromRepresentation(nodeString);
            undoactions.Add(node);
            if (undoactions.Count > limit)
            {
                undoactions.RemoveAt(0);
            }
            if (redoactions.Count > limit)
            {
                redoactions.RemoveAt(0);
            }
        }


        public void UndoLast()
        {
            if (undoactions.Count >= 1)
            {
                MindNode action = undoactions.Last();
                undoactions.Remove(action);
                redoactions.Add(action);
                if (undoactions.Count >= 1)
                    GlobalNodeHandler.masterNode = undoactions.Last();

                GlobalNodeHandler.viewNode = GlobalNodeHandler.masterNode;
            }
        }

        public void RedoLast()
        {
            if (redoactions.Count >= 1)
            {
                MindNode action = redoactions.Last();
                redoactions.Remove(action);
                undoactions.Add(action);
                if (redoactions.Count >= 1)
                    GlobalNodeHandler.masterNode = redoactions.Last();

                GlobalNodeHandler.viewNode = GlobalNodeHandler.masterNode;
            }
        }

        public void AddAction(MindNodeAction action)
        {
            this.AddAction();
        }
    }
}
