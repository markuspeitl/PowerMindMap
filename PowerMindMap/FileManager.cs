using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Popups;

namespace MindNoderPort
{
    public class FileManager
    {
        //private static readonly ILog filemanagerlog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public FileManager()
        {

        }

        public async void WriteFile(String path, IStorageFile fiel)
        {
            try
            {
                //int linesperNode = 6;
                int nodecnt = GlobalNodeHandler.masterNode.GetNodeCount();
                //String FileText = GlobalNodeHandler.masterNode.GetRepresentationText();
                String FileText = GlobalNodeHandler.masterNode.GetRepresentationXString();

                await Windows.Storage.FileIO.WriteTextAsync(fiel, FileText, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
            catch
            {
                MessageDialog dialog = new MessageDialog("Something did go wrong when saving the file, please try again!", "Error!");
                await dialog.ShowAsync();
            }
        }

        public async Task ReadFile(String path, IStorageFile fiel)
        {

            try
            {

                MindNode newMasterNode = new MindNode(0, 0, 0, 0, 0, false);

                string fullText = await Windows.Storage.FileIO.ReadTextAsync(fiel, Windows.Storage.Streams.UnicodeEncoding.Utf8);

                System.Xml.Linq.XDocument xmlFile = null;
                try
                {
                    xmlFile = System.Xml.Linq.XDocument.Parse(fullText);

                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("No valid XML File falling back to text parser!");
                }

                if (xmlFile == null)
                {
                    IList<String> text = await Windows.Storage.FileIO.ReadLinesAsync(fiel, Windows.Storage.Streams.UnicodeEncoding.Utf8);

                    String[] FileLines = text.ToArray();
                    MindNode selectedNode = null;

                    Stack<MindNode> Selnodes = new Stack<MindNode>();
                    Stack<int> connIds = new Stack<int>();
                    Stack<Color> conncolors = new Stack<Color>();
                    Stack<String> connnames = new Stack<String>();

                    for (int i = 0; i < FileLines.Length; i++)
                    {
                        String[] atts = FileLines[i].Split(':', '/');
                        switch (atts[0])
                        {
                            case "ID":
                                selectedNode = newMasterNode.GetExistingNode(int.Parse(atts[1]));
                                break;
                            case "TextPos":
                                if (selectedNode != null)
                                {
                                    selectedNode.textpos.X = int.Parse(atts[1]);
                                    selectedNode.textpos.Y = int.Parse(atts[2]);
                                }
                                break;
                            case "TextFormat":
                                if (selectedNode != null)
                                {
                                    CanvasTextFormat newFormat = new CanvasTextFormat();
                                    newFormat.FontSize = int.Parse(atts[1]);
                                    newFormat.FontFamily = atts[2];
                                    if (atts[3].Equals("Italic"))
                                        newFormat.FontStyle = Windows.UI.Text.FontStyle.Italic;
                                    else
                                        newFormat.FontStyle = Windows.UI.Text.FontStyle.Normal;
                                    if (atts[4].Equals("Bold"))
                                        newFormat.FontWeight = FontWeights.Bold;
                                    else
                                        newFormat.FontWeight = FontWeights.Normal;

                                    selectedNode.SetTextStyle(newFormat);
                                    selectedNode.TextColor = GetColorFromHexString(atts[5]);

                                }
                                break;
                            case "Name":
                                if (selectedNode != null)
                                    selectedNode.SetText(atts[1]);
                                break;
                            case "NodeStyle":
                                if (selectedNode != null)
                                {
                                    selectedNode.NodeDrawingStyle = DrawingStyle.ELLIPSE;

                                    if ((atts[1]).Equals(DrawingStyle.BUTTON.ToString()))
                                        selectedNode.NodeDrawingStyle = DrawingStyle.BUTTON;
                                    if ((atts[1]).Equals(DrawingStyle.CIRCLE.ToString()))
                                        selectedNode.NodeDrawingStyle = DrawingStyle.CIRCLE;
                                    if ((atts[1]).Equals(DrawingStyle.ELLIPSEEDGE.ToString()))
                                        selectedNode.NodeDrawingStyle = DrawingStyle.ELLIPSEEDGE;
                                    if ((atts[1]).Equals(DrawingStyle.RECTANGLE.ToString()))
                                        selectedNode.NodeDrawingStyle = DrawingStyle.RECTANGLE;
                                }
                                break;
                            case "Color":
                                if (selectedNode != null)
                                {
                                    selectedNode.TextColor = GetColorFromHexString(atts[1]);
                                    selectedNode.BorderColor = GetColorFromHexString(atts[2]);
                                    selectedNode.NodeColor = GetColorFromHexString(atts[3]);
                                }
                                break;
                            case "Bounds":
                                if (selectedNode != null)
                                {
                                    selectedNode.width = int.Parse(atts[3]);
                                    selectedNode.height = int.Parse(atts[4]);
                                    selectedNode.SetPosition(int.Parse(atts[1]), int.Parse(atts[2]), false);
                                }
                                break;
                            case "Scaled":
                                if (selectedNode != null)
                                {
                                    selectedNode.SetScaled(true);
                                }
                                break;
                            case "ConnNodes":
                                for (int c = 1; c < atts.Length; c++)
                                {
                                    if (atts[c].Length >= 1)
                                    {
                                        Selnodes.Push(selectedNode);
                                        connIds.Push(int.Parse(atts[c]));
                                    }
                                }
                                break;
                            case "ConnColors":
                                for (int c = 1; c < atts.Length; c++)
                                {
                                    if (atts[c].Length >= 1)
                                    {
                                        conncolors.Push(GetColorFromHexString(atts[c]));
                                    }
                                }
                                break;
                            case "ConnNames":
                                for (int c = 1; c < atts.Length; c++)
                                {
                                    if (atts[c].Length >= 1)
                                    {
                                        connnames.Push(atts[c]);
                                    }
                                }
                                break;
                            case "Childs":
                                for (int c = 1; c < atts.Length; c++)
                                {
                                    if (atts[c].Length >= 1)
                                    {
                                        MindNode newNode = newMasterNode.GetExistingNode(int.Parse(atts[c]));
                                        if (newNode == null)
                                        {
                                            newNode = new MindNode(int.Parse(atts[c]), 0, 0, 0, 0, false);
                                        }
                                        selectedNode.AddChild(newNode, true);
                                    }
                                }

                                break;

                        }
                    }

                    MindNode m;
                    String connText;
                    Color ncolor;
                    while (Selnodes.Count > 0)
                    {
                        m = newMasterNode.GetExistingNode(connIds.Pop());
                        connText = connnames.Pop();
                        ncolor = conncolors.Pop();
                        MindNode selectednode = Selnodes.Pop();
                        selectednode.AddConnection(m, ncolor, connText);
                    }

                }

                GlobalNodeHandler.masterNode = newMasterNode;

                if(xmlFile != null)
                    GlobalNodeHandler.masterNode.FromRepresentation(xmlFile);

                GlobalNodeHandler.masterNode.UpdateAllWidths();

            }
            catch
            {

            }

        }

        public static Color GetColorFromHexString(string hexValue)
        {
            String astring = hexValue.Substring(1, 2);
            String rstring = hexValue.Substring(3, 2);
            String gstring = hexValue.Substring(5, 2);
            String bstring = hexValue.Substring(7, 2);

            var a = Convert.ToByte(hexValue.Substring(1, 2), 16);
            var r = Convert.ToByte(hexValue.Substring(3, 2), 16);
            var g = Convert.ToByte(hexValue.Substring(5, 2), 16);
            var b = Convert.ToByte(hexValue.Substring(7, 2), 16);
            return Color.FromArgb(a, r, g, b);
        }

        public MindNode RestoreNode(String NodeText)
        {

            
                MindNode newMasterNode = new MindNode(0, 0, 0, 0, 0, false);

            try
            {
                String[] FileLines = NodeText.Split('\n');
                MindNode selectedNode = null;

                Stack<MindNode> Selnodes = new Stack<MindNode>();
                Stack<int> connIds = new Stack<int>();
                Stack<Color> conncolors = new Stack<Color>();
                Stack<String> connnames = new Stack<String>();

                bool masterfound = false;

                for (int i = 0; i < FileLines.Length; i++)
                {
                    String[] atts = FileLines[i].Split(':', '/');
                    switch (atts[0])
                    {
                        case "ID":
                            selectedNode = newMasterNode.GetExistingNode(int.Parse(atts[1]));
                            if (selectedNode == null)
                            {
                                selectedNode = new MindNode(0, 0, 0, 0, 0, false);
                            }
                            break;
                        case "TextPos":
                            if (selectedNode != null)
                            {
                                selectedNode.textpos.X = int.Parse(atts[1]);
                                selectedNode.textpos.Y = int.Parse(atts[2]);
                            }
                            break;
                        case "TextFormat":
                            if (selectedNode != null)
                            {
                                CanvasTextFormat newFormat = new CanvasTextFormat();
                                newFormat.FontSize = int.Parse(atts[1]);
                                newFormat.FontFamily = atts[2];
                                if (atts[3].Equals("Italic"))
                                    newFormat.FontStyle = Windows.UI.Text.FontStyle.Italic;
                                else
                                    newFormat.FontStyle = Windows.UI.Text.FontStyle.Normal;
                                if (atts[4].Equals("Bold"))
                                    newFormat.FontWeight = FontWeights.Bold;
                                else
                                    newFormat.FontWeight = FontWeights.Normal;

                                selectedNode.SetTextStyle(newFormat);
                                selectedNode.TextColor = GetColorFromHexString(atts[5]);

                            }
                            break;
                        case "Name":
                            if (selectedNode != null)
                                selectedNode.SetText(atts[1]);
                            break;
                        case "NodeStyle":
                            if (selectedNode != null)
                            {
                                selectedNode.NodeDrawingStyle = DrawingStyle.ELLIPSE;

                                if ((atts[1]).Equals(DrawingStyle.BUTTON.ToString()))
                                    selectedNode.NodeDrawingStyle = DrawingStyle.BUTTON;
                                if ((atts[1]).Equals(DrawingStyle.CIRCLE.ToString()))
                                    selectedNode.NodeDrawingStyle = DrawingStyle.CIRCLE;
                                if ((atts[1]).Equals(DrawingStyle.ELLIPSEEDGE.ToString()))
                                    selectedNode.NodeDrawingStyle = DrawingStyle.ELLIPSEEDGE;
                                if ((atts[1]).Equals(DrawingStyle.RECTANGLE.ToString()))
                                    selectedNode.NodeDrawingStyle = DrawingStyle.RECTANGLE;
                            }
                            break;
                        case "Color":
                            if (selectedNode != null)
                            {
                                selectedNode.TextColor = GetColorFromHexString(atts[1]);
                                selectedNode.BorderColor = GetColorFromHexString(atts[2]);
                                selectedNode.NodeColor = GetColorFromHexString(atts[3]);
                            }
                            break;
                        case "Bounds":
                            if (selectedNode != null)
                            {
                                selectedNode.width = int.Parse(atts[3]);
                                selectedNode.height = int.Parse(atts[4]);
                                selectedNode.SetPosition(int.Parse(atts[1]), int.Parse(atts[2]), false);
                            }
                            break;
                        case "Scaled":
                            if (selectedNode != null)
                            {
                                selectedNode.SetScaled(true);
                            }
                            break;
                        case "ConnNodes":
                            for (int c = 1; c < atts.Length; c++)
                            {
                                if (atts[c].Length >= 1)
                                {
                                    Selnodes.Push(selectedNode);
                                    connIds.Push(int.Parse(atts[c]));
                                }
                            }
                            break;
                        case "ConnColors":
                            for (int c = 1; c < atts.Length; c++)
                            {
                                if (atts[c].Length >= 1)
                                {
                                    conncolors.Push(GetColorFromHexString(atts[c]));
                                }
                            }
                            break;
                        case "ConnNames":
                            for (int c = 1; c < atts.Length; c++)
                            {
                                if (atts[c].Length >= 1)
                                {
                                    connnames.Push(atts[c]);
                                }
                            }
                            break;
                        case "Childs":
                            for (int c = 1; c < atts.Length; c++)
                            {
                                if (atts[c].Length >= 1)
                                {
                                    MindNode newNode = newMasterNode.GetExistingNode(int.Parse(atts[c]));
                                    if (newNode == null)
                                    {
                                        newNode = new MindNode(int.Parse(atts[c]), 0, 0, 0, 0, false);
                                    }
                                    selectedNode.AddChild(newNode, true);
                                }
                            }

                            if (!masterfound)
                            {
                                newMasterNode = selectedNode;
                                masterfound = true;
                            }

                            break;

                    }
                }

                newMasterNode.DeleteAllConnections();

                MindNode m;
                String connText;
                Color ncolor;
                while (Selnodes.Count > 0)
                {
                    m = newMasterNode.GetExistingNode(connIds.Pop());
                    connText = connnames.Pop();
                    ncolor = conncolors.Pop();
                    MindNode selectednode = Selnodes.Pop();
                    selectednode.AddConnection(m, ncolor, connText);
                    selectednode.UpdateBridgesRepresentation();
                }

                

            }
            catch (Exception e)
            {

            }
            return newMasterNode;

            
        }

    }
}
