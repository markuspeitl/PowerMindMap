using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MindNoderPort
{
    public class MouseHandler
    {
        public CalcPoint position;
        private List<MindNode> selectedGroup = new List<MindNode>();
        private List<CalcPoint> diffPoints = new List<CalcPoint>();
        public int tickcnt = 0;

        public TextBox activetext;

        public MouseHandler(TextBox activetext)
        {
            this.activetext = activetext;
            //activetext = new TextBox();
            //activetext.SetBounds(10, 10, 50, 20);
            //activetext.Visible = true;
        }
        
        int transformrecx = 0;
        int transformrecy = 0;
        CalcPoint transformStart = new CalcPoint();
        CalcPoint NodeStart = new CalcPoint();
        int startWidth;
        int startHeight;
        int clickstartcount = 0;
        bool surpressMouseup = false;
        CalcPoint mouseStartPoint;
        CalcPoint mouseEndPoint;
        public Rect selectionRec = new Rect(0, 0, 0, 0);
        public bool dragRec;
        public bool dragView;

        bool dragging = false;
        Pointer leftpointer = null;
        Pointer rightpointer = null;


        CalcPoint transformPoint = new CalcPoint(0, 0);
        MindNode transformnode = null;
        int transformInBorder = 7;

        public async void viewPane_Pressed(MouseObject mousObj)
        {
            CalcPoint mousePos = mousObj.currentMousePos;

            if (GlobalNodeHandler.TabletSelected)
            {
                if (!GlobalNodeHandler.typing)
                {
                    if (GlobalNodeHandler.pick1.BoxContains(mousePos.ToPoint()))
                    {
                        if (GlobalNodeHandler.pick1.ChangeColorTo(mousePos.ToPoint()))
                        {
                            repaint(this, new EventArgs());
                        }
                    }
                    else
                    {
                        if (GlobalNodeHandler.transforming)
                        {
                            if ((transformnode = GlobalNodeHandler.viewNode.ContainsTRecChild(mousePos.X, mousePos.Y)) != null)
                            {
                                MindNodeAction action = new MindNodeAction(6, "Transform", transformnode);
                                transformStart = mousePos;
                                startWidth = transformnode.width;
                                startHeight = transformnode.height;
                                action.sourceSize.X = startWidth;
                                action.sourceSize.Y = startHeight;
                                action.startpoint.Push(new CalcPoint(transformnode.xpos,transformnode.ypos));
                                dragView = false;

                                NodeStart = new CalcPoint(transformnode.xpos, transformnode.ypos);

                                if (mousePos.X < transformnode.xpos + transformInBorder)
                                {
                                    transformPoint.X = -1;
                                }
                                else if (mousePos.X > transformnode.xpos + transformnode.width - transformInBorder)
                                {
                                    transformPoint.X = 1;
                                }
                                if (mousePos.Y < transformnode.ypos + transformInBorder)
                                {
                                    transformPoint.Y = -1;
                                }
                                else if (mousePos.Y > transformnode.ypos + transformnode.height - transformInBorder)
                                {
                                    transformPoint.Y = 1;
                                }
                                GlobalNodeHandler.actionLog.AddAction(action);
                            }
                        }
                        if (GlobalNodeHandler.connecting)
                        {
                            if (GlobalNodeHandler.clickedNode != null)
                                GlobalNodeHandler.clickedNode.setSelected(false);
                        }


                        if ((GlobalNodeHandler.clickedNode = GlobalNodeHandler.viewNode.ContainsChildNode(mousePos.X, mousePos.Y)) == null)
                        {
                            GlobalNodeHandler.clickedLabel = GlobalNodeHandler.viewNode.ContainsChildLabel(mousePos.X, mousePos.Y);
                            GlobalNodeHandler.rightNode = null;
                            mouseStartPoint = mousePos;
                            if (transformnode == null)
                                dragView = true;
                            clickstartcount = tickcnt;

                            if (GlobalNodeHandler.coloring)
                            {
                                if (GlobalNodeHandler.clickedLabel != null)
                                {
                                    GlobalNodeHandler.clickedLabel.NodeColor = ColorPicker.selectedcolor;
                                    GlobalNodeHandler.clickedLabel.updateRepresentation();
                                    repaint(this, new EventArgs()); ;
                                }
                            }

                            if (GlobalNodeHandler.paste)
                            {
                                await GlobalNodeHandler.settings.RestoreFromClipboard(mousePos.X, mousePos.Y);
                            }
                        }
                        else
                        {
                            if (mousObj.thisButton.Equals(MouseButton.LEFT))
                            {
                                if (GlobalNodeHandler.coloring)
                                {
                                    if (GlobalNodeHandler.clickedNode != null)
                                    {
                                        GlobalNodeHandler.clickedNode.NodeColor = ColorPicker.selectedcolor;
                                        GlobalNodeHandler.clickedNode.updateRepresentation();
                                        repaint(this, new EventArgs()); ;
                                    }
                                }
                                if (GlobalNodeHandler.jumping)
                                {
                                    GlobalNodeHandler.JumpInto(GlobalNodeHandler.clickedNode);
                                    repaint(this, new EventArgs());
                                }
                                if (GlobalNodeHandler.copy)
                                {
                                    GlobalNodeHandler.settings.CopyToClipboard(GlobalNodeHandler.clickedNode);
                                }
                                else if (GlobalNodeHandler.cut)
                                {
                                    GlobalNodeHandler.settings.CopyToClipboard(GlobalNodeHandler.clickedNode);
                                    GlobalNodeHandler.actionLog.AddAction(new MindNodeAction(1, "DeleteNode", GlobalNodeHandler.clickedNode));
                                    GlobalNodeHandler.clickedNode.DeleteAllConnections();
                                    GlobalNodeHandler.clickedNode.DeleteNode();
                                    repaint(this, new EventArgs());
                                }
                                else if (GlobalNodeHandler.connecting)
                                {
                                    if (GlobalNodeHandler.rightNode == null)
                                    {
                                        GlobalNodeHandler.rightNode = GlobalNodeHandler.clickedNode;
                                        GlobalNodeHandler.rightNode.setSelected(true);
                                    }
                                    else if (!GlobalNodeHandler.clickedNode.Equals(GlobalNodeHandler.rightNode))
                                    {
                                        GlobalNodeHandler.rightNode.setSelected(false);
                                        if(GlobalNodeHandler.rightNode.AddConnection(GlobalNodeHandler.clickedNode))
                                            GlobalNodeHandler.actionLog.AddAction(new MindNodeAction(3, "ConnectNodes", GlobalNodeHandler.rightNode, GlobalNodeHandler.clickedNode));
                                        GlobalNodeHandler.rightNode.UpdateBridgesRepresentation();
                                        GlobalNodeHandler.clickedNode = null;
                                        GlobalNodeHandler.rightNode = null;
                                    }
                                    repaint(this, new EventArgs());
                                }
                                else if (GlobalNodeHandler.disconnecting)
                                {
                                    if (GlobalNodeHandler.rightNode == null)
                                    {
                                        GlobalNodeHandler.rightNode = GlobalNodeHandler.clickedNode;
                                        GlobalNodeHandler.rightNode.setSelected(true);
                                    }
                                    else
                                    {
                                        GlobalNodeHandler.rightNode.setSelected(false);

                                        if (GlobalNodeHandler.rightNode.connNodes.Contains(GlobalNodeHandler.clickedNode))
                                        {
                                            GlobalNodeHandler.actionLog.AddAction(new MindNodeAction(4, "DeleteConnections", GlobalNodeHandler.rightNode, GlobalNodeHandler.clickedNode));
                                        }
                                        else
                                        {
                                            GlobalNodeHandler.actionLog.AddAction(new MindNodeAction(4, "DeleteConnections", GlobalNodeHandler.clickedNode, GlobalNodeHandler.rightNode));
                                        }

                                        GlobalNodeHandler.rightNode.DeleteConnection(GlobalNodeHandler.clickedNode);
                                        GlobalNodeHandler.clickedNode.DeleteConnection(GlobalNodeHandler.rightNode);
                                        
                                        
                                        GlobalNodeHandler.clickedNode = null;
                                        GlobalNodeHandler.rightNode = null;
                                    }
                                    repaint(this, new EventArgs());
                                }
                                else if (GlobalNodeHandler.deleting)
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
                                    repaint(this, new EventArgs());
                                }
                                else if (GlobalNodeHandler.moving)
                                {
                                    if (selectedGroup.Count != 0)
                                    {
                                        MindNodeAction moveaction = new MindNodeAction(2, "MoveNodes");
                                        foreach (MindNode m in selectedGroup)
                                        {
                                            diffPoints.Add(new CalcPoint(m.xpos - GlobalNodeHandler.clickedNode.xpos, m.ypos - GlobalNodeHandler.clickedNode.ypos));
                                            moveaction.startpoint.Push(new CalcPoint(m.xpos, m.ypos));
                                            moveaction.involvedNodes.Push(m);
                                        }
                                        GlobalNodeHandler.actionLog.AddAction(moveaction);
                                    }
                                    else {

                                        MindNodeAction moveaction = new MindNodeAction(2, "MoveNodes", GlobalNodeHandler.clickedNode);
                                        moveaction.startpoint.Push(new CalcPoint(GlobalNodeHandler.clickedNode.xpos, GlobalNodeHandler.clickedNode.ypos));
                                        GlobalNodeHandler.actionLog.AddAction(moveaction);
                                    }
                                    dragging = true;
                                }
                            }
                        }
                        if (GlobalNodeHandler.placelabel)
                        {
                            if (GlobalNodeHandler.clickedNode != null)
                            {
                                activetext.Text = GlobalNodeHandler.clickedNode.text;
                                GlobalNodeHandler.ShowTextBox(mousObj.currentMousePos, activetext);
                                GlobalNodeHandler.actionLog.AddAction(new MindNodeAction(5, "ChangeText", GlobalNodeHandler.clickedNode, GlobalNodeHandler.clickedNode.text));
                                GlobalNodeHandler.typing = true;
                            }
                            else if (GlobalNodeHandler.clickedLabel != null)
                            {
                                activetext.Text = GlobalNodeHandler.clickedLabel.GetText();
                                GlobalNodeHandler.ShowTextBox(mousObj.currentMousePos, activetext);
                                GlobalNodeHandler.actionLog.AddAction(new MindNodeAction(5, "ChangeText", GlobalNodeHandler.clickedLabel, GlobalNodeHandler.clickedLabel.GetText()));
                                GlobalNodeHandler.typing = true;
                            }
                        }
                    }
                }
            }
            else if (GlobalNodeHandler.mouseSelected)
            {
                viewPane_MouseDown(mousObj);
            }

        }

        public void viewPane_Released(MouseObject mousObj)
        {
            CalcPoint mousePos = mousObj.currentMousePos;

            if (GlobalNodeHandler.TabletSelected)
            {
                if (GlobalNodeHandler.adding && tickcnt - clickstartcount < 10)
                {
                    if (GlobalNodeHandler.clickedNode == null && GlobalNodeHandler.clickedLabel == null)
                    {
                        if (GlobalNodeHandler.CreateNewNode(mousePos.X, mousePos.Y, true))
                        {
                            CalcPoint CreatePoint;
                            CreatePoint = new CalcPoint(mousePos.X - GlobalNodeHandler.clickedNode.width,
                                mousePos.Y - GlobalNodeHandler.clickedNode.height);

                            GlobalNodeHandler.ShowTextBox(CreatePoint, activetext);
                            repaint(this, new EventArgs());
                        }
                    }
                }
                if (dragging)
                {
                    dragging = false;
                    int x = mousePos.X;
                    int y = mousePos.Y;
                    if (selectedGroup.Count != 0)
                    {
                        diffPoints = new List<CalcPoint>();
                    }
                    else if (GlobalNodeHandler.clickedNode != null)
                    {
                        GlobalNodeHandler.clickedNode.SetPosition(mousePos.X, mousePos.Y, true);
                        GlobalNodeHandler.clickedNode.setHovered(false);
                        GlobalNodeHandler.clickedNode = null;
                        repaint(this, new EventArgs());
                    }
                }
                if (transformPoint.X != 0 || transformPoint.Y != 0)
                {
                    transformPoint = new CalcPoint();
                    transformnode.SetScaled(true);
                    transformnode.UpdatePivots();
                    transformnode.UpdateTransformRec();
                    transformnode.updateRepresentation();
                    repaint(this, new EventArgs());
                    transformnode = null;
                }
                if (dragView)
                {
                    dragView = false;
                    repaint(this, new EventArgs());
                }

            }
            else if (GlobalNodeHandler.mouseSelected)
            {
                viewPane_MouseUp(mousObj);
            }
        }

        public void viewPane_MouseDown(MouseObject mousObj)
        {
            CalcPoint mousePos = mousObj.currentMousePos;

            if (GlobalNodeHandler.pick1.BoxContains(mousePos.ToPoint()) && GlobalNodeHandler.pick1.isVisible)
            {
                if (GlobalNodeHandler.pick1.ChangeColorTo(mousePos.ToPoint()))
                {
                    if (GlobalNodeHandler.typing)
                    {
                        if (GlobalNodeHandler.clickedNode != null)
                            GlobalNodeHandler.clickedNode.TextColor = ColorPicker.selectedcolor;
                        if (GlobalNodeHandler.clickedLabel != null)
                            GlobalNodeHandler.clickedLabel.TextColor = ColorPicker.selectedcolor;
                    }

                    repaint(this, new EventArgs());
                }
            }
            else {

                if (mousObj.thisButton.Equals(MouseButton.LEFT))
                {
                    clickstartcount = tickcnt;

                    if ((GlobalNodeHandler.clickedNode = GlobalNodeHandler.viewNode.ContainsChildNode(mousePos.X, mousePos.Y)) == null)
                    {
                        surpressMouseup = false;
                        if (selectedGroup.Count != 0)
                        {
                            foreach (MindNode m in selectedGroup)
                            {
                                m.setHovered(false);
                            }
                            selectedGroup = new List<MindNode>();
                            diffPoints = new List<CalcPoint>();
                            surpressMouseup = true;
                        }

                        mouseStartPoint = mousePos;
                        dragRec = true;
                    }
                }
                if (mousObj.thisButton.Equals(MouseButton.MIDDLE))
                {
                    if ((GlobalNodeHandler.clickedNode = GlobalNodeHandler.viewNode.ContainsChildNode(mousePos.X, mousePos.Y)) != null)
                    {
                        MainPage.status2 += "Middle Chosen" + "\n";
                        GlobalNodeHandler.clickedNode.NodeColor = ColorPicker.selectedcolor;
                        GlobalNodeHandler.clickedNode.updateRepresentation();
                        repaint(this,new EventArgs());;
                    }
                    else if ((GlobalNodeHandler.clickedLabel = GlobalNodeHandler.viewNode.ContainsChildLabel(mousePos.X, mousePos.Y)) != null)
                    {
                        GlobalNodeHandler.clickedLabel.NodeColor = ColorPicker.selectedcolor;
                        GlobalNodeHandler.clickedLabel.updateRepresentation();
                        repaint(this,new EventArgs());;
                    }

                }
                if (mousObj.thisButton.Equals(MouseButton.RIGHT))
                {
                    if ((GlobalNodeHandler.clickedNode = GlobalNodeHandler.viewNode.ContainsChildNode(mousePos.X, mousePos.Y)) != null)
                    {
                        MainPage.status2 += "IsRight CLicked" + "\n";
                        MindNodeAction action = new MindNodeAction(GlobalNodeHandler.clickedNode);
                        if (mousePos.X < GlobalNodeHandler.clickedNode.xpos + 5)
                        {
                            transformrecx = -1;
                            transformStart = mousePos;
                            startWidth = GlobalNodeHandler.clickedNode.width;
                            NodeStart = new CalcPoint(GlobalNodeHandler.clickedNode.xpos, GlobalNodeHandler.clickedNode.ypos);
                            action.sourceSize.X = startWidth;
                            action.id = 6;
                            action.name = "Transform";
                            GlobalNodeHandler.actionLog.AddAction(action);
                        }
                        else if (mousePos.X > GlobalNodeHandler.clickedNode.xpos + GlobalNodeHandler.clickedNode.width - 5)
                        {
                            transformrecx = 1;
                            transformStart = mousePos;
                            startWidth = GlobalNodeHandler.clickedNode.width;
                            NodeStart = new CalcPoint(GlobalNodeHandler.clickedNode.xpos, GlobalNodeHandler.clickedNode.ypos);
                            action.sourceSize.X = startWidth;
                            action.id = 6;
                            action.name = "Transform";
                            GlobalNodeHandler.actionLog.AddAction(action);
                        }
                        else if (mousePos.Y < GlobalNodeHandler.clickedNode.ypos + 5)
                        {
                            transformrecy = -1;
                            transformStart = mousePos;
                            startHeight = GlobalNodeHandler.clickedNode.height;
                            NodeStart = new CalcPoint(GlobalNodeHandler.clickedNode.xpos, GlobalNodeHandler.clickedNode.ypos);
                            action.sourceSize.Y = startHeight;
                            action.id = 6;
                            action.name = "Transform";
                            GlobalNodeHandler.actionLog.AddAction(action);
                        }
                        else if (mousePos.Y > GlobalNodeHandler.clickedNode.ypos + GlobalNodeHandler.clickedNode.height - 5)
                        {
                            transformrecy = 1;
                            transformStart = mousePos;
                            startHeight = GlobalNodeHandler.clickedNode.height;
                            NodeStart = new CalcPoint(GlobalNodeHandler.clickedNode.xpos, GlobalNodeHandler.clickedNode.ypos);
                            action.sourceSize.Y = startHeight;
                            action.id = 6;
                            action.name = "Transform";
                            GlobalNodeHandler.actionLog.AddAction(action);
                        }
                        else {

                            if (GlobalNodeHandler.clickedNode.getSelected())
                            {
                                GlobalNodeHandler.clickedNode.setHovered(false);
                            }

                            if (selectedGroup.Count != 0)
                            {
                                MindNodeAction moveaction = new MindNodeAction(2, "MoveNodes");

                                foreach (MindNode m in selectedGroup)
                                {
                                    diffPoints.Add(new CalcPoint(m.xpos - GlobalNodeHandler.clickedNode.xpos, m.ypos - GlobalNodeHandler.clickedNode.ypos));

                                    moveaction.startpoint.Push(new CalcPoint(m.xpos, m.ypos));
                                    moveaction.involvedNodes.Push(m);

                                }
                                GlobalNodeHandler.actionLog.AddAction(moveaction);

                            }
                            else {

                                MindNodeAction moveaction = new MindNodeAction(2, "MoveNodes", GlobalNodeHandler.clickedNode);
                                moveaction.startpoint.Push(new CalcPoint(GlobalNodeHandler.clickedNode.xpos, GlobalNodeHandler.clickedNode.ypos));
                                GlobalNodeHandler.actionLog.AddAction(moveaction);
                            }
                            dragging = true;
                        }

                    }
                    else
                    {
                        mouseStartPoint = mousePos;
                        dragView = true;
                    }
                }
            }
        }
        
        public void viewPane_MouseUp(MouseObject mousObj)
        {
            CalcPoint mousePos = mousObj.currentMousePos;

            //MainPage.status2 += "Released"+"" + ptrPt.Properties.IsRightButtonPressed.ToString();

            if (mousObj.thisButton.Equals(MouseButton.LEFT))
            {
                if (tickcnt - clickstartcount < 10 && !surpressMouseup)
                {
                    surpressMouseup = false;
                    //System.out.println("mouseclick");
                    if (!GlobalNodeHandler.typing)
                    {

                        if ((GlobalNodeHandler.clickedNode = GlobalNodeHandler.viewNode.ContainsChildNode(mousePos.X, mousePos.Y)) == null)
                        {
                            if (!dragView)
                            {
                                if ((GlobalNodeHandler.clickedLabel = GlobalNodeHandler.viewNode.ContainsChildLabel(mousePos.X, mousePos.Y)) != null)
                                {
                                    activetext.Visibility = Visibility.Visible;
                                    activetext.Margin = new Thickness(currentFormPosition.X, currentFormPosition.Y, 0, 0);
                                    activetext.Text = GlobalNodeHandler.clickedLabel.GetText();
                                    GlobalNodeHandler.actionLog.AddAction(new MindNodeAction(5, "ChangeText", GlobalNodeHandler.clickedLabel, GlobalNodeHandler.clickedLabel.GetText()));
                                    GlobalNodeHandler.typing = true;
                                }
                                else {
                                    if (GlobalNodeHandler.CreateNewNode(mousePos.X, mousePos.Y, true))
                                    {
                                        CalcPoint CreatePoint = new CalcPoint();

                                        if (currentFormPosition != null)
                                        {
                                            CreatePoint = new CalcPoint(currentFormPosition.X - GlobalNodeHandler.clickedNode.width,
                                                currentFormPosition.Y - GlobalNodeHandler.clickedNode.height);
                                        }

                                        GlobalNodeHandler.ShowTextBox(CreatePoint, activetext);
                                        repaint(this, new EventArgs());
                                    }
                                }
                            }
                        }
                        else {
                            activetext.Visibility = Visibility.Visible;
                            activetext.Margin = new Thickness(currentFormPosition.X, currentFormPosition.Y, 0, 0);
                            activetext.Text = GlobalNodeHandler.clickedNode.text;
                            GlobalNodeHandler.actionLog.AddAction(new MindNodeAction(5, "ChangeText", GlobalNodeHandler.clickedNode, GlobalNodeHandler.clickedNode.text));

                            GlobalNodeHandler.typing = true;
                        }
                        
                    }
                    if (dragRec)
                    {
                        mouseEndPoint = mousePos;
                        dragRec = false;

                        selectedGroup = GlobalNodeHandler.viewNode.ChildrenInRec(selectionRec);
                        foreach (MindNode m in selectedGroup)
                        {
                            m.setHovered(true);
                        }

                        selectionRec = new Rect(0, 0, 0, 0);
                        repaint(this, new EventArgs());
                    }
                    else if (dragView)
                    {
                        dragView = false;
                        repaint(this, new EventArgs());
                    }

                }
            }
            if (mousObj.thisButton.Equals(MouseButton.RIGHT))
            {
                if (transformrecx != 0 || transformrecy != 0)
                {
                    if (transformrecx != 0)
                        transformrecx = 0;
                    if (transformrecy != 0)
                        transformrecy = 0;
                    GlobalNodeHandler.clickedNode.SetScaled(true);
                    GlobalNodeHandler.clickedNode.updateRepresentation();
                    repaint(this, new EventArgs());
                }
                if (dragging)
                {
                    dragging = false;
                    int x = mousePos.X;
                    int y = mousePos.Y;

                    if (selectedGroup.Count != 0)
                    {
                        diffPoints = new List<CalcPoint>();
                    }
                    else if (GlobalNodeHandler.clickedNode != null)
                    {
                        GlobalNodeHandler.clickedNode.SetPosition(x, y, true);
                        GlobalNodeHandler.clickedNode.setHovered(false);
                        GlobalNodeHandler.clickedNode = null;
                        repaint(this, new EventArgs());
                    }
                }
                else if (dragView)
                {
                    dragView = false;
                    repaint(this, new EventArgs());
                }
                    
            }

        }

        MindNode tempNode = null;
        bool hoveractive = false;
        MindNode changedNode = null;
        int index = -1;

        public CalcPoint currentViewPanePosition;
        public CalcPoint currentFormPosition;

        public void timer1_Tick(object sender, object e)
        {
            int x = currentViewPanePosition.X;
            int y = currentViewPanePosition.Y;
            tickcnt++;
            if (GlobalNodeHandler.typing)
            {
                FocusText(this, new EventArgs());
            }
            if (tickcnt - MainPage.startcnt > 10 && MainPage.scrolling)
            {
                MainPage.scrollcnt = 0;
                MainPage.scrolling = false;
                GlobalNodeHandler.viewNode.UpdateViewRepresentation();
                repaint(this,new EventArgs());
            }
            if (transformPoint.X != 0)
            {
                transformnode.width = startWidth + transformPoint.X * (x - transformStart.X);

                if (transformnode.width < 10)
                {
                    transformnode.width = 10;
                }
                else if (transformPoint.X == -1)
                {
                    transformnode.xpos = NodeStart.X - (transformStart.X - x);
                }
                repaint(this,new EventArgs());
            }
            if (transformPoint.Y != 0)
            {
                transformnode.height = startHeight + transformPoint.Y * (y - transformStart.Y);
                if (transformnode.height < 10)
                {
                    transformnode.height = 10;
                }
                else if (transformPoint.Y == -1)
                {
                    transformnode.ypos = NodeStart.Y - (transformStart.Y - y);
                }
                repaint(this,new EventArgs());
            }
            if (selectedGroup.Count == 0 && !dragging)
            {
                if (tempNode == null)
                {
                    tempNode = GlobalNodeHandler.viewNode.ContainsChildNode(x, y);
                    if (tempNode != null)
                    {
                        MainPage.status2 += "Hovered over child" + "\n";
                    }
                }
                else
                {
                    if (!hoveractive)
                    {
                        MainPage.status2 += "Hovered is true" + "\n";
                        tempNode.setHovered(true);
                        repaint(this,new EventArgs());
                    }
                    hoveractive = true;
                    if (!tempNode.Contains(x, y))
                    {
                        hoveractive = false;
                        tempNode.setHovered(false);
                        tempNode = null;
                        repaint(this,new EventArgs());
                    }
                }

            }
            else if (dragging)
            {

                if (selectedGroup.Count == 0)
                {
                    GlobalNodeHandler.clickedNode.SetPosition(x, y, true);
                    GlobalNodeHandler.clickedNode.UpdatePivots();
                    GlobalNodeHandler.clickedNode.UpdateBridgesRepresentation();
                    GlobalNodeHandler.clickedNode.UpdateBridgesRepresentationInward();
                    GlobalNodeHandler.clickedNode.setHovered(true);
                }
                else
                {
                    GlobalNodeHandler.clickedNode.SetPosition(x, y, true);
                    GlobalNodeHandler.clickedNode.UpdatePivots();
                    GlobalNodeHandler.clickedNode.setHovered(true);
                    if (selectedGroup.Contains(GlobalNodeHandler.clickedNode))
                    {
                        for (int i = 0; i < diffPoints.Count; i++)
                        {
                            selectedGroup.ElementAt(i).SetPosition(GlobalNodeHandler.clickedNode.xpos + diffPoints.ElementAt(i).X, GlobalNodeHandler.clickedNode.ypos + diffPoints.ElementAt(i).Y, false);
                            selectedGroup.ElementAt(i).UpdatePivots();
                        }
                    }
                    else if (diffPoints.Count != 0)
                    {
                        foreach (MindNode m in selectedGroup)
                        {
                            m.setHovered(false);
                        }
                        selectedGroup = new List<MindNode>();
                        diffPoints = new List<CalcPoint>();
                    }
                }

                repaint(this,new EventArgs());
            }
            if (dragRec)
            {
                mouseEndPoint = new CalcPoint(x,y);
                if (mouseStartPoint.X < mouseEndPoint.X && mouseStartPoint.Y < mouseEndPoint.Y)
                    selectionRec = new Rect(mouseStartPoint.X, mouseStartPoint.Y, mouseEndPoint.X - mouseStartPoint.X, mouseEndPoint.Y - mouseStartPoint.Y);
                else if (mouseStartPoint.X > mouseEndPoint.X && mouseStartPoint.Y > mouseEndPoint.Y)
                    selectionRec = new Rect(mouseEndPoint.X, mouseEndPoint.Y, mouseStartPoint.X - mouseEndPoint.X, mouseStartPoint.Y - mouseEndPoint.Y);
                else if (mouseStartPoint.X > mouseEndPoint.X && mouseStartPoint.Y < mouseEndPoint.Y)
                    selectionRec = new Rect(mouseEndPoint.X, mouseStartPoint.Y, mouseStartPoint.X - mouseEndPoint.X, mouseEndPoint.Y - mouseStartPoint.Y);
                else if (mouseStartPoint.X < mouseEndPoint.X && mouseStartPoint.Y > mouseEndPoint.Y)
                    selectionRec = new Rect(mouseStartPoint.X, mouseEndPoint.Y, mouseEndPoint.X - mouseStartPoint.X, mouseStartPoint.Y - mouseEndPoint.Y);

                repaint(this,new EventArgs());;
            }
            else if (dragView)
            {
                mouseEndPoint = new CalcPoint(x, y);
                CalcPoint relative = new CalcPoint(mouseStartPoint.X - mouseEndPoint.X, mouseStartPoint.Y - mouseEndPoint.Y);
                mouseStartPoint = mouseEndPoint;

                GlobalNodeHandler.viewNode.MoveViewRelativePix(relative);
                repaint(this,new EventArgs());
            }
        }

        public event EventHandler focusText;
        public event EventHandler repainted;

        public void repaint(Object sender, EventArgs e)
        {
            repainted.Invoke(sender,e);
        }

        public void FocusText(Object sender, EventArgs e)
        {
            activetext.Focus(FocusState.Programmatic);
            //activetext.SelectionStart = activetext.Text.Length;
        }

    }
    public enum MouseButton
    {
        NONE,
        LEFT,
        RIGHT,
        MIDDLE
    }

    public class MouseObject
    {
        public MouseButton thisButton = MouseButton.NONE;
        public CalcPoint currentMousePos = new CalcPoint(-1, -1);

        public MouseObject(MouseButton pressedButton, CalcPoint currentMousePos)
        {
            this.thisButton = pressedButton;
            this.currentMousePos = currentMousePos;
        }

    }
}
