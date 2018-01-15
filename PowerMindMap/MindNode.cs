using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.UI;

namespace MindNoderPort
{
    public class MindNode
    {

        public int id = -1;
        public String text = "";
        public Point textpos;
        public Point textoffset;
        private Size textSize = new Size(0,0);

        public Color TextColor = Colors.Black;
        public Color NodeColor = Colors.Red;
        public Color BorderColor = Colors.Black;

        public DrawingStyle NodeDrawingStyle = DrawingStyle.ELLIPSE;
        private CanvasTextFormat NodeTextStyle;
        private int fontsize = 15;

        public int borderOffset = 40;
        
        private bool hovered = false;
        private bool selected = false;
        public bool jumpable = true;
        
        private bool scaled = false;

        Rect boxrect;
        public int xpos;
        public int ypos;
        public int height;
        public int width;

        public Rect transformRec;
        public int transformBorder = 15;

        List<MindNode> childNodes = new List<MindNode>();
        public MindNode parent = null;

        public List<MindNode> backconnNodes = new List<MindNode>();
        public List<MindNode> connNodes = new List<MindNode>();
        private List<Bridge> connections;

        private float heightmod = 1.0f;
        public double scale = 1.0;

        public MindNode referenceNode = null;

        public MindNode(int id, int x, int y, int width, int height, bool mousepivot)
        {
            NodeTextStyle = new CanvasTextFormat();
            text = "";
            this.id = id;
            this.width =(int) (width * scale);
            this.height = (int)(height * scale);

            //NodeTextStyle.FontFamily = "Times New Roman";
            //NodeTextStyle.FontSize= 12;

            //fontsize = 12;

            textoffset = new Point(10,0);
            connections = new List<Bridge>();
            SetPosition(x, y, mousepivot);
            UpdateWidth();

            transformRec = new Rect(xpos - transformBorder, ypos - transformBorder, width + 2 * transformBorder, height + 2 * transformBorder);

            updateRepresentation();
        }

        public void setHovered(bool active)
        {
            if (active != hovered)
            {
                hovered = active;
                updateRepresentation();
            }
        }

        int basewidth = 0;
        int baseheight = 0;
        public void SetScaled(bool scaledactive)
        {
            scaled = scaledactive;
            basewidth = width;
            baseheight = height;
        }

        public void setSelected(bool active)
        {
            if (active != selected)
            {
                selected = active;
                updateRepresentation();
            }
        }
        public bool getSelected()
        {
            return selected;
        }

        public void SetTextStyle(CanvasTextFormat newTextformat)
        {
            int newFontsize = (int)(newTextformat.FontSize);
            
            this.NodeTextStyle.FontSize = (int)(newFontsize * scale);
            this.NodeTextStyle.FontFamily = newTextformat.FontFamily;
            this.NodeTextStyle.FontStyle = newTextformat.FontStyle;
            this.NodeTextStyle.FontWeight = newTextformat.FontWeight;

            this.NodeTextStyle.WordWrapping = CanvasWordWrapping.NoWrap;
            fontsize = newFontsize;
        }

        public void SetHeightMod(float mod)
        {
            heightmod = mod;
            this.height = (int)(height * mod);
        }

        public void UpdateViewRepresentation()
        {
            if(this.width >0 && this.height > 0)
            {
                this.UpdatePivots();
                this.updateRepresentation();
            }

            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                childNodes[i].UpdateViewRepresentation();
            }
        }

        public void SetText(String newtext)
        {
            text = newtext;
            UpdateWidth();
        }

        public MindNode GetExistingNode(int id)
        {
            if(this.id == id)
            {
                return this;
            }

            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                if (childNodes[i].GetExistingNode(id) != null)
                    return childNodes[i].GetExistingNode(id);
            }

            return null;
        }
    
        public void SetNodeScale(double ratio)
        {
            if (scale >= 0.4)
            {
                this.scale = ratio;
                if (ratio > 100)
                {
                    this.scale = 100;
                }

                NodeTextStyle.FontSize = (int)(fontsize * scale);

                for (int i = connections.Count - 1; i >= 0; i--)
                {
                    connections[i].textNode.SetNodeScale(ratio);
                }

                for (int i = childNodes.Count - 1; i >= 0; i--)
                {
                    childNodes[i].SetNodeScale(ratio);
                }
                UpdateWidth();
            }
        }

        public void PseudoScale(double ratio)
        {
            if (scale >= 0.4)
            {
                this.scale = ratio;
                //this.width = (int)(width * scale);
                //this.height = (int)(height * scale);
                NodeTextStyle.FontSize = (int)(fontsize * scale);

                for (int i = connections.Count - 1; i >= 0; i--)
                {
                    connections[i].textNode.PseudoScale(ratio);
                }

                for (int i = childNodes.Count - 1; i >= 0; i--)
                {
                    childNodes[i].PseudoScale(ratio);
                }
            }

            if (width > 0 && height > 0)
            {
                if (text.Length > 2)
                {
                    textSize = CreateTextSize();
                    if (!scaled)
                    {
                        width = (int)(textSize.Width + 40 * scale);
                        height = (int)(textSize.Height + 40*scale * fontsize/14);
                    }
                    else
                    {
                        width = (int)(basewidth * scale);
                        height = (int)(baseheight * scale);
                    }
                    UpdatePivots();
                }
                boxrect = new Rect(xpos, ypos, height, width);
                textpos = new Point((int)((width - textSize.Width)/2), (int)((height - textSize.Height) / 2));
                transformRec = new Rect(xpos - transformBorder, ypos - transformBorder, width + 2 * transformBorder, height + 2 * transformBorder);
            }
        }

        public void SetRelativeNodeScale(double ratio)
        {
            this.scale *= ratio;
            NodeTextStyle.FontSize = (int)(fontsize * scale);
            UpdateWidth();

            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                childNodes[i].SetNodeScale(ratio);
            }
        }

        int xoffset = 25;
        int yoffset = 0;
        public void ExpandOnAnchor(CalcPoint Anchor, double factor)
        {
            //factor = 1.0;
            Point diff;
            for (int i = childNodes.Count - 1; i >= 0; i--) {
                if (childNodes[i].width > 0 && childNodes[i].height > 0)
                {
                    diff = new Point(Anchor.X+xoffset - childNodes[i].GetCenter().X, Anchor.Y+yoffset - childNodes[i].GetCenter().Y);
                    diff.X = (int)(diff.X * factor - diff.X);
                    diff.Y = (int)(diff.Y * factor - diff.Y);
                    childNodes[i].SetPosition((int)(childNodes[i].xpos- diff.X), (int)(childNodes[i].ypos - diff.Y),false);

                    for (int x = childNodes[i].connections.Count - 1; x >= 0; x--)
                    {
                        childNodes[i].UpdatePivots();
                        childNodes[i].UpdatePosition();
                    }
                }
            }
            
        }

        public void MoveView(Point direction, double factor)
        {
            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                if (childNodes[i].width > 0 && childNodes[i].height > 0)
                {
                    childNodes[i].SetPosition(childNodes[i].xpos - (int)(direction.X * factor), childNodes[i].ypos - (int)(direction.Y * factor), false);

                    for (int x = childNodes[i].connections.Count - 1; x >= 0; x--)
                    {
                        childNodes[i].UpdatePivots();
                        childNodes[i].UpdatePosition();
                    }
                }
            }
        }

        public void MoveViewRelativePix(CalcPoint direction)
        {
            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                if (childNodes[i].width > 0 && childNodes[i].height > 0)
                {
                    childNodes[i].SetPosition((int)(childNodes[i].xpos - direction.X), (int)(childNodes[i].ypos - direction.Y), false);

                    childNodes[i].UpdatePosition();
                    for (int x = childNodes[i].connections.Count - 1; x >= 0; x--)
                    {
                        childNodes[i].connections[x].MoveBoundBox(direction);
                        childNodes[i].connections[x].textNode.MoveBoundBox(direction);
                    }
                }
            }
        }

        public int GetNodeCount()
        {
            int cnt = 0;
            cnt++;
            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                cnt += childNodes[i].GetNodeCount(); 
            }

            return cnt;
        }

        public List<MindNode> ChildrenInRec(Rect rec)
        {
            List<MindNode> selChilds = new List<MindNode>();
            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                if (rec.Contains(new Point(childNodes[i].xpos, childNodes[i].ypos)))
                {
                    selChilds.Add(childNodes[i]);
                }
            }
            return selChilds;
        }

        public String GetRepresentationText()
        {
            String output = "";

            output += "ID:" + this.id + "\n";
            output += "TextPos:" + this.textpos.X + "/" + this.textpos.Y + "\n";
            output += "TextFormat:" + this.NodeTextStyle.FontSize + "/" + this.NodeTextStyle.FontFamily + "/" + this.NodeTextStyle.FontStyle + "/" + this.NodeTextStyle.FontWeight + "/" + TextColor.ToString() + "\n";
            output += "Name:" + this.text + "\n";
            output += "NodeStyle:" +this.NodeDrawingStyle.ToString() + "\n";
            output += "Color:" + this.TextColor.ToString() +"/"+ this.BorderColor.ToString() + "/" + this.NodeColor.ToString() + "\n";
            output += "Bounds:" + this.xpos +"/" + this.ypos + "/" + this.width + "/" + this.height + "\n";
            if(scaled)
                output += "Scaled:"+ "\n";
            output += "ConnNodes:";
            foreach (MindNode m in connNodes)
            {
                output += m.id + "/";
            }
            output += "\n";
            output += "ConnColors:";
            foreach (Bridge b in connections)
            {
                output += b.textNode.NodeColor.ToString() + "/";
            }
            output += "\n";
            output += "ConnNames:";
            foreach (Bridge b in connections)
            {
                output += b.textNode.GetText() + "/";
            }
            output += "\n";
            output += "Childs:";
            foreach (MindNode m in childNodes)
            {
                output += m.id + "/";
            }
            output += "\n";

            foreach (MindNode m in childNodes)
            {
                output += m.GetRepresentationText();
            }

            return output;
        }

        public System.Xml.Linq.XElement GetChildrenRepresentationX()
        {
            XElement children = new XElement("CHILDRENLIST");
            foreach (MindNode m in childNodes)
            {
                children.Add(m.GetRepresentationX());
            }
            return children;
        }

        public System.Xml.Linq.XElement GetConnectionsX()
        {
            //BACKNODECONNID
            XElement conns = new XElement("CONNECTIONSLIST");
            for (int i = 0; i < this.connNodes.Count; i++)
            {
                conns.Add(new XElement("CONNECTION",
                              new XElement("CONNNODECOLOR", this.connections[i].textNode.NodeColor.ToString()),
                              new XElement("CONNNODETEXT", this.connections[i].textNode.GetText()),
                              new XElement("NODECONNID", this.connNodes[i].id)));
            }

            return conns;
        }

        public System.Xml.Linq.XElement GetBackConnectionsX()
        {
            XElement conns = new XElement("BACKCONNECTIONSLIST");
            for (int i = 0; i < this.connNodes.Count; i++)
            {
                conns.Add(new XElement("BACKNODECONNID",this.backconnNodes[i].id));
            }

            return conns;
        }

        public void FromRepresentation(String noderepresentation)
        {
            FromRepresentation(XDocument.Parse(noderepresentation));
        }

        //Restores the Project from an XContainer Representation
        public void FromRepresentation(XContainer noderepresentation)
        {
            // Creates an Containers of all PROJECT ChildNodes
            XContainer container = noderepresentation.Element("NODE");
            if (container == null)
            {
                container = noderepresentation;
            }

            try
            {

                //System.Diagnostics.Debug.WriteLine(container.ToString());

                Int32.TryParse(container.Element("ID").Value, out this.id);

                double textpos = 0;
                double.TryParse(container.Element("TEXTPOSX").Value, out textpos);
                this.textpos.X = textpos;
                double.TryParse(container.Element("TEXTPOSY").Value, out textpos);
                this.textpos.Y = textpos;

                float fontsize = 0;
                float.TryParse(container.Element("TEXTSIZE").Value, out fontsize);
                this.NodeTextStyle.FontSize = fontsize;

                this.NodeTextStyle.FontFamily = container.Element("TEXTFONT").Value;

                String fontstyle = container.Element("TEXTSTYLE").Value;
                if (fontstyle.Equals("Italic"))
                    this.NodeTextStyle.FontStyle = Windows.UI.Text.FontStyle.Italic;
                else
                    this.NodeTextStyle.FontStyle = Windows.UI.Text.FontStyle.Normal;

                String fontweight = container.Element("TEXTFONT").Value;
                if (fontweight.Equals("Bold"))
                    this.NodeTextStyle.FontWeight = Windows.UI.Text.FontWeights.Bold;
                else
                    this.NodeTextStyle.FontWeight = Windows.UI.Text.FontWeights.Normal;

                this.TextColor = FileManager.GetColorFromHexString(container.Element("TEXTCOLOR").Value);

                this.text = container.Element("NODETEXT").Value;

                String nodestyle = container.Element("NODESTYLE").Value;
                this.NodeDrawingStyle = DrawingStyle.ELLIPSE;
                if ((nodestyle).Equals(DrawingStyle.BUTTON.ToString()))
                    this.NodeDrawingStyle = DrawingStyle.BUTTON;
                if ((nodestyle).Equals(DrawingStyle.CIRCLE.ToString()))
                    this.NodeDrawingStyle = DrawingStyle.CIRCLE;
                if ((nodestyle).Equals(DrawingStyle.ELLIPSEEDGE.ToString()))
                    this.NodeDrawingStyle = DrawingStyle.ELLIPSEEDGE;
                if ((nodestyle).Equals(DrawingStyle.RECTANGLE.ToString()))
                    this.NodeDrawingStyle = DrawingStyle.RECTANGLE;

                this.BorderColor = FileManager.GetColorFromHexString(container.Element("COLORBORDER").Value);
                this.NodeColor = FileManager.GetColorFromHexString(container.Element("COLORNODE").Value);

                Int32.TryParse(container.Element("NODEBOUNDX").Value, out this.xpos);
                Int32.TryParse(container.Element("NODEBOUNDY").Value, out this.ypos);
                Int32.TryParse(container.Element("NODEBOUNDWIDTH").Value, out this.width);
                Int32.TryParse(container.Element("NODEBOUNDHEIGHT").Value, out this.height);

                Boolean.TryParse(container.Element("ISSCALED").Value, out this.scaled);
                this.SetScaled(this.scaled);

                childNodes.Clear();
                //Restore all child nodes
                foreach (XElement xe in container.Element("CHILDRENLIST").Descendants("NODE"))
                {
                    //if (xe.Parent.Parent.Parent.Name != "PROJECTTEMPLATE")
                    MindNode ch = new MindNode(0,0,0,0,0,false);
                    ch.FromRepresentation(xe);
                    childNodes.Add(ch);
                    //ch.RestoreConnections(xe);
                }
                this.connNodes.Clear();
                this.connections.Clear();
                // Only Restore Connections after all Nodes were added to Masternode, because otherwise the nodes could not be found
                for (int i = 0; i < this.childNodes.Count; i++)
                {
                    if (i < container.Element("CHILDRENLIST").Descendants("NODE").Count())
                    {
                        this.childNodes[i].RestoreConnections(container.Element("CHILDRENLIST").Descendants("NODE").ElementAt(i));
                    }
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("There has been an error restoring Project XML file:" + e.Message);
            }
        }
        public void RestoreConnections(XContainer noderepresentation)
        {
            XContainer container = noderepresentation.Element("NODE");
            if (container == null)
            {
                container = noderepresentation;
            }

            foreach (XElement xe in container.Element("CONNECTIONSLIST").Descendants("CONNECTION"))
            {
                Color connColor = FileManager.GetColorFromHexString(xe.Element("CONNNODECOLOR").Value);
                String connText = xe.Element("CONNNODETEXT").Value;
                int connNodeId = -1;
                Int32.TryParse(xe.Element("NODECONNID").Value, out connNodeId);
                MindNode connNode = GlobalNodeHandler.masterNode.GetExistingNode(connNodeId);
                if(connNode != null)
                    this.AddConnection(connNode, connColor, connText);
            }
        }

        public System.Xml.Linq.XElement GetRepresentationX()
        {

            XElement node = new XElement(
                        new XElement("NODE",
                            new XElement("ID", this.id),
                            new XElement("TEXTPOSX", this.textpos.X),
                            new XElement("TEXTPOSY", this.textpos.Y),
                            new XElement("TEXTSIZE", this.NodeTextStyle.FontSize),
                            new XElement("TEXTFONT", this.NodeTextStyle.FontFamily),
                            new XElement("TEXTSTYLE", this.NodeTextStyle.FontStyle),
                            new XElement("TEXTWEIGHT", this.NodeTextStyle.FontWeight),
                            new XElement("TEXTCOLOR", this.TextColor.ToString()),
                            new XElement("NODETEXT", this.text),
                            new XElement("NODESTYLE", this.NodeDrawingStyle.ToString()),
                            new XElement("COLORTEXT", this.TextColor.ToString()),
                            new XElement("COLORBORDER", this.BorderColor.ToString()),
                            new XElement("COLORNODE", this.NodeColor.ToString()),
                            new XElement("NODEBOUNDX", this.xpos),
                            new XElement("NODEBOUNDY", this.ypos),
                            new XElement("NODEBOUNDWIDTH", this.width),
                            new XElement("NODEBOUNDHEIGHT", this.height),
                            new XElement("ISSCALED", this.scaled),
                            GetChildrenRepresentationX(),
                            GetConnectionsX()//,
                            //GetBackConnectionsX()
                        )
                    );

            return node;
            
        }
        public String GetRepresentationXString()
        {
            return this.GetRepresentationX().ToString();
        }

        double prevscale = 0.0;
        public void UpdateWidth()
        {
            if (width >0 && height > 0)
            {
                if (text.Length > 2)
                {
                    textSize = CreateTextSize();
                    if (!scaled)
                    {
                        width = (int)(textSize.Width + 40 * scale);
                        height = (int)(textSize.Height + 40 * scale * fontsize / 14);
                        if (this.NodeDrawingStyle.Equals(DrawingStyle.CIRCLE))
                        {
                            height = width;
                        }
                        CreateTextSize();
                    }
                    else
                    {
                        width = (int)(basewidth * scale);
                        height = (int)(baseheight * scale);
                    }
                    UpdatePivots();
                    UpdateBridgesRepresentation();
                    UpdateBridgesRepresentationInward();
                }
                boxrect = new Rect(xpos, ypos, height, width);
                textpos = new Point((int)((width - textSize.Width) / 2), (int)((height - textSize.Height) / 2));
                transformRec = new Rect(xpos - transformBorder, ypos - transformBorder, width + 2 * transformBorder, height + 2 * transformBorder);
                updateRepresentation();
            }

        }

        public void UpdateAllWidths()
        {
            this.UpdateWidth();

            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                childNodes[i].UpdateAllWidths();
            }
        }

        public void UpdateTransformRec()
        {
            if (width > 0 && height > 0)
                transformRec = new Rect(xpos - transformBorder, ypos - transformBorder, width + 2 * transformBorder, height + 2 * transformBorder);
        }

        public void UpdatePosition()
        {
            boxrect = new Rect(xpos, ypos, height, width);
            textpos = new Point(xpos + (int)((width - textSize.Width) / 2), ypos + (int)((height - textSize.Height) / 2));
            transformRec = new Rect(xpos - transformBorder, ypos - transformBorder, width + 2 * transformBorder, height + 2 * transformBorder);
        }

        public void Center()
        {
            xpos -= width / 2;
            ypos -= height / 2;
        }

        public CalcPoint GetCenter()
        {
            CalcPoint center = new CalcPoint();
            center.X = this.xpos + this.width / 2;
            center.Y = this.ypos + this.height / 2;
            return center;
        }

        public MindNode GetLastChild()
        {
            if(childNodes.Count >0)
                return childNodes.Last();

            return null;
        }

        public bool AddChild(MindNode node, bool virtualCreate)
        {
            if (!CheckCollision(node) || virtualCreate)
            {
                if (!virtualCreate)
                {
                    node.id = GlobalNodeHandler.id;
                    //if(!GlobalNodeHandler.copy && !GlobalNodeHandler.paste) does not change nodestyle while copying or pasting

                    node.NodeDrawingStyle = GlobalNodeHandler.nodeStyle;

                    if (GlobalNodeHandler.lighttext)
                        node.TextColor = Colors.White;

                    node.SetTextStyle(GlobalNodeHandler.standardtextFormat);
                }
                if (GetLastChild() != null)
                {
                     //GetLastChild().AddConnection(node);
                    //GetLastChild().UpdateBridgesRepresentation();
                }
                node.parent = this;
                childNodes.Add(node);

                if (!virtualCreate)
                    GlobalNodeHandler.id++;

                return true;
            }
            return false;
        }

        public bool AddChildNoStyle(MindNode node, bool virtualCreate)
        {
            if (!CheckCollision(node) || virtualCreate)
            {
                if (!virtualCreate)
                {
                    node.id = GlobalNodeHandler.id;
                    //if(!GlobalNodeHandler.copy && !GlobalNodeHandler.paste) does not change nodestyle while copying or pasting
                }
                if (GetLastChild() != null)
                {
                    //GetLastChild().AddConnection(node);
                    //GetLastChild().UpdateBridgesRepresentation();
                }
                node.parent = this;
                childNodes.Add(node);

                if (!virtualCreate)
                    GlobalNodeHandler.id++;

                return true;
            }
            return false;
        }


        public void DeleteNode()
        {
            if(parent != null)
                this.parent.childNodes.Remove(this);
        }

        public bool AddConnection(MindNode node2)
        {
            if (!this.connNodes.Contains(node2) && !node2.backconnNodes.Contains(this) && !node2.connNodes.Contains(this) && !this.backconnNodes.Contains(node2))
            {
                Bridge b = GetPivotPoints(this, node2);
                b.textNode.UpdateWidth();

                this.connections.Add(b);
                //connNodes could be redundant
                this.connNodes.Add(node2);

                node2.backconnNodes.Add(this);

                return true;
            }  

            return false;
        }
        public bool AddConnection(MindNode node2,Color c, String bridgetext)
        {
            if (!this.connNodes.Contains(node2) && !node2.backconnNodes.Contains(this) && !node2.connNodes.Contains(this) && !this.backconnNodes.Contains(node2))
            {
                this.UpdateWidth();
                Bridge b = GetPivotPoints(this, node2);
                b.textNode.NodeColor = c;
                b.textNode.SetText(bridgetext);
                b.textNode.UpdateWidth();
                this.connections.Add(b);
                //connNodes could be redundant
                this.connNodes.Add(node2);

                node2.backconnNodes.Add(this);

                return true;
            }

            return false;
        }

        public void DeleteConnection(MindNode node2)
        {
            if (this.connNodes.Contains(node2))
            {
                int index = this.connNodes.IndexOf(node2);
                this.connNodes.RemoveAt(index);
                this.connections.RemoveAt(index);
            }
            else if (node2.connNodes.Contains(this))
            {
                int index = node2.connNodes.IndexOf(this);
                node2.connNodes.RemoveAt(index);
                node2.connections.RemoveAt(index);
            }
            if (this.backconnNodes.Contains(node2))
            {
                int index = this.backconnNodes.IndexOf(node2);
                this.backconnNodes.RemoveAt(index);
            }
            else if (node2.backconnNodes.Contains(this))
            {
                int index = node2.backconnNodes.IndexOf(this);
                node2.backconnNodes.RemoveAt(index);
            }
        }
        public void DeleteAllConnections()
        {
            DeleteOutWardConnections();
            DeleteInWardConnections();
        }
        public void DeleteOutWardConnections()
        {
            this.connections = new List<Bridge>();
            this.connNodes = new List<MindNode>();
        }
        public void DeleteInWardConnections()
        {
            for (int i = backconnNodes.Count - 1; i >= 0; i--)
            {
                if (backconnNodes[i].connNodes.Contains(this))
                {
                    int index = backconnNodes[i].connNodes.IndexOf(this);
                    backconnNodes[i].connNodes.RemoveAt(index);
                    backconnNodes[i].connections.RemoveAt(index);
                }
            }
        }
        public void DeleteNode(MindNode node)
        {
            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                if (node.Equals(childNodes[i]))
                {
                    childNodes.RemoveAt(i);
                }
            }
        }
        
        public MindNode SearchMindNodeName(String name)
        {
            if(this.text.Equals(name))
            {
                return this;
            }
            MindNode resultNode;
            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                if((resultNode = childNodes[i].SearchMindNodeName(name)) != null)
                    return resultNode;
            }

            return null;
        }

        public void UpdatePivotPoints()
        {
            for (int i = connections.Count - 1; i >= 0; i--)
            {
                Bridge b = GetPivotPoints(this, connNodes[i]);
                int xpos2 = (int)b.textNode.boundBox.X;
                int ypos2 = (int)b.textNode.boundBox.Y;

                connections[i].CopyExt(b);
                
            }
        }

        public void UpdateSinglePivotLine(int index)
        {
            if (connNodes.Count > index && index != -1) {
            Bridge b = GetPivotPoints(this, connNodes[index]);
            int xpos2 = (int)b.textNode.boundBox.X;
            int ypos2 = (int)b.textNode.boundBox.Y;

            connections[index].CopyExt(b);
            }

        }

        public void UpdateBackPivotPoints()
        {
            for (int i = backconnNodes.Count - 1; i >= 0; i--)
            {
                backconnNodes[i].UpdateSinglePivotLine(backconnNodes[i].connNodes.IndexOf(this));
            }
        }

        public void UpdatePivots()
        {
            UpdatePivotPoints();
            UpdateBackPivotPoints();
        }

        public void UpdateChildPivots()
        {
            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                childNodes[i].UpdatePivots();
            }
        }

        public Bridge GetPivotPoints(MindNode node1, MindNode node2)
        {
            Point directionmdifier1 = (new Bridge(node1.GetCenter(), node2.GetCenter(),this.NodeColor)).PointDirection();
            Point directionmdifier2 = (new Bridge(node2.GetCenter(), node1.GetCenter(), this.NodeColor)).PointDirection();
            CalcPoint startpoint = new CalcPoint(node1.GetCenter().X + node1.width * directionmdifier1.X / 2,
                    node1.GetCenter().Y + node1.height * directionmdifier1.Y / 2);
            CalcPoint endpoint = new CalcPoint(node2.GetCenter().X + node2.width * directionmdifier2.X / 2,
                    node2.GetCenter().Y + node2.height * directionmdifier2.Y / 2);

            return new Bridge(startpoint, endpoint, this.NodeColor);
        }

        public bool CheckCollision(MindNode childnode)
        {
            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                if (childnode.collides(childNodes[i]) || childNodes[i].collides(childnode))
                {
                    return true;
                }
            }
            return false;
        }

        public bool collides(MindNode childnode)
        {

            if (new Rect(xpos, ypos, width, height).Contains(new Point(childnode.xpos, childnode.ypos)))
            {
                return true;
            }
            if (new Rect(xpos, ypos, width, height).Contains(new Point(childnode.xpos, childnode.ypos + childnode.height)))
            {
                return true;
            }
            if (new Rect(xpos, ypos, width, height).Contains(new Point(childnode.xpos + childnode.width, childnode.ypos)))
            {
                return true;
            }
            if (new Rect(xpos, ypos, width, height).Contains(new Point(childnode.xpos + childnode.width, childnode.ypos + childnode.height)))
            {
                return true;
            }
            return false;
        }

        public List<MindNode> GetChildren()
        {
            return childNodes;
        }

        public MindNode ContainsNode(int x, int y)
        {
            if (Contains(x, y))
            {
                return this;
            }
            return null;
        }

        public MindNode ContainsTRecNode(int x, int y)
        {
            if (transformRec.Contains(new Point(x,y)))
            {
                return this;
            }
            return null;
        }

        public bool ContainsTRec(int x, int y)
        {
            return transformRec.Contains(new Point(x, y));
        }

        public MindNode ContainsTRecChild(int x, int y)
        {
            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                if (childNodes[i].ContainsTRec(x, y))
                {
                    return childNodes[i];
                }
            }

            return null;
        }

        public NodeLabel ContainsChildLabel(int x, int y)
        {
            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                foreach (Bridge b in childNodes[i].connections)
                {
                    if (b.textNode.Contains(x, y))
                    {
                        return b.textNode;
                    }
                }
            }

            return null;
        }

        public void ReIDChilds()
        {
            for (int i = childNodes.Count - 1; i >= 0; i--) {
                childNodes[i].id = GlobalNodeHandler.id++;
            }
        }

        public MindNode ContainsChildNode(int x, int y)
        {
            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                if (childNodes[i].Contains(x, y))
                {
                    return childNodes[i];
                }

                foreach (Bridge b in childNodes[i].connections)
                {
                    if (b.textNode.Contains(x, y))
                    {
                        //return b.textNode;
                    }
                }
            }

            return null;
        }

        public bool Contains(int x, int y)
        {
            if(height > 0 && width > 0)
                return (new Rect(xpos, ypos, width, height).Contains(new Point(x,y)));

            return false;
        }

        public void SetPosition(int x, int y, bool mousePivot)
        {
            if (mousePivot)
            {
                xpos = x - (int)((width / 2));
                ypos = y - (int)((height / 2));
            }
            if (!mousePivot)
            {
                xpos = x;
                ypos = y;
            }

            UpdatePosition();
        }

        private NodeStyler style = new NodeStyler();

        public int CountNodes()
        {
            int cnt = 1;
            for (int i = 0; i < childNodes.Count; i++)
            {
                cnt += childNodes[i].CountNodes();
            }
            return cnt;
        }

        public int CountViewNodes()
        {
            return childNodes.Count;
        }

        public int CountConns()
        {
            int cnt = 0;
            for (int i = 0; i < childNodes.Count; i++)
            {
                cnt += childNodes[i].CountConns();
            }
            for (int i = 0; i < connections.Count; i++)
            {
                cnt += connections.Count;
            }
            return cnt;
        }

        public MindNode GetMinChildX()
        {
            return GetMinMaxChild(true, true);
        }

        public MindNode GetMaxChildX()
        {
            return GetMinMaxChild(false, true);
        }

        public MindNode GetMinChildY()
        {
            return GetMinMaxChild(true, false);
        }

        public MindNode GetMaxChildY()
        {
            return GetMinMaxChild(false, false);
        }

        private MindNode GetMinMaxChild(bool isMin,bool isXAxis)
        {
            MindNode minmaxNode = null;

            if (childNodes.Count > 0)
            {
                minmaxNode = childNodes[0];

                for (int i = 1; i < childNodes.Count; i++)
                {
                    if (isXAxis)
                    {
                        if (isMin)
                        {
                            if (minmaxNode.xpos > childNodes[i].xpos)
                            {
                                minmaxNode = childNodes[i];
                            }
                        }
                        else
                        {
                            if (minmaxNode.xpos < childNodes[i].xpos)
                            {
                                minmaxNode = childNodes[i];
                            }
                        }
                    }
                    else
                    {
                        if (isMin)
                        {
                            if (minmaxNode.ypos > childNodes[i].ypos)
                            {
                                minmaxNode = childNodes[i];
                            }
                        }
                        else
                        {
                            if (minmaxNode.ypos < childNodes[i].ypos)
                            {
                                minmaxNode = childNodes[i];
                            }
                        }
                    }
                }
            }

            return minmaxNode;
        }

        public int GetMinMaxSpanX()
        {
            int spanx = -1;
            MindNode left = GetMinChildX();
            MindNode right = GetMaxChildX();
            if(left != null && right != null)
            {
                spanx = right.xpos + right.width - left.xpos;
            }

            return spanx;
        }

        public int GetMinMaxSpanY()
        {
            int spany = -1;
            MindNode up = GetMinChildY();
            MindNode down = GetMaxChildY();
            if (up != null && down != null)
            {
                spany = down.ypos + down.height - up.ypos;
            }

            return spany;
        }

        public void Centerview(MindNode node,int imgHeight, int imgWidth)
        {
            Point centerPos = new Point(imgWidth / 2, imgHeight / 2);
            Point move = node.GetDiff(centerPos);
            this.MoveView(move, 1.0);
        }

        public Point GetDiff(Point reference)
        {
            return new Point(this.xpos - reference.X, this.ypos - reference.Y);
        }

        public void MakeViewPositive(int offset)
        {
            MindNode minx = GetMinChildX();
            MindNode miny = GetMinChildY();

            if (minx != null && miny != null)
            {
                Point moving = new Point(0 - minx.xpos, 0- miny.ypos);
                if(moving.X > 0)
                {
                    moving.X += offset;
                }
                if (moving.Y > 0)
                {
                    moving.Y += offset;
                }

                this.MoveView(moving, 1.0);
            }
        }




        public void drawView(CanvasControl sender,CanvasDrawingSession g2d,int xoffset, int yoffset)
        {
            for (int i = 0; i < childNodes.Count; i++)
            {
                //if (childNodes[i].repupdated)
                //{

                //Task.Run(() =>
                //{
                for (int x = 0 ; x < childNodes[i].connections.Count; x++)
                {
                    childNodes[i].connections[x].drawBridge(sender,g2d, xoffset, yoffset);
                }

                if (childNodes[i].represent == null)
                {
                    childNodes[i].updateRepresentation();
                }

                childNodes[i].DrawRepresentation(g2d, xoffset, yoffset);

                if (GlobalNodeHandler.transforming)
                {
                    childNodes[i].DrawRepresentation(g2d, xoffset, yoffset, childNodes[i].transformrectpicture, childNodes[i].transformRec);
                    //g2d.DrawRectangle(childNodes[i].transformRec, Colors.Black, 2);
                }
          
                //childNodes[i].repupdated = false;
                //}
            }
        }

        public void drawView(CanvasControl sender, CanvasDrawingSession g2d)
        {
            drawView(sender, g2d, 0, 0);
        }

        public void DrawRepresentation(CanvasDrawingSession g2d,int xoffset, int yoffset)
        {
            style.DrawImage(g2d, xpos+ xoffset, ypos+ yoffset, width, height, represent);
        }

        public void DrawRepresentation(CanvasDrawingSession g2d, int xoffset, int yoffset, CanvasRenderTarget picture, Rect boundsdrawrec)
        {
            style.DrawImage(g2d, (int)boundsdrawrec.X + xoffset, (int)boundsdrawrec.Y + yoffset, (int)boundsdrawrec.Width, (int)boundsdrawrec.Height, picture);
        }

        public void DrawRepresentationAt(CanvasDrawingSession g2d,int x, int y)
        {
            style.DrawImage(g2d, x, y, width, height, represent);
        }


        CanvasRenderTarget represent;
        CanvasRenderTarget transformrectpicture;
        bool repupdated = false;
        public void updateRepresentation()
        {
            if(represent != null)
                represent.Dispose();

            try {
                if (height > 0 && width > 0)
                {

                    CanvasDevice device = CanvasDevice.GetSharedDevice();
                    represent = new CanvasRenderTarget(device, this.width, this.height, 96);

                    using (CanvasDrawingSession g2d = represent.CreateDrawingSession())
                    {
                        g2d.Clear(Colors.Transparent);

                        g2d.Antialiasing = CanvasAntialiasing.Antialiased;

                        Color bordercolor = Colors.Black;
                        Color fillcolor = NodeColor;
                        //int linesize = 5;
                        
                        if (hovered)
                        {
                            fillcolor = Colors.LightGray;
                        }
                        if (selected)
                        {
                            fillcolor = Colors.Green;
                        }

                        style.DrawStyle(g2d, NodeDrawingStyle, 3, 3, width-6, height-6, fillcolor, bordercolor, (int)(borderOffset * scale));

                        if (!scaled)
                        {
                            g2d.DrawText(text, (int)(width - textSize.Width) / 2, (int)(height - textSize.Height) / 2, TextColor, NodeTextStyle);
                        }
                        else
                        {
                            g2d.DrawText(text, (int)(width - textSize.Width) / 2, (int)(height - textSize.Height) / 2, TextColor, NodeTextStyle);
                        }

                        repupdated = true;
                    }

                    UpdatePosition();
                    transformrectpicture = new CanvasRenderTarget(device, (float)transformRec.Width, (float)transformRec.Height , 96);
                    using (CanvasDrawingSession g2d = transformrectpicture.CreateDrawingSession())
                    {
                        g2d.DrawRectangle(new Rect(0,0, transformRec.Width, transformRec.Height), Colors.Black,2f);
                        /*g2d.DrawRectangle(new Rect(transformBorder, transformBorder, this.width, this.height), Colors.Black, 2f);

                        g2d.DrawRectangle(new Rect(0,0,transformBorder, transformBorder), Colors.Black, 2f);
                        g2d.DrawRectangle(new Rect(0, transformRec.Height- transformBorder, transformBorder, transformBorder), Colors.Black, 2f);
                        g2d.DrawRectangle(new Rect(transformRec.Width - transformBorder, transformRec.Height - transformBorder, transformBorder, transformBorder), Colors.Black, 2f);
                        g2d.DrawRectangle(new Rect(transformRec.Width - transformBorder, 0, transformBorder, transformBorder), Colors.Black, 2f);*/
                        g2d.DrawLine(transformBorder, 0, transformBorder, (float)transformRec.Height, Colors.Black, 2f);
                        g2d.DrawLine(0, transformBorder, (float)transformRec.Width, transformBorder, Colors.Black, 2f);
                        g2d.DrawLine(0, (float)transformRec.Height-transformBorder, (float)transformRec.Width, (float)transformRec.Height - transformBorder, Colors.Black, 2f);
                        g2d.DrawLine((float)transformRec.Width - transformBorder, 0, (float)transformRec.Width - transformBorder, (float)transformRec.Height, Colors.Black, 2f);

                    }

                        /*for (int x = 0; x < this.connections.Count; x++)
                        {
                            connections[x].updateRepresentation();
                        }*/
                }
            }
            catch(Exception e)
            {

            }
        }

        public void UpdateBridgesRepresentation()
        {
            for (int x = 0; x < this.connections.Count; x++)
            {
                connections[x].updateRepresentation();
            }
        }

        public void UpdateBridgesRepresentationInward()
        {
            for (int i = backconnNodes.Count - 1; i >= 0; i--)
            {
                if (backconnNodes[i].connNodes.Contains(this))
                {
                    int index = backconnNodes[i].connNodes.IndexOf(this);
                    backconnNodes[i].connections[index].updateRepresentation();
                }
            }
        }


        public Size CreateTextSize()
        {
            if (scale < 100)
            {
                Size textbounds;
                CanvasDevice device = CanvasDevice.GetSharedDevice();
                CanvasRenderTarget MaxtextCanvas = new CanvasRenderTarget(device, (int)(text.Length * 15 * scale), (int)(15 * scale), 96);

                using (CanvasDrawingSession g2d = MaxtextCanvas.CreateDrawingSession())
                {
                    g2d.Clear(Colors.Transparent);
                    NodeTextStyle.WordWrapping = CanvasWordWrapping.NoWrap;
                    CanvasTextLayout textLayout = new CanvasTextLayout(g2d, text, NodeTextStyle, 0.0f, 0.0f);
                    textbounds.Width = textLayout.DrawBounds.Width;
                    textbounds.Height = textLayout.DrawBounds.Height;

                    return textbounds;
                }
            }
            return new Size(0,0);
        }
        

    }
}
