using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace MindNoderPort
{
    public class NodeLabel
    {
        String text = "";
        CalcPoint textpos;
        public Rect boundBox;
        CalcPoint LabelCenter;

        private Font TextFont;
        Size textSize;

        public Color TextColor = Colors.White;
        public Color NodeColor = Colors.Red;
        public Color BorderColor = Colors.Black;
        private CanvasTextFormat NodeTextStyle;
        private int fontsize = 15;

        public DrawingStyle NodeDrawingStyle = DrawingStyle.RECTANGLE;
        public int borderOffset = 40;

        private bool hovered = false;
        public Bridge parent = null;

        public double scale = 1.0;

        public NodeLabel(int x, int y, int width, int height, bool mousepivot)
        {
            NodeTextStyle = new CanvasTextFormat();
            NodeTextStyle.FontFamily = "Arial";
            NodeTextStyle.FontSize= (int)(fontsize * scale);
            NodeTextStyle.WordWrapping = CanvasWordWrapping.NoWrap;
            fontsize = 18;

            this.boundBox = new Rect(x,y, (int)(width *scale), (int)(height *scale));
            SetPosition(x, y, mousepivot);

            UpdateCenter();

            UpdateWidth();
        }

        public void setHovered(bool active)
        {
            if (active != hovered)
            {
                hovered = active;
                updateRepresentation();
            }
        }

        public void SetText(String newtext)
        {
            text = newtext;
            UpdateWidth();
        }
        public String GetText()
        {
            return text;
        }

        public void SetNodeScale(double ratio)
        {
            if (scale >= 0.4)
            {
                this.scale = ratio;
                NodeTextStyle.FontSize = (int)(fontsize * scale);

                //this.width = (int)(width * scale);
                //this.height = (int)(height * scale);
                UpdateWidth();
            }
        }

        public void PseudoScale(double ratio)
        {
            if (scale >= 0.4)
            {
                this.scale = ratio;
                NodeTextStyle.FontSize = (int)(fontsize * scale);
                //this.width = (int)(width * scale);
                //this.height = (int)(height * scale);
                
            }

            if (boundBox.Width > 0 && boundBox.Height > 0)
            {
                if (text.Length > 2)
                {
                    textSize = CreateTextSize();
                    //textSize = TextRenderer.MeasureText(text, TextFont);
                    boundBox.Width = (int)(textSize.Width + 10*scale);
                    boundBox.Height = (int)(textSize.Height + 7 * scale);
                }

            }
        }

        public void UpdateCenter()
        {
            LabelCenter =new CalcPoint((int)(boundBox.X + boundBox.Width / 2), (int)(boundBox.X + boundBox.Width / 2));
        }

        public void ExpandOnAnchor(CalcPoint Anchor, double factor)
        {
            //factor = 1.0;
            CalcPoint diff;
            diff = new CalcPoint(Anchor.X - this.LabelCenter.X, Anchor.Y - this.LabelCenter.Y);
            diff.X = (int)(diff.X * factor - diff.X);
            diff.Y = (int)(diff.Y * factor - diff.Y);
            this.SetPosition(this.LabelCenter.X - diff.X, this.LabelCenter.Y - diff.Y, true);
        }

        public void UpdateWidth()
        {
            if (boundBox.Width > 0 && boundBox.Height > 0)
            {
                if (text.Length > 2)
                {
                    //textSize = TextRenderer.MeasureText(text, TextFont);
                    textSize = CreateTextSize();
                    boundBox.Width = (int)(textSize.Width + 10 * scale);
                    boundBox.Height = (int)(textSize.Height + 7 * scale);
                }
             
                updateRepresentation();
            }

        }

        public NodeLabel ContainsNode(int x, int y)
        {
            if (Contains(x, y))
            {
                return this;
            }
            return null;
        }

        public bool Contains(int x, int y)
        {
            return (boundBox.Contains(new Point(x,y)));
        }

        public void SetPosition(int x, int y, bool mousePivot)
        {
            if (mousePivot)
            {
                boundBox.X = x - (int)((boundBox.Width / 2));
                boundBox.Y = y - (int)((boundBox.Height / 2));
            }
            if (!mousePivot)
            {
                boundBox.X = x;
                boundBox.Y = y;
            }
        }

        public void MoveBoundBox(CalcPoint direction)
        {
            if (boundBox != null)
            {
                if (boundBox.Width > 0 && boundBox.Width > 0)
                {
                    boundBox.X = boundBox.X - direction.X;
                    boundBox.Y = boundBox.Y - direction.Y;
                }
            }
        }

        private NodeStyler style = new NodeStyler();

        public void DrawRepresentation(CanvasDrawingSession g2d,int xoffset,int yoffset)
        {
            style.DrawImage(g2d, new Rect(boundBox.X + xoffset,boundBox.Y + yoffset,boundBox.Width,boundBox.Height), represent);
        }

        CanvasRenderTarget represent;
        public void updateRepresentation()
        {
            //UpdateWidth();
            try
            {
                if (boundBox.Height > 0 && boundBox.Width > 0)
                {
                    CanvasDevice device = CanvasDevice.GetSharedDevice();

                    represent = new CanvasRenderTarget(device, (int)this.boundBox.Width, (int)this.boundBox.Height, 96);

                    using (CanvasDrawingSession g2d = represent.CreateDrawingSession())
                    {
                        g2d.Clear(Colors.Transparent);

                        g2d.Antialiasing = CanvasAntialiasing.Antialiased;

                        //int linesize = 5;

                        Color fillbrush = NodeColor;

                        if (hovered)
                        {
                            fillbrush = Colors.Black;
                        }

                        style.DrawStyle(g2d, NodeDrawingStyle, 0, 0, (int)boundBox.Width-2, (int)boundBox.Height-2, fillbrush, BorderColor, (int)(borderOffset * scale));

                        g2d.DrawText(text, 0, 0, TextColor, NodeTextStyle);

                    }
                }
            }
            catch (Exception e)
            {

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
            return new Size(0, 0);
        }

    }
    
}
