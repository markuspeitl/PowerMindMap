using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace MindNoderPort
{
    public class Bridge
    {
        public bool useNode = true;
        public NodeLabel textNode;
        public CalcPoint startPoint;
        public CalcPoint endPoint;
        public double angle;
        public CalcPoint middlePoint = new CalcPoint();
        public double distance;
        public Direction direction;
        //public int directionSmall = -1;
        private bool usedirections = true;
        private int directions = 4;
        public List<CalcPoint> nodes = new List<CalcPoint>();

        //private static readonly ILog log = LogManager.GetLogger(typeof(Bridge));

        public Bridge(CalcPoint start, CalcPoint end, Color col)
        {
            ReBridge(start, end);
            textNode.NodeColor = col;
        }

        public Point PointDirection()
        {
            //CalcPoint directionpoint = new CalcPoint(0, 0);

            Point directionpoint = new Point(0, 0);

            /*if (direction == Direction.RIGHT)
            {
                directionpoint.X = 1;
            }
            else if (direction == Direction.DOWN)
            {
                directionpoint.Y = 1;
            }
            else if (direction == Direction.LEFT)
            {
                directionpoint.X = -1;
            }
            else if (direction == Direction.UP)
            {
                directionpoint.Y = -1;
            }*/

            directionpoint.X = 1 * Math.Sin((angle / 360) * Math.PI);
            directionpoint.Y = 1 * Math.Cos((angle / 360) * Math.PI);

            if (direction != Direction.LEFT && direction != Direction.RIGHT)
            {
                if(usedirections)
                    directionpoint.X = ((double)(int)(directionpoint.X * (directions))) / directions;

                if (directionpoint.Y < 0)
                {
                    directionpoint.Y = -1.0;
                }
                else
                {
                    directionpoint.Y = 1.0;
                }
            }
            else
            {
                if (usedirections)
                    directionpoint.Y = ((double)(int)(directionpoint.Y * (directions))) / directions;

                if (directionpoint.X < 0)
                {
                    directionpoint.X = -1.0;
                }
                else
                {
                    directionpoint.X = 1.0;
                }
            }

            return directionpoint;
        }

        public Bridge(int startx, int starty, int endx, int endy, Color col)
        {
            ReBridge(new CalcPoint(startx, starty), new CalcPoint(endx, endy));
            textNode.NodeColor = col;
        }

        public List<CalcPoint> GetNodeList()
        {
            return nodes;
        }
        public List<CalcPoint> GetNodeListRelativeTo(CalcPoint point)
        {
            nodes.Add(endPoint);
            for (int x = nodes.Count - 1; x >= 0; x--)
            {
                nodes[x] = new CalcPoint(nodes[x].X-point.X, nodes[x].Y -point.Y);
            }

            return nodes;
        }

        public void ExpandOnAnchor(CalcPoint Anchor, double factor)
        {
            //factor = 1.0;
            CalcPoint diff;
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                    diff = new CalcPoint(Anchor.X - nodes[i].X, Anchor.Y - nodes[i].Y);
                    diff.X = (int)(diff.X * factor - diff.X);
                    diff.Y = (int)(diff.Y * factor - diff.Y);
                    nodes[i] = new CalcPoint(nodes[i].X - diff.X, nodes[i].Y - diff.Y);
            }

        }

        public void ReBridge(CalcPoint start, CalcPoint end)
        {
            //Task.Run(() =>
            //{
                nodes = new List<CalcPoint>();
                startPoint = start;
                endPoint = end;
                nodes.Add(startPoint);

                angle = CalculateAngle();
                if (angle < 270 && angle >= 90)
                {
                    direction = Direction.RIGHT;
                }
                else if (angle < 90 && angle >= -90)
                {
                    direction = Direction.DOWN;
                }
                else if (angle < -90 && angle >= -270)
                {
                    direction = Direction.LEFT;
                }
                else if (angle < (-270) || angle >= 270)
                {
                    direction = Direction.UP;
                }

                CalculateDistance();
                CaculateMiddlePoint();
                AddBasicNodes();
            //});

            textNode = new NodeLabel(middlePoint.X, middlePoint.Y, 20, 20, true);
            textNode.SetText("Bridge");
        }

        public void CopyExt(Bridge ext)
        {
            this.nodes = ext.nodes;

            this.angle = ext.angle;
            this.middlePoint = ext.middlePoint;
            this.direction = ext.direction;

            this.textNode.boundBox = ext.textNode.boundBox;

            this.textNode.UpdateWidth();
            this.updateRepresentation();
        }

        public void AddBasicNodes()
        {

            if (direction == Direction.RIGHT)
            {
                nodes.Add(new CalcPoint(middlePoint.X, startPoint.Y));
                nodes.Add(new CalcPoint(middlePoint.X, endPoint.Y));
            }
            else if (direction == Direction.DOWN)
            {
                nodes.Add(new CalcPoint(startPoint.X, middlePoint.Y));
                nodes.Add(new CalcPoint(endPoint.X, middlePoint.Y));
            }
            else if (direction == Direction.LEFT)
            {
                nodes.Add(new CalcPoint(middlePoint.X, startPoint.Y));
                nodes.Add(new CalcPoint(middlePoint.X, endPoint.Y));
            }
            else if (direction == Direction.UP)
            {
                nodes.Add(new CalcPoint(startPoint.X, middlePoint.Y));
                nodes.Add(new CalcPoint(endPoint.X, middlePoint.Y));
            }

            nodes.Add(endPoint);
        }

        public void CaculateMiddlePoint()
        {

            middlePoint.Y = (startPoint.Y + endPoint.Y) / 2;
            middlePoint.X = (startPoint.X + endPoint.X) / 2;
        }

        public double CalculateAngle()
        {
            double xDiff = endPoint.X - startPoint.X;
            double yDiff = endPoint.Y - startPoint.Y;

            // Range: -180 -> 180, y-axis = 180/ 0(down)
            return Math.Atan2(xDiff, yDiff) * 360 / Math.PI;
        }

        public double CalculateDistance()
        {
            double xDiff = startPoint.X - endPoint.X;
            double yDiff = startPoint.Y - endPoint.Y;
            distance = Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
            return distance;
        }

        NodeStyler style = new NodeStyler();
        /*public void DrawRepresentation(Graphics g2d)
        {
            updateRepresentation();
            style.DrawImage(g2d, bitmappoint.X, bitmappoint.Y, represent.Width, represent.Height, represent);
            textNode.DrawRepresentation(g2d);
            represent.Dispose();
        }*/
        
        public void drawBridge(CanvasControl sender,CanvasDrawingSession g2d, int xoffset, int yoffset)
        {
            /*g2d.Antialiasing = CanvasAntialiasing.Antialiased;

            Color bordercolor = Colors.Black;
            int linesize = 2;
            

            CalcPoint second = new CalcPoint();
            CalcPoint arrowleft;
            CalcPoint arrowright;
            CalcPoint direction;
            List<CalcPoint> pointslist;

            pointslist = this.nodes;

            direction = this.PointDirection();
            
            *//*for (int i = 0; i < pointslist.Count - 1; i++)
            {
                second = pointslist.ElementAt(i + 1);
                g2d.DrawLine(pointslist[i].X, second.X, pointslist[i].Y, second.Y, bordercolor, linesize);

            }*//*
            second = pointslist.Last();

            var pathBuilder = new CanvasPathBuilder(sender);

            pathBuilder.BeginFigure(new Vector2(pointslist[0].X+ xoffset, pointslist[0].Y + yoffset));
            pathBuilder.AddCubicBezier(new Vector2(pointslist[1].X + xoffset, pointslist[1].Y + yoffset), 
                new Vector2(pointslist[2].X + xoffset, pointslist[2].Y + yoffset), new Vector2(pointslist[3].X + xoffset, pointslist[3].Y + yoffset));
            pathBuilder.EndFigure(CanvasFigureLoop.Open);
            CanvasGeometry geometry = CanvasGeometry.CreatePath(pathBuilder);

            g2d.DrawGeometry(geometry,bordercolor,linesize);


            arrowleft = new CalcPoint( second.X + xoffset, second.Y + yoffset);
            arrowright = new CalcPoint(second.X + xoffset, second.Y + yoffset);

            if (direction.X != 0)
            {
                arrowleft = new CalcPoint(second.X - 10 * direction.X, second.Y - 10);
                arrowright = new CalcPoint(second.X - 10 * direction.X, second.Y + 10);
            }
            if (direction.Y != 0)
            {
                arrowleft = new CalcPoint(second.X - 10, second.Y - 10 * direction.Y);
                arrowright = new CalcPoint(second.X + 10, second.Y - 10 * direction.Y);
            }

            //g2d.DrawLine(new Vector2(second), new Vector2(arrowleft), bordercolor, linesize);
            g2d.DrawLine(second.X, second.Y, arrowleft.X, arrowleft.Y, bordercolor, linesize);
            g2d.DrawLine(second.X, second.Y, arrowright.X, arrowright.Y, bordercolor, linesize);*/

            //CreateBoundBox();

            style.DrawImage(g2d, (int)boundbox.X + xoffset, (int)boundbox.Y + yoffset, (int)boundbox.Width, (int)boundbox.Height, represent);

            if(GlobalNodeHandler.bridgecontrol)
                textNode.DrawRepresentation(g2d,xoffset,yoffset);

        }

        CalcPoint masteredge = new CalcPoint();
        int pixborder = 10;
        public void CreateBoundBox()
        {
            int xmin;
            int xmax;
            int ymin;
            int ymax;
            CalcPoint start = this.nodes.First();
            CalcPoint end = this.nodes.Last();

            if (start.X >= end.X)
            {
                xmax = start.X;
                xmin = end.X;
                masteredge.X = 0;
            }
            else
            {
                xmin = start.X;
                xmax = end.X;
                masteredge.X = 1;
            }
            if (start.Y >= end.Y)
            {
                ymax = start.Y;
                ymin = end.Y;
                masteredge.Y = 0;
            }
            else
            {
                ymin = start.Y;
                ymax = end.Y;
                masteredge.Y = 1;
            }
            boundbox = new Rect(xmin - pixborder, ymin - pixborder, xmax-xmin + 2* pixborder, ymax-ymin + 2* pixborder);
        }

        public void MoveBoundBox(CalcPoint direction)
        {
            if (boundbox != null)
            {
                if (boundbox.Width > 0 && boundbox.Width > 0)
                {
                    boundbox.X = boundbox.X - direction.X;
                    boundbox.Y = boundbox.Y - direction.Y;
                }
            }
        }

        CanvasRenderTarget represent;
        bool repupdated = false;
        Rect boundbox;

        DrawingCore core = new DrawingCore();

        CanvasDevice device = CanvasDevice.GetSharedDevice();
        CanvasGeometry geometry;
        public void updateRepresentation()
        {
            if (represent != null)
                represent.Dispose();

            CreateBoundBox();
            
            try
            {
                if (boundbox.Height > 0 && boundbox.Width > 0)
                {
                    represent = new CanvasRenderTarget(device, (int)this.boundbox.Width, (int)this.boundbox.Height, 96);

                    using (CanvasDrawingSession g2d = represent.CreateDrawingSession())
                    {
                        g2d.Clear(Colors.Transparent);

                        g2d.Antialiasing = CanvasAntialiasing.Antialiased;

                        Color bordercolor = Colors.Black;
                        int linesize = 2;


                        CalcPoint second = new CalcPoint();
                        CalcPoint arrowleft;
                        CalcPoint arrowright;
                        CalcPoint direction;
                        List<CalcPoint> pointslist;

                        int xoffset = -(int)(boundbox.X);
                        int yoffset = -(int)(boundbox.Y);

                        pointslist = this.nodes;

                        direction = new CalcPoint(this.PointDirection());

                        /*for (int i = 0; i < pointslist.Count - 1; i++)
                        {
                            second = pointslist.ElementAt(i + 1);
                            g2d.DrawLine(pointslist[i].X, second.X, pointslist[i].Y, second.Y, bordercolor, linesize);

                        }*/
                        second = pointslist.Last();

                        core.DrawBezier(g2d,pointslist,xoffset,yoffset,bordercolor,linesize);
                        
                        arrowleft = new CalcPoint(second.X + xoffset, second.Y + yoffset);
                        arrowright = new CalcPoint(second.X + xoffset, second.Y + yoffset);

                        if (direction.X != 0)
                        {
                            arrowleft = new CalcPoint(second.X + xoffset - 10 * direction.X, second.Y + yoffset - 10);
                            arrowright = new CalcPoint(second.X + xoffset - 10 * direction.X, second.Y + yoffset + 10);
                        }
                        if (direction.Y != 0)
                        {
                            arrowleft = new CalcPoint(second.X + xoffset - 10, second.Y + yoffset - 10 * direction.Y);
                            arrowright = new CalcPoint(second.X + xoffset + 10, second.Y + yoffset - 10 * direction.Y);
                        }

                        //g2d.DrawLine(new Vector2(second), new Vector2(arrowleft), bordercolor, linesize);
                        core.DrawLine(g2d,second.X + xoffset, second.Y + yoffset, arrowleft.X, arrowleft.Y, bordercolor, linesize);
                        core.DrawLine(g2d,second.X + xoffset, second.Y + yoffset, arrowright.X, arrowright.Y, bordercolor, linesize);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        /*public Bitmap represent;
        Point bitmappoint = new Point();
        public void updateRepresentation()
        {

            if(represent != null)
                represent.Dispose();

            int height = Math.Abs(startPoint.Y - endPoint.Y);
            int width = Math.Abs(startPoint.X - endPoint.X);
            bitmappoint.X = Math.Min(startPoint.X, endPoint.X);
            bitmappoint.Y = Math.Min(startPoint.Y, endPoint.Y);

            if (height > 0 && width > 0)
            {
                Rectangle boundingrect = new Rectangle(0, 0, width, height);
                //represent = new Bitmap(width, height);

                Pen borderpen = new Pen(Color.Black, 2);
                Brush fillbrush = new SolidBrush(Color.Black);

                represent = new Bitmap(width, height);
                    using (Graphics g2d = Graphics.FromImage(represent))
                    {
                        Point second = new Point();
                        Point arrowleft;
                        Point arrowright;
                        Point direction;
                        List<Point> pointslist;

                        pointslist = this.GetNodeList();
                        for (int x = pointslist.Count - 1; x >= 0; x--)
                        {
                            pointslist[x] = new Point(nodes[x].X - bitmappoint.X, nodes[x].Y - bitmappoint.Y);
                        }

                    direction = this.PointDirection();

                    for (int i = 0; i < pointslist.Count - 1; i++)
                    {
                        second = pointslist.ElementAt(i + 1);
                        g2d.DrawLine(borderpen, pointslist[i], second);

                    }

                    g2d.DrawBezier(borderpen, pointslist.ElementAt(0), pointslist.ElementAt(1), pointslist.ElementAt(2), pointslist.ElementAt(3));
                    //g2d.DrawLine(borderpen, width / 2, 0, width / 2, height);
                    //g2d.DrawLine(borderpen, 0, 0, width / 2, 0);

                    arrowleft = second;
                        arrowright = second;

                        if (direction.X != 0)
                        {
                            arrowleft = new Point(second.X - 10 * direction.X, second.Y - 10);
                            arrowright = new Point(second.X - 10 * direction.X, second.Y + 10);
                        }
                        if (direction.Y != 0)
                        {
                            arrowleft = new Point(second.X - 10, second.Y - 10 * direction.Y);
                            arrowright = new Point(second.X + 10, second.Y - 10 * direction.Y);
                        }

                        g2d.DrawLine(borderpen, second, arrowleft);
                        g2d.DrawLine(borderpen, second, arrowright);

                        

                        //this.textNode.draw(g2d);
                    }
            }
        }*/

        public enum Direction
        {
            LEFT,
            UP,
            RIGHT,
            DOWN
        }

    }
}
