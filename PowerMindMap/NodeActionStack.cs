using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MindNoderPort;

namespace PowerMindMap
{
    public class NodeActionStack : ActionStackInterface
    {
        private List<MindNodeAction> undoactions = new List<MindNodeAction>();
        private List<MindNodeAction> redoactions = new List<MindNodeAction>();
        private int limit = 50;

        public void AddAction(MindNodeAction action)
        {
            undoactions.Add(action);
            if (undoactions.Count > limit)
            {
                undoactions.RemoveAt(0);
            }
            if (redoactions.Count > limit)
            {
                redoactions.RemoveAt(0);
            }
        }

        public void RedoLast()
        {
            throw new NotImplementedException();
        }

        public void UndoLast()
        {
            if (undoactions.Count >= 1)
            {
                MindNodeAction action = undoactions.Last();
                undoactions.Remove(action);
                switch (action.name)
                {
                    case "CreateNode":
                        if (action.involvedNodes.Count >= 1)
                        {
                            MindNode createdNode = action.involvedNodes.Pop();
                            redoactions.Add(new MindNodeAction(1, "DeleteNode", createdNode));
                            createdNode.DeleteNode();
                        }
                        break;
                    case "DeleteNode":
                        if (action.involvedNodes.Count >= 1)
                        {
                            MindNode deletednode = action.involvedNodes.Pop();
                            redoactions.Add(new MindNodeAction(0, "CreateNode", deletednode));
                            if (deletednode.parent != null)
                                deletednode.parent.AddChildNoStyle(deletednode, false);
                            // Refresh Pivot Representation after reinstating node
                            deletednode.UpdatePivots();
                        }
                        break;
                    case "MoveNodes":
                        if (action.involvedNodes.Count >= 1)
                        {
                            MindNodeAction newaction = new MindNodeAction(2, "MoveNodes");
                            foreach (MindNode movedNode in action.involvedNodes)
                            {

                                newaction.involvedNodes.Push(movedNode);
                                newaction.startpoint.Push(new CalcPoint(movedNode.xpos, movedNode.ypos));

                                if (action.startpoint.Count > 0)
                                {
                                    CalcPoint oldPos = action.startpoint.Pop();

                                    movedNode.SetPosition(oldPos.X, oldPos.Y, false);
                                }
                                // Refresh Pivot Representation after moving node
                                movedNode.UpdatePivots();
                            }
                            redoactions.Add(newaction);
                        }
                        break;
                    case "ConnectNodes":
                        if (action.involvedNodes.Count >= 2)
                        {
                            MindNodeAction newAction = new MindNodeAction(3, "DeleteConnections");
                            for (int i = 0; i < action.involvedNodes.Count / 2; i++)
                            {
                                MindNode rightnode = action.involvedNodes.Pop();
                                MindNode leftnode = action.involvedNodes.Pop();
                                
                                newAction.involvedNodes.Push(leftnode);
                                newAction.involvedNodes.Push(rightnode);

                                leftnode.DeleteConnection(rightnode);
                            }
                            redoactions.Add(newAction);
                        }
                        break;
                    case "DeleteConnections":
                        if (action.involvedNodes.Count >= 2)
                        {
                            MindNodeAction newAction = new MindNodeAction(3, "ConnectNodes");
                            for (int i = 0; i < action.involvedNodes.Count / 2; i++)
                            {

                                MindNode rightnode = action.involvedNodes.Pop();
                                MindNode leftnode = action.involvedNodes.Pop();

                                MindNode rightExistingNode = GlobalNodeHandler.masterNode.GetExistingNode(rightnode.id);
                                MindNode leftExistingNode = GlobalNodeHandler.masterNode.GetExistingNode(rightnode.id);

                                newAction.involvedNodes.Push(leftnode);
                                newAction.involvedNodes.Push(rightnode);

                                //leftnode.AddConnection(rightnode);

                                // Refresh Pivot Representation after reinstating connection
                                //leftnode.UpdatePivots();
                            }

                            redoactions.Add(newAction);
                        }
                        break;
                    case "ChangeText":
                        if (action.involvedNodes.Count >= 1)
                        {
                            MindNodeAction newaction = new MindNodeAction(4, "ChangeText");

                            MindNode changedNode = action.involvedNodes.Pop();
                            newaction.text = changedNode.text;
                            newaction.involvedNodes.Push(changedNode);

                            changedNode.SetText(action.text);

                            redoactions.Add(newaction);
                        }
                        else if (action.involvedLabel != null)
                        {
                            MindNodeAction newaction = new MindNodeAction(4, "ChangeText");

                            NodeLabel changedLabel = action.involvedLabel;
                            newaction.text = changedLabel.GetText();
                            newaction.involvedLabel = changedLabel;

                            changedLabel.SetText(action.text);

                            redoactions.Add(newaction);
                        }
                        break;
                    case "Transform":
                        if (action.involvedNodes.Count >= 1)
                        {
                            MindNodeAction newaction = new MindNodeAction(5, "Transform");

                            MindNode changedNode = action.involvedNodes.Pop();
                            newaction.sourceSize.X = changedNode.width;
                            newaction.sourceSize.Y = changedNode.height;
                            newaction.startpoint.Push(new CalcPoint(changedNode.xpos, changedNode.ypos));
                            newaction.involvedNodes.Push(changedNode);

                            if (action.sourceSize.X != -1)
                            {
                                changedNode.width = action.sourceSize.X;
                                changedNode.SetScaled(false);
                            }
                            if (action.sourceSize.Y != -1)
                            {
                                changedNode.height = action.sourceSize.Y;
                                changedNode.SetScaled(false);
                            }

                            CalcPoint oldPos = action.startpoint.Pop();
                            if (oldPos != null)
                                changedNode.SetPosition(oldPos.X, oldPos.Y, false);

                            // Refresh Pivot Representation after resetting transformation
                            changedNode.UpdatePivots();
                            changedNode.updateRepresentation();

                            redoactions.Add(newaction);
                        }
                        break;
                }

            }
        }
    }
}
