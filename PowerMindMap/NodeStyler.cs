using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace MindNoderPort
{
    public class NodeStyler
    {
        DrawingCore core;

        public NodeStyler() {
            core = new DrawingCore();
        }

        Point[] points = new Point[6];
        int distance;
        int linesize = 2;
        public void DrawStyle(CanvasDrawingSession g2d, DrawingStyle style,int xpos, int ypos, int width, int height, Color fillbrush, Color borderpen,int offeset)
        {
   
            distance = height;
            switch (style)
            {
                case DrawingStyle.RECTANGLE:
                    // Rectangle Style
                    core.FillRectangle(g2d,xpos, ypos, width, height, fillbrush);
                    core.DrawRectangle(g2d, xpos, ypos, width, height, borderpen, linesize);
                    break;
                case DrawingStyle.ELLIPSE:
                    // Ellipse Style
                    core.FillEllipse(g2d,xpos + width/2, ypos+ height/2, width/2, height/2, fillbrush);
                    core.DrawEllipse(g2d,xpos + width / 2, ypos + height / 2, width / 2, height / 2, borderpen,linesize);
                    //if(GlobalNodeHandler.transforming)
                        //g2d.DrawRectangle(xpos, ypos, width, height, borderpen, linesize);
                    break;
                case DrawingStyle.BUTTON:
                    // ButtonStyle (Gets Weird on long nodes)
                    /*g2d.DrawRectangle(borderpen, xpos, ypos, width, height);
                    points[0] = new Point(xpos + width- offeset, ypos);
                    points[1] = new Point(xpos + width, ypos + height / 2);
                    points[2] = new Point(xpos + width - offeset, ypos + height);
                    points[3] = new Point(xpos + offeset, ypos);
                    points[4] = new Point(xpos, ypos + height / 2);
                    points[5] = new Point(xpos + offeset, ypos + height);
                    g2d.DrawClosedCurve(borderpen, points);
                    g2d.FillClosedCurve(fillbrush, points);
                    g2d.FillRectangle(fillbrush, xpos, ypos, width, height);*/
                    offeset = offeset / 2;
                    core.DrawRectangle(g2d, xpos + offeset / 2, ypos, width - offeset, height, borderpen, linesize);
                    core.DrawEllipse(g2d, xpos + offeset / 2, ypos + height / 2, offeset / 2, height / 2, borderpen, linesize);
                    core.DrawEllipse(g2d, xpos + width - offeset / 2, ypos + height / 2, offeset / 2, height / 2, borderpen, linesize);
                    core.FillRectangle(g2d, xpos + offeset / 2, ypos, width - offeset, height, fillbrush);
                    core.FillEllipse(g2d, xpos + offeset / 2, ypos + height / 2, offeset / 2, height / 2, fillbrush);
                    core.FillEllipse(g2d, xpos + width - offeset / 2, ypos + height / 2, offeset / 2, height / 2, fillbrush);
                    break;
                case DrawingStyle.CURVED:
                    //Simple Closed Curve (Gets Weird on long nodes)
                    /*points[0] = new Point(xpos+ offeset, ypos);
                    points[1] = new Point(xpos, ypos + height / 2);
                    points[2] = new Point(xpos+ offeset, ypos + height);
                    points[3] = new Point(xpos + width-offeset, ypos + height);
                    points[4] = new Point(xpos + width, ypos + height / 2);
                    points[5] = new Point(xpos + width-offeset, ypos);
                    g2d.FillClosedCurve(fillbrush, points);
                    g2d.DrawClosedCurve(borderpen, points);*/
                    break;
                case DrawingStyle.POINTY:
                    // Edgy (Triangles around edges)
                    /*points[0] = new Point(xpos + offeset, ypos);
                    points[1] = new Point(xpos, ypos + height / 2);
                    points[2] = new Point(xpos + offeset, ypos + height);
                    points[3] = new Point(xpos + width -offeset, ypos + height);
                    points[4] = new Point(xpos + width, ypos + height / 2);
                    points[5] = new Point(xpos + width - offeset, ypos);
                    g2d.FillPolygon(fillbrush, points);
                    g2d.DrawPolygon(borderpen, points);*/
                    break;
                case DrawingStyle.TRAPEZE:
                    // Trapeze
                    /*distance = height/4;
                    points = new Point[8];
                    points[0] = new Point(xpos + offeset, ypos);
                    points[1] = new Point(xpos, ypos + distance);
                    points[2] = new Point(xpos, ypos + height - distance);
                    points[3] = new Point(xpos + offeset, ypos + height);
                    points[4] = new Point(xpos + width - offeset, ypos + height);
                    points[5] = new Point(xpos + width, ypos + height - distance);
                    points[6] = new Point(xpos + width, ypos + distance);
                    points[7] = new Point(xpos + width - offeset, ypos);
                    g2d.FillPolygon(fillbrush, points);
                    g2d.DrawPolygon(borderpen, points);*/
                    break;
                case DrawingStyle.FISHFIN:
                    // Fish fin style
                    /*distance = height;
                    points = new Point[8];
                    points[0] = new Point(xpos + offeset, ypos);
                    points[1] = new Point(xpos, ypos + distance);
                    points[2] = new Point(xpos, ypos + height - distance);
                    points[3] = new Point(xpos + offeset, ypos + height);
                    points[4] = new Point(xpos + width - offeset, ypos + height);
                    points[5] = new Point(xpos + width, ypos + height - distance);
                    points[6] = new Point(xpos + width, ypos + distance);
                    points[7] = new Point(xpos + width - offeset, ypos);
                    g2d.FillPolygon(fillbrush, points);
                    g2d.DrawPolygon(borderpen, points);*/
                    break;
                case DrawingStyle.ELLIPSEEDGE:
                    //Draws the edges as ellipses which are defined through borderwidth
                    core.DrawRectangle(g2d, xpos + offeset / 2, ypos, width - offeset, height, borderpen, linesize);
                    core.DrawEllipse(g2d, xpos + offeset / 2, ypos + height / 2, offeset / 2, height / 2, borderpen, linesize);
                    core.DrawEllipse(g2d, xpos + width - offeset/2, ypos + height / 2, offeset/2, height/2,borderpen, linesize);
                    core.FillRectangle(g2d, xpos + offeset / 2, ypos, width - offeset, height, fillbrush);
                    core.FillEllipse(g2d, xpos + offeset / 2, ypos + height / 2, offeset / 2, height / 2, fillbrush);
                    core.FillEllipse(g2d, xpos + width - offeset / 2, ypos + height / 2, offeset / 2, height / 2, fillbrush);
                    break;
                case DrawingStyle.HANDLE:
                    //Draws the edges as ellipses which are defined through borderwidth
                    core.DrawRectangle(g2d, xpos + offeset, ypos, width - offeset * 2, height, borderpen,linesize);

                    core.DrawEllipse(g2d, xpos, ypos, offeset, height, borderpen,linesize);
                    core.DrawEllipse(g2d, xpos + width - offeset, ypos, offeset, height, borderpen, linesize);
                    core.FillRectangle(g2d, xpos + offeset, ypos, width - offeset * 2, height, fillbrush);
                    core.FillEllipse(g2d, xpos, ypos, offeset, height, fillbrush);
                    core.FillEllipse(g2d, xpos + width - offeset, ypos, offeset, height, fillbrush);
                    break;

                case DrawingStyle.CIRCLE:
                    //Draws the edges as ellipses which are defined through borderwidth
                    core.FillEllipse(g2d, xpos + width / 2, ypos + height / 2, width / 2, height / 2, fillbrush);
                    core.DrawEllipse(g2d, xpos + width / 2, ypos + height / 2, width / 2, height / 2, borderpen, linesize);
                    //if (GlobalNodeHandler.transforming)
                        //g2d.DrawRectangle(xpos, ypos, width, height, borderpen, linesize);
                    break;
                    //case DrawingStyle.IMAGE:
                    // Using a predefined Sprite for the Node Style
                    /*Bitmap map = new Bitmap(500, 500);
                    using (Graphics g = Graphics.FromImage(map))
                    {
                        g.FillEllipse(fillbrush, 0, 0, 500, 500);
                    }

                    g2d.DrawImage(map,xpos,ypos,width,height);*/
                    //break;

            }

        }

        public void DrawImage(CanvasDrawingSession g2d, int xpos, int ypos, int width, int height, CanvasRenderTarget map)
        {
            if(width > 0 && height >0)
                core.DrawImage(g2d,map,new Rect(xpos,ypos,width,height));
        }

        public void DrawImage(CanvasDrawingSession g2d, Rect rect, CanvasRenderTarget map)
        {
            if (rect.Width > 0 && rect.Height > 0)
                core.DrawImage(g2d,map, rect);
        }



    }

    public enum DrawingStyle{
        RECTANGLE,
        ELLIPSE,
        BUTTON,
        CURVED,
        POINTY,
        TRAPEZE,
        FISHFIN,
        ELLIPSEEDGE,
        HANDLE,
        IMAGE,
        CIRCLE
    }
}
