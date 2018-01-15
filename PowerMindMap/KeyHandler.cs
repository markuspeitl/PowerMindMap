using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MindNoderPort
{
    public class KeyHandler
    {
        private TextBox activetextbox;
        public KeyHandler(TextBox activetextbox)
        {
            this.activetextbox = activetextbox;
        }

        Point arrowdir = new Point(0, 0);
        public void thisKeyDownExec(KeyObject keypress)
        {
            if (!GlobalNodeHandler.typing)
            {
                int x = keypress.currentMousePos.X;
                int y = keypress.currentMousePos.Y;

                if (keypress.thisKey == Key.N)
                {
                    /*if (GlobalNodeHandler.viewNode.GetLastChild() != null)
                    {
                        GlobalNodeHandler.CreateNewNode(GlobalNodeHandler.viewNode.GetLastChild().xpos + GlobalNodeHandler.viewNode.GetLastChild().width + 200, 
                            GlobalNodeHandler.viewNode.GetLastChild().ypos, false);
                    }
                    else if (GlobalNodeHandler.viewNode.GetChildren().Count == 0)
                    {
                        GlobalNodeHandler.CreateNewNode(10, 1000 / 2, false);
                        //GlobalNodeHandler.ShowTextBox(new CalcPoint(10, 1000 / 2), activetextbox);
                    }*/
                }
                if (keypress.thisKey == Key.I)
                {

                    if ((GlobalNodeHandler.clickedNode = GlobalNodeHandler.viewNode.ContainsChildNode(x, y)) != null)
                    {
                        GlobalNodeHandler.JumpInto(GlobalNodeHandler.clickedNode);
                        repaint(this, new EventArgs());
                    }

                }
                if (keypress.thisKey == Key.O)
                {
                    GlobalNodeHandler.JumpOut();
                    repaint(this, new EventArgs());
                }
                if (keypress.thisKey == Key.C)
                {

                    if (GlobalNodeHandler.clickedNode == null)
                    {
                        if ((GlobalNodeHandler.clickedNode = GlobalNodeHandler.viewNode.ContainsChildNode(x, y)) != null)
                        {
                            GlobalNodeHandler.clickedNode.setSelected(true);
                        }
                    }
                    else
                    {
                        if ((GlobalNodeHandler.rightNode = GlobalNodeHandler.viewNode.ContainsChildNode(x, y)) != null && GlobalNodeHandler.rightNode != GlobalNodeHandler.clickedNode)
                        {
                            GlobalNodeHandler.clickedNode.setSelected(false);
                            GlobalNodeHandler.clickedNode.AddConnection(GlobalNodeHandler.rightNode);

                            GlobalNodeHandler.actionLog.AddAction(new MindNodeAction(3, "ConnectNodes", GlobalNodeHandler.clickedNode, GlobalNodeHandler.rightNode));

                            GlobalNodeHandler.clickedNode = null;
                            GlobalNodeHandler.rightNode = null;
                        }
                        else if (GlobalNodeHandler.clickedNode != null)
                        {
                            GlobalNodeHandler.clickedNode.setSelected(false);
                            GlobalNodeHandler.clickedNode = null;
                        }
                    }
                }
                if (keypress.thisKey == Key.D)
                {

                    if (GlobalNodeHandler.clickedNode == null)
                    {
                        if ((GlobalNodeHandler.clickedNode = GlobalNodeHandler.viewNode.ContainsChildNode(x, y)) != null)
                        {
                            GlobalNodeHandler.clickedNode.setSelected(true);
                        }
                    }
                    else
                    {
                        if ((GlobalNodeHandler.rightNode = GlobalNodeHandler.viewNode.ContainsChildNode(x, y)) != null && GlobalNodeHandler.rightNode != GlobalNodeHandler.clickedNode)
                        {
                            GlobalNodeHandler.clickedNode.setSelected(false);
                            GlobalNodeHandler.clickedNode.DeleteConnection(GlobalNodeHandler.rightNode);

                            GlobalNodeHandler.actionLog.AddAction(new MindNodeAction(4, "DeleteConnections", GlobalNodeHandler.clickedNode, GlobalNodeHandler.rightNode));

                            GlobalNodeHandler.clickedNode = null;
                            GlobalNodeHandler.rightNode = null;

                        }
                        else if (GlobalNodeHandler.clickedNode != null)
                        {
                            GlobalNodeHandler.clickedNode.setSelected(false);
                            GlobalNodeHandler.clickedNode = null;
                        }
                    }
                }
                if (keypress.thisKey == Key.A)
                {
                    if ((GlobalNodeHandler.clickedNode = GlobalNodeHandler.viewNode.ContainsChildNode(x, y)) != null)
                    {
                        MindNodeAction action = new MindNodeAction(3, "DeleteConnections");
                        action.involvedNodes.Push(GlobalNodeHandler.clickedNode);
                        foreach (MindNode m in GlobalNodeHandler.clickedNode.connNodes)
                        {
                            action.involvedNodes.Push(GlobalNodeHandler.clickedNode);
                            action.involvedNodes.Push(m);
                        }
                        foreach (MindNode m in GlobalNodeHandler.clickedNode.backconnNodes)
                        {
                            action.involvedNodes.Push(m);
                            action.involvedNodes.Push(GlobalNodeHandler.clickedNode);
                        }
                        GlobalNodeHandler.actionLog.AddAction(action);

                        GlobalNodeHandler.clickedNode.DeleteAllConnections();
                    }
                }
                if (keypress.thisKey == Key.K)
                {
                    if ((GlobalNodeHandler.clickedNode = GlobalNodeHandler.viewNode.ContainsChildNode(x, y)) != null)
                    {
                        MindNodeAction action = new MindNodeAction(3, "DeleteConnections");
                        action.involvedNodes.Push(GlobalNodeHandler.clickedNode);
                        foreach (MindNode m in GlobalNodeHandler.clickedNode.connNodes)
                        {
                            action.involvedNodes.Push(GlobalNodeHandler.clickedNode);
                            action.involvedNodes.Push(m);
                        }
                        foreach (MindNode m in GlobalNodeHandler.clickedNode.backconnNodes)
                        {
                            action.involvedNodes.Push(m);
                            action.involvedNodes.Push(GlobalNodeHandler.clickedNode);
                        }
                        GlobalNodeHandler.actionLog.AddAction(action);

                        GlobalNodeHandler.clickedNode.DeleteAllConnections();

                        GlobalNodeHandler.actionLog.AddAction(new MindNodeAction(1, "DeleteNode", GlobalNodeHandler.clickedNode));
                        GlobalNodeHandler.viewNode.DeleteNode(GlobalNodeHandler.clickedNode);
                    }
                }
                if (keypress.thisKey == Key.U)
                {
                    GlobalNodeHandler.actionLog.UndoLast();
                }
                if (keypress.thisKey == Key.R)
                {
                    GlobalNodeHandler.actionLog.RedoLast();
                }
                if (keypress.thisKey == Key.RIGHT)
                {
                    arrowdir.X = 1;
                    GlobalNodeHandler.viewNode.MoveView(arrowdir, 15);
                    //panel1.HorizontalScroll.Value = panel1.HorizontalScroll.Value + 15;
                }
                if (keypress.thisKey == Key.LEFT)
                {
                    arrowdir.X = -1;
                    GlobalNodeHandler.viewNode.MoveView(arrowdir, 15);
                    //if(panel1.HorizontalScroll.Value > 15)
                    //  panel1.HorizontalScroll.Value = panel1.HorizontalScroll.Value - 15;
                }
                if (keypress.thisKey == Key.UP)
                {
                    arrowdir.Y = -1;
                    GlobalNodeHandler.viewNode.MoveView(arrowdir, 15);
                    //panel1.VerticalScroll.Value = panel1.HorizontalScroll.Value + 15;
                }
                if (keypress.thisKey == Key.DOWN)
                {
                    arrowdir.Y = 1;
                    GlobalNodeHandler.viewNode.MoveView(arrowdir, 15);
                    //if (panel1.VerticalScroll.Value > 15)
                    //  panel1.VerticalScroll.Value = panel1.VerticalScroll.Value - 15;
                }
                repaint(this, new EventArgs());
            }
        }

        public void thisKeyUpExec(KeyObject keypress)
        {
            if (keypress.thisKey == Key.RIGHT)
            {
                arrowdir.X = 0;
            }
            if (keypress.thisKey == Key.LEFT)
            {
                arrowdir.X = 0;
            }
            if (keypress.thisKey == Key.UP)
            {
                arrowdir.Y = 0;
            }
            if (keypress.thisKey == Key.DOWN)
            {
                arrowdir.Y = 0;
            }
        }

        public void EnterTextBox()
        {
            if (!activetextbox.Equals(null) && !activetextbox.Text.Equals(""))
            {
                GlobalNodeHandler.typing = false;
                if (GlobalNodeHandler.clickedNode != null)
                {
                    // Object Binder
                    /*MindNode resultNode;
                    if ((resultNode = GlobalNodeHandler.masterNode.SearchMindNodeName(activetextbox.Text)) != null)
                    {
                        GlobalNodeHandler.clickedNode.SetText(activetextbox.Text);
                        GlobalNodeHandler.clickedNode.referenceNode = resultNode;
                    }*/

                    GlobalNodeHandler.clickedNode.SetText(activetextbox.Text);
                }
                else if (GlobalNodeHandler.clickedLabel != null)
                {
                    GlobalNodeHandler.clickedLabel.SetText(activetextbox.Text);
                }

                activetextbox.Visibility = Visibility.Collapsed;
                activetextbox.Text = "";
                GlobalNodeHandler.clickedNode = null;

                //GrabFocus();
                repaint(this, new EventArgs());
            }
        }
        
        public event EventHandler repainted;

        public void repaint(Object sender, EventArgs e)
        {
            repainted.Invoke(sender, e);
        }
        
    }

    public enum Key
    {
        NONE,
        LEFT,
        RIGHT,
        UP,
        DOWN, C, K, D,
        I, O, U, R, A, N,
        ENTER
    }

    public class KeyObject
    {
        public Key thisKey = Key.NONE;
        public CalcPoint currentMousePos = new CalcPoint(-1, -1);

        public KeyObject(Key pressedKey, CalcPoint currentMousePos)
        {
            this.thisKey = pressedKey;
            this.currentMousePos = currentMousePos;
        }

    }
}
