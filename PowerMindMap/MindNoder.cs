using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Core;
using Windows.UI.Xaml.Input;

namespace MindNoderPort
{
    public class MindNoder
    {
        /*int id = 0;
        public static MindNode masterNode;
        public static MindNode viewNode;
        private TextBox activetext = new TextBox();
        public static MindNode clickedNode;
        public static NodeLabel clickedLabel;
        public static MindNode rightNode;
        private List<MindNode> selectedGroup = new List<MindNode>();
        private List<Point> diffPoints = new List<Point>();
        public static ActionStack actionLog = new ActionStack();

        //ColorPicker pick1 = new ColorPicker(4, 4);


        Point mouseStartPoint;
        Point mouseEndPoint;

        static Rect selectionRec = new Rect(0,0,0,0);
        static bool dragRec;
        static bool dragView;

        bool dragging = false;
        
        private int windowheight = 500;
        private int windowwidth = 1000;
        MouseHandler mouse;*/

        //ScrollBar vScrollBar1 = new VScrollBar();
        //ScrollBar hScrollBar1 = new HScrollBar();


        //private static readonly ILog log2 = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //public MindNoder()
        //{
            /*InitializeComponent();

            
            vScrollBar1.Dock = DockStyle.Right;
            vScrollBar1.Scroll += (sender, e) => { panel1.VerticalScroll.Value = vScrollBar1.Value; };
            panel1.Controls.Add(vScrollBar1);

            hScrollBar1.Dock = DockStyle.Bottom;
            hScrollBar1.Scroll += (sender, e) => { panel1.HorizontalScroll.Value = hScrollBar1.Value; };
            panel1.Controls.Add(hScrollBar1);

            log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("C:\\Users\\Markus\\Documents\\Visual Studio 2015\\Projects\\MindNoder1\\MindNoder1\\App.config"));
            log2.Info("Test");*/
            
            /*masterNode = new MindNode(id,0, 0, 0, 0,false);
            id++;
            masterNode.text = "masterNode";

            viewNode = masterNode;

            activetext.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            activetext.KeyDown += new KeyEventHandler(activeTextfield_KeyDown);

            this.KeyDown += new KeyEventHandler(this_KeyDown);
            this.KeyUp += new KeyEventHandler(this_KeyUp);
            

            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            this.timer1.Interval = (1000/120);
            


            this.MouseWheel += this.OnMouseScroll;
            //viewPane.MouseWheel += this.OnMouseScroll;

            mouse = new MouseHandler(activetext);
            mouse.repainted += this.repaint;
            viewPane.MouseDown += mouse.viewPane_MouseDown;
            viewPane.MouseUp += mouse.viewPane_MouseUp;
            this.timer1.Tick += mouse.timer1_Tick;

            colorPicker.Image = pick1.repaint(colorPicker.Height, colorPicker.Width);
            colorPicker.MouseDown += pick1.colorPicker_MouseDown;
            colorPicker.MouseDown += this.RepaintColorPicker;

            repaint(this, new EventArgs());

            Random r = new Random();
            for (int i = 0; i < 100; i++)
            {
                mouse.CreateNewNode((int)(r.NextDouble() * 2000), (int)(r.NextDouble() * 1000), true);
                mouse.activetext.Text = "Node:" + i;
                EnterTextBox();
            }*//*

        }

        public void RepaintColorPicker(object sender, EventArgs e)
        {
            //colorPicker.Image = pick1.repaint(colorPicker.Height, colorPicker.Width);
        }

        public void activeTextfield_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                EnterTextBox();
            }
        }

        public Point GetCursorPosition()
        {
            return viewPane.PointToClient(Cursor.Position);
        }
        

        public void EnterTextBox()
        {
            mouse.typing = false;
            if (!mouse.activetext.Text.Equals(null) && !mouse.activetext.Text.Equals(""))
            {
                if (clickedNode != null)
                {
                    MindNode resultNode;
                    if((resultNode = masterNode.SearchMindNodeName(mouse.activetext.Text))!= null)
                    {
                        clickedNode.SetText(activetext.Text);
                        clickedNode.referenceNode = resultNode;
                    }
                    clickedNode.SetText(mouse.activetext.Text);
                }
                else if (mouse.clickedLabel != null)
                {
                    mouse.clickedLabel.SetText(mouse.activetext.Text);
                }

                mouse.activetext.Visible = false;
                mouse.activetext.Text = "";
                clickedNode = null;
                
                GrabFocus();
                repaint(this,new EventArgs());
            }
        }
        
        public void repaint(object sender, EventArgs e)
        {
         
        }

        public void GrabFocus()
        {
            this.Focus();
        }

        public void JumpInto(MindNode jumpNode)
        {
            if (jumpNode.jumpable) {
                if (jumpNode.referenceNode == null)
                {
                    viewNode = jumpNode;
                }
                else
                {
                    viewNode = jumpNode.referenceNode;
                }
                repaint(this,new EventArgs());;
            }
        }

        public void JumpOut()
        {
            //Resets the viewnode to master node
            //System.out.println("Try Jump out of:" + viewNode.text);
            //System.out.println("Parent:" + viewNode.parent);
            if (viewNode.parent != null)
            {
                viewNode = viewNode.parent;
                repaint(this,new EventArgs());;
            }
        }

        Point arrowdir = new Point(0, 0);
        public void this_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (!mouse.typing)
            {
                int x = viewPane.PointToClient(Cursor.Position).X;
                int y = viewPane.PointToClient(Cursor.Position).Y;

                log2.Info("Key Fired");

                if (e.KeyCode == Keys.N)
                {
                    if (viewNode.GetLastChild() != null)
                    {
                        mouse.CreateNewNode(viewNode.GetLastChild().xpos + viewNode.GetLastChild().width + 200, viewNode.GetLastChild().ypos, false);
                    }
                    else if (viewNode.GetChildren().Count == 0)
                    {
                        mouse.CreateNewNode(10, windowheight / 2, false);
                    }
                }
                if (e.KeyCode == Keys.I)
                {

                    if ((clickedNode = viewNode.ContainsChildNode(x, y)) != null)
                    {
                        JumpInto(clickedNode);
                        clickedNode.setHovered(false);
                        repaint(this, new EventArgs());
                    }

                }
                if (e.KeyCode == Keys.O)
                {
                    JumpOut();
                }
                if (e.KeyCode == Keys.C)
                {

                    if (clickedNode == null)
                    {
                        if ((clickedNode = viewNode.ContainsChildNode(x, y)) != null)
                        {
                            clickedNode.setSelected(true);
                            repaint(this, new EventArgs());
                        }
                    }
                    else
                    {
                        if ((rightNode = viewNode.ContainsChildNode(x, y)) != null && rightNode != clickedNode)
                        {
                            clickedNode.setSelected(false);
                            clickedNode.AddConnection(rightNode);

                            actionLog.AddAction(new MindNodeAction(3, "ConnectNodes", clickedNode, rightNode));

                            clickedNode = null;
                            rightNode = null;

                        }
                        else if (clickedNode != null)
                        {
                            clickedNode.setSelected(false);
                            clickedNode = null;
                        }

                        repaint(this, new EventArgs()); ;
                    }
                }
                if (e.KeyCode == Keys.D)
                {

                    if (clickedNode == null)
                    {
                        if ((clickedNode = viewNode.ContainsChildNode(x, y)) != null)
                        {
                            clickedNode.setSelected(true);
                            repaint(this, new EventArgs()); ;
                        }
                    }
                    else
                    {
                        if ((rightNode = viewNode.ContainsChildNode(x, y)) != null && rightNode != clickedNode)
                        {
                            clickedNode.setSelected(false);
                            clickedNode.DeleteConnection(rightNode);

                            actionLog.AddAction(new MindNodeAction(4, "DeleteConnections", clickedNode, rightNode));

                            clickedNode = null;
                            rightNode = null;

                        }
                        else if (clickedNode != null)
                        {
                            clickedNode.setSelected(false);
                            clickedNode = null;
                        }

                        repaint(this, new EventArgs()); ;
                    }
                }
                if (e.KeyCode == Keys.A)
                {
                    if ((clickedNode = viewNode.ContainsChildNode(x, y)) != null)
                    {
                        clickedNode.DeleteAllConnections();

                        repaint(this, new EventArgs()); ;
                    }
                }
                if (e.KeyCode == Keys.K)
                {
                    if ((clickedNode = viewNode.ContainsChildNode(x, y)) != null)
                    {
                        actionLog.AddAction(new MindNodeAction(1, "DeleteNode", clickedNode));

                        clickedNode.DeleteAllConnections();
                        viewNode.DeleteNode(clickedNode);

                        repaint(this, new EventArgs()); ;
                    }
                }
                if (e.KeyCode == Keys.U)
                {
                    actionLog.UndoLast();
                    repaint(this, new EventArgs()); ;
                }
                if (e.KeyCode == Keys.R)
                {
                    actionLog.RedoLast();
                    repaint(this, new EventArgs()); ;
                }
                if (e.KeyCode == Keys.Right)
                {
                    arrowdir.X = 1;
                    viewNode.MoveView(arrowdir, 15);
                    //panel1.HorizontalScroll.Value = panel1.HorizontalScroll.Value + 15;
                    repaint(this, new EventArgs()); ;
                }
                if (e.KeyCode == Keys.Left)
                {
                    arrowdir.X = -1;
                    viewNode.MoveView(arrowdir, 15);
                    //if(panel1.HorizontalScroll.Value > 15)
                    //  panel1.HorizontalScroll.Value = panel1.HorizontalScroll.Value - 15;
                    repaint(this, new EventArgs()); ;
                }
                if (e.KeyCode == Keys.Up)
                {
                    arrowdir.Y = -1;
                    viewNode.MoveView(arrowdir, 15);
                    //panel1.VerticalScroll.Value = panel1.HorizontalScroll.Value + 15;
                    repaint(this, new EventArgs()); ;
                }
                if (e.KeyCode == Keys.Down)
                {
                    arrowdir.Y = 1;
                    viewNode.MoveView(arrowdir, 15);
                    //if (panel1.VerticalScroll.Value > 15)
                    //  panel1.VerticalScroll.Value = panel1.VerticalScroll.Value - 15;
                    repaint(this, new EventArgs()); ;
                }

            }

        }

        public void this_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Right)
            {
                arrowdir.X = 0;
            }
            if (e.Key == Windows.System.VirtualKey.Left)
            {
                arrowdir.X = 0;
            }
            if (e.Key == Windows.System.VirtualKey.Up)
            {
                arrowdir.Y = 0;
            }
            if (e.Key == Windows.System.VirtualKey.Down)
            {
                arrowdir.Y = 0;
            }
        }

        private void viewPane_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void viewPane_MouseUp(object sender, MouseEventArgs e)
        {
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            int x = viewPane.PointToClient(Cursor.Position).X;
            int y = viewPane.PointToClient(Cursor.Position).Y;
            mouse.currentViewPanePosition = viewPane.PointToClient(Cursor.Position);
            mouse.currentFormPosition = this.PointToClient(Cursor.Position);
        }

        public double zoom = 1.0;
        public static int scrollcnt = 0;
        public static int startcnt = 0;
        public static bool scrolling = false;
        public void OnMouseScroll(object sender, MouseEventArgs e)
        {
            startcnt = mouse.tickcnt;
            scrolling = true;

            scrollcnt++;
            if (scrollcnt % 6 == 0)
            {
                zoom = mouse.Zoom((this.viewPane.PointToClient(Cursor.Position)), zoom, e.Delta);
                repaint(this,new EventArgs());;
            }
            else
            {
                zoom = mouse.PseudoZoom((this.viewPane.PointToClient(Cursor.Position)), zoom, e.Delta);
                repaint(this,new EventArgs());;
            }
        }

        private Settings settings = new Settings();
        private void saveMap_Click(object sender, EventArgs e)
        {
            //settings.saveMap();
        }
        private void saveAs_Click(object sender, EventArgs e)
        {
            //settings.saveMapAs();
        }


        private void openMap_Click(object sender, EventArgs e)
        {
            //settings.openMap();
            repaint(this, new EventArgs());
        }
        private void newMap_Click(object sender, EventArgs e)
        {
            //settings.newMap();
            repaint(this,new EventArgs());
        }
        private void ExitMindNoder_Click(object sender, EventArgs e)
        {
            //this.Close();
        }
        private void exportMap_Click(object sender, EventArgs e)
        {
            //viewNode.MakeViewPositive(20);
            //settings.exportMap(viewNode.GetMinMaxSpanX(), viewNode.GetMinMaxSpanX(), viewPane.Image);
        }
        private void MapStatistics_Click(object sender, EventArgs e)
        {
            //settings.ShowMapStatistics();
        }

        private void closeMap_Click(object sender, EventArgs e)
        {

        }

        private void printMap_Click(object sender, EventArgs e)
        {

        }
        
        private void StepBack_Click(object sender, EventArgs e)
        {
            MindNoder.actionLog.UndoLast();
        }

        private void RedoStep_Click(object sender, EventArgs e)
        {
            MindNoder.actionLog.RedoLast();
        }

        private void cutMenü_Click(object sender, EventArgs e)
        {
            if (selectedGroup.Count == 1)
            {
                settings.CopyToClipboard(selectedGroup[0]);
                selectedGroup[0].DeleteNode();
            }
            repaint(this, new EventArgs());
        }

        private void pasteMenü_Click(object sender, EventArgs e)
        {
            MindNode node = settings.RestoreFromClipboard();
            node.SetPosition(viewPane.Width / 2, viewPane.Height / 2, true);
            viewNode.AddChild(node);
        }

        private void createNewNodeMenü_Click(object sender, EventArgs e)
        {

        }

        private void transformNodeMenü_Click(object sender, EventArgs e)
        {

        }

        private void deleteNodeMenü_Click(object sender, EventArgs e)
        {

        }*/
    }
}
