using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MindNoderPort
{
    public class GlobalNodeHandler
    {
        public static int id = 0;
        public static MindNode masterNode;
        public static MindNode viewNode;
        public static MindNode clickedNode;
        public static NodeLabel clickedLabel;
        public static MindNode rightNode;
        public static List<MindNode> selectedGroup = new List<MindNode>();
        public static List<Point> diffPoints = new List<Point>();
        public static PowerMindMap.ActionStackInterface actionLog = new ActionStack();

        public static Settings settings = new Settings();

        public static ColorPicker pick1;

        public static bool dragRec;
        public static bool dragView;
        public static bool dragging = false;
        public static bool typing = false;

        public static Color background = Colors.White;

        public static bool mouseSelected = false;
        public static bool TabletSelected = false;

        public static bool adding = false;
        public static bool connecting = false;
        public static bool disconnecting = false;
        public static bool deleting = false;
        public static bool transforming = false;
        public static bool moving = false;
        public static bool selecting = false;
        public static bool jumping = false;
        public static bool bridgecontrol = true;
        public static bool lighttext = false;
        public static Button selectedButton = Button.NONE;

        public static bool undo = false;
        public static bool redo = false;
        public static bool copy = false;
        public static bool cut = false;
        public static bool paste = false;
        public static bool placelabel = false;
        public static bool placefreeink = false;
        public static bool coloring = false;

        public static bool textbold = false;
        public static bool textitalic = false;
        public static bool textunderlined = false;

        public static CanvasTextFormat standardtextFormat;

        public static DrawingStyle nodeStyle = DrawingStyle.ELLIPSE;

        public static int globalfontsize = 15;

        public static Button lastbutton = Button.NONE;

        public static double zoom = 1.0;

        public static void ResetSelected()
        {
            if (clickedNode != null)
            {
                //clickedNode.setHovered(false);
                //clickedNode.setSelected(false);
                clickedNode = null;
            }
            clickedLabel = null;
            rightNode = null;
        }

        public static void ResetButtons()
        {
            adding = false;
            connecting = false;
            disconnecting = false;
            deleting = false;
            transforming = false;
            moving = false;
            selecting = false;
            selectedButton = Button.NONE;

            undo = false;
            redo = false;
            copy = false;
            cut = false;
            paste = false;
            placelabel = false;
            placefreeink = false;
            coloring = false;
            ResetSelected();
        }

        public static void ResetSelection()
        {
            if(rightNode != null)
                rightNode.setSelected(false);
            if (clickedNode != null)
                clickedNode.setSelected(false);
            rightNode = null;
        }

        public static void JumpInto(MindNode jumpNode)
        {
            if (jumpNode.jumpable)
            {
                if (jumpNode.referenceNode == null)
                {
                    GlobalNodeHandler.viewNode = jumpNode;
                }
                else
                {
                    GlobalNodeHandler.viewNode = jumpNode.referenceNode;
                }
            }
            if (clickedNode != null)
            {
                GlobalNodeHandler.clickedNode.setHovered(false);
            }
        }

        public static void JumpOut()
        {
            //Resets the GlobalNodeHandler.viewNode to master node
            //System.out.println("Try Jump out of:" + GlobalNodeHandler.viewNode.text);
            //System.out.println("Parent:" + GlobalNodeHandler.viewNode.parent);
            if (GlobalNodeHandler.viewNode.parent != null)
            {
                GlobalNodeHandler.viewNode = GlobalNodeHandler.viewNode.parent;
            }
        }

        public static bool CreateNewNode(int x, int y, bool mousepivot)
        {
            MindNode newnode = new MindNode(0, x, y, 40, 40, mousepivot);

            if (viewNode.AddChild(newnode,false))
            {
                newnode.SetNodeScale(zoom);

                newnode.NodeColor = ColorPicker.selectedcolor;

                actionLog.AddAction(new MindNodeAction(0, "CreateNode", newnode));
                typing = true;
                clickedNode = newnode;

                return true;
            }
            return false;
        }
        
        public static void Zoom(CalcPoint p, double delta)
        {
            GlobalNodeHandler.zoom *= ((double)(delta * 0.001d) + 1);
            //log2.Info("Wheelfired:" + ((double)(e.Delta* 0.002d) + 1));
            if (GlobalNodeHandler.zoom >= 0.4)
            {
                GlobalNodeHandler.masterNode.SetNodeScale(GlobalNodeHandler.zoom);
                //log2.Info(zoom);
                GlobalNodeHandler.viewNode.ExpandOnAnchor(p, ((double)(delta * 0.001d) + 1));
            }
            else
            {
                GlobalNodeHandler.zoom = 0.4d;
            }
        }

        public static void PseudoZoom(CalcPoint p, double delta)
        {
            GlobalNodeHandler.zoom *= ((double)(delta * 0.001d) + 1);
            //log2.Info("Wheelfired:" + ((double)(e.Delta* 0.002d) + 1));
            if (GlobalNodeHandler.zoom >= 0.4)
            {
                GlobalNodeHandler.masterNode.PseudoScale(GlobalNodeHandler.zoom);
                //log2.Info(zoom);
                GlobalNodeHandler.viewNode.ExpandOnAnchor(p, ((double)(delta * 0.001d) + 1));
            }
            else
            {
                GlobalNodeHandler.zoom = 0.4d;
            }
        }

        public static void ShowTextBox(CalcPoint position, TextBox box)
        {
            box.Visibility = Windows.UI.Xaml.Visibility.Visible;

            box.Margin = new Thickness(position.X, position.Y, 0, 0);

            if (GlobalNodeHandler.clickedNode != null)
                box.Text = GlobalNodeHandler.clickedNode.text;

            box.Focus(FocusState.Programmatic);
            box.SelectionStart = box.Text.Length;
        }


    }
}
