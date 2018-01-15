using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace MindNoderPort
{
    public class ColorPicker
    {
        Stack<Color> colorpalette;
        int cols = 0;
        int rows = 0;
        ColorField[][] colormatrix;
        int pixSize = 20;
        CalcPoint matrixPos = new CalcPoint(10, 10);
        int spacing = 10;
        ColorField selectedField;
        public static Color selectedcolor = Colors.Blue;
        public Rect boundingbox;
        public bool isVisible = true;

        public ColorPicker()
        {
            
            selectedField = new ColorField();
            selectedField.color = Colors.Blue;
        }

        public ColorPicker(int cols, int rows, Rect boundingBox)
        {
            this.boundingbox = boundingBox;
            selectedField = new ColorField();
            selectedField.color = Colors.Blue;

            colorpalette = new Stack<Color>();
            this.cols = cols;
            this.rows = rows;
            colormatrix = new ColorField[cols][];
            for (int i = 0; i < colormatrix.Length; i++)
            {
                colormatrix[i] = new ColorField[rows];
            }

            colorpalette.Push(Colors.Blue);
            colorpalette.Push(Colors.LightBlue);
            colorpalette.Push(Colors.DeepSkyBlue);
            colorpalette.Push(Colors.Green);
            colorpalette.Push(Colors.LightGreen);
            colorpalette.Push(Colors.LawnGreen);
            colorpalette.Push(Colors.Red);
            colorpalette.Push(Colors.DarkRed);
            colorpalette.Push(Colors.Pink);
            colorpalette.Push(Colors.Yellow);
            colorpalette.Push(Colors.Orange);
            colorpalette.Push(Colors.Gold);
            colorpalette.Push(Colors.Silver);
            colorpalette.Push(Colors.Cyan);
            colorpalette.Push(Colors.DarkCyan);
            colorpalette.Push(Colors.Violet);
            colorpalette.Push(Colors.Beige);
            colorpalette.Push(Colors.Lavender);
            colorpalette.Push(Colors.Tomato);
            colorpalette.Push(Colors.Turquoise);
            colorpalette.Push(Colors.Wheat);

            colorpalette.Push(Colors.Black);
            colorpalette.Push(Colors.White);

            fillMatrix();
            updateRepresentation();

        }

        public void AddColor(Color newcolor)
        {
            colorpalette.Push(newcolor);
            updateRepresentation();
        }

        public bool thisContains(int x, int y)
        {
            return thisContains(new Point(x, y));
        }

        public bool thisContains(Point xy)
        {
            return boundingbox.Contains(xy);
        }

        public bool ChangeColorTo(Point xy)
        {
            if (isVisible)
            {
                if (boundingbox.Contains(xy))
                {
                    ColorField selectedfield = null;

                    if ((selectedfield = GetColorFieldSelected(new Point(xy.X - boundingbox.X, xy.Y - boundingbox.Y))) != null)
                    {
                        this.selectedField.color = selectedfield.color;
                        selectedcolor = selectedField.color;
                        updateRepresentation();
                        return true;
                    }
                }
            }
            return false;
        }

        public bool BoxContains(Point xy)
        {
            return boundingbox.Contains(xy);
        }


        public void fillMatrix()
        {
            for (int x = 0; x < colormatrix.Length; x++)
            {
                for (int y = 0; y < colormatrix[x].Length; y++)
                {
                    colormatrix[x][y] = new ColorField();
                }
            }

        }

        public void draw(CanvasDrawingSession g2d)
        {
            if(isVisible)
                g2d.DrawImage(represent, boundingbox);
        }

        CanvasRenderTarget represent;
        public void updateRepresentation()
        {
            if (boundingbox.Height > 0 && boundingbox.Width > 0)
            {

                CanvasDevice device = CanvasDevice.GetSharedDevice();
                represent = new CanvasRenderTarget(device, (int)boundingbox.Width, (int)boundingbox.Height, 96);

                using (CanvasDrawingSession g2d = represent.CreateDrawingSession())
                {
                    g2d.Clear(Colors.LightGray);

                    g2d.Antialiasing = CanvasAntialiasing.Antialiased;

                    int xpos = 0;
                    int ypos = 0;
                    for (int y = 0; y < colormatrix.Length; y++)
                    {
                        for (int x = 0; x < colormatrix[y].Length; x++)
                        {
                            xpos = matrixPos.X + x * (pixSize + spacing);
                            ypos = matrixPos.Y + y * (pixSize + spacing);
                            colormatrix[x][y].pickRect = new Rect(xpos, ypos, pixSize, pixSize);
                            if (colorpalette.Count >= 1)
                                colormatrix[x][y].color = colorpalette.Pop();
                            colormatrix[x][y].draw(g2d);

                        }
                    }

                    selectedField.pickRect = new Rect(matrixPos.X, ypos + pixSize + spacing, xpos - pixSize, pixSize + spacing);
                    selectedField.draw(g2d);

                }
            }
        }

        public ColorField GetColorFieldSelected(Point xy)
        {
            for (int y = 0; y < colormatrix.Length; y++)
            {
                for (int x = 0; x < colormatrix[y].Length; x++)
                {
                    if (colormatrix[x][y].ContainsPoint(xy))
                    {
                        return colormatrix[x][y];
                    }
                }
            }

            return null;
        }
        
    }
    

    public class ColorField
    {

        public Rect pickRect;
        public Color color = Colors.White;
        private Color blackpen = Colors.Black;
        private int linesize = 2;

        public ColorField()
        {
        }

        public bool ContainsPoint(Point xy)
        {
            if (this.pickRect.Contains(xy))
            {
                return true;
            }
            return false;
        }

        public void draw(CanvasDrawingSession g2d)
        {
            Color fillbrush = color;

            g2d.FillRectangle(pickRect,fillbrush);
            g2d.DrawRectangle(pickRect,blackpen, linesize);
        }

    }
}
