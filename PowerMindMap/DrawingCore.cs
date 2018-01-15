using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace MindNoderPort
{
    public class DrawingCore
    {
        public DrawingCore()
        {

        }

        public void DrawImage(CanvasDrawingSession g2d, CanvasRenderTarget map, Rect drawRect)
        {
            g2d.DrawImage(map, drawRect);
        }

        public void DrawBezier(CanvasDrawingSession g2d,List<CalcPoint> pointslist,int xoffset, int yoffset, Color bordercolor, float linesize)
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasGeometry geometry;
            CanvasPathBuilder pathBuilder = new CanvasPathBuilder(device);

            pathBuilder.BeginFigure(new Vector2(pointslist[0].X + xoffset, pointslist[0].Y + yoffset));
            pathBuilder.AddCubicBezier(new Vector2(pointslist[1].X + xoffset, pointslist[1].Y + yoffset),
                new Vector2(pointslist[2].X + xoffset, pointslist[2].Y + yoffset), new Vector2(pointslist[3].X + xoffset, pointslist[3].Y + yoffset));
            pathBuilder.EndFigure(CanvasFigureLoop.Open);
            geometry = CanvasGeometry.CreatePath(pathBuilder);
            pathBuilder.Dispose();

            g2d.DrawGeometry(geometry, bordercolor, linesize);
            geometry.Dispose();
        }

        public void DrawLine(CanvasDrawingSession g2d, int xpos, int ypos, int xpos2, int ypos2, Color borderpen, float linesize)
        {
            g2d.DrawLine(xpos, ypos, xpos2, ypos2, borderpen, linesize);
        }

        public void DrawRectangle(CanvasDrawingSession g2d, int xpos, int ypos, int width, int height, Color borderpen, float linesize)
        {
            g2d.DrawRectangle(xpos, ypos, width, height, borderpen, linesize);
        }

        public void FillRectangle(CanvasDrawingSession g2d, int xpos, int ypos, int width, int height, Color fillbrush)
        {
            g2d.FillRectangle(xpos, ypos, width, height, fillbrush);
        }

        public void DrawEllipse(CanvasDrawingSession g2d, int xpos, int ypos, int width, int height, Color borderpen, float linesize)
        {
            g2d.DrawEllipse(xpos, ypos, width, height, borderpen, linesize);
        }

        public void FillEllipse(CanvasDrawingSession g2d, int xpos, int ypos, int width, int height, Color fillbrush)
        {
            g2d.FillEllipse(xpos, ypos, width, height, fillbrush);
        }
        
    }
}
