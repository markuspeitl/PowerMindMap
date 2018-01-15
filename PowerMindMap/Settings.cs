using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace MindNoderPort
{
    public class Settings
    {
        FileManager filemanager;

        public Settings()
        {
            filemanager = new FileManager();

        }

        private String path = ""; 
        public void saveMap()
        {
            if (savepath == null)
            {
                saveMapAs();
            }
            else
            {
                
                filemanager.WriteFile(path, savepath);
            }
        }

        IStorageFile savepath;
        public async void saveMapAs()
        {
            FileSavePicker saveFileDialog1 = new FileSavePicker();

            // Set filter options and filter index.
            saveFileDialog1.FileTypeChoices.Add("PowerMindMap Format", new List<string>() { ".ppm" });

            Windows.Storage.StorageFile file = await saveFileDialog1.PickSaveFileAsync();
            // Process input if the user clicked OK.
            if (file != null)
            {
                filemanager.WriteFile(file.Path,file);

                savepath = file;
            }
        }

        private int maxAutoSaves = 20;
        public async Task autoSaveMap()
        {
            try
            {
                String now = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                if (localFolder != null)
                {
                    IReadOnlyList<StorageFile> dirFiles = await localFolder.GetFilesAsync();

                    if (dirFiles != null)
                    {
                        if (dirFiles.Count > maxAutoSaves)
                        {
                            String filename = dirFiles.First().Name;
                            StorageFile firstfile = await localFolder.GetFileAsync(filename);
                            await firstfile.DeleteAsync();
                        }
                    }
                    String name = "Autosave" + now + ".ppm";
                    Windows.Storage.StorageFile file = await localFolder.CreateFileAsync(name);
                    filemanager.WriteFile(path, file);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Autosave failed: Message:" + e.Message);
            }
        }

        public async Task openAutoSave()
        {
            try
            {
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                IReadOnlyList<StorageFile> dirFiles = await localFolder.GetFilesAsync();

                if (dirFiles != null)
                {
                    if (dirFiles.Count > 0)
                    {
                        String name = dirFiles.Last().Name;
                        StorageFile last = await localFolder.GetFileAsync(name);

                        // Process input if the user clicked OK.
                        if (last != null)
                        {
                            await filemanager.ReadFile(last.Path, last);
                            GlobalNodeHandler.viewNode = GlobalNodeHandler.masterNode;
                            GlobalNodeHandler.viewNode.UpdateViewRepresentation();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Could not open AutoSave: Message:" + e.Message);
            }
        }

        public async Task openMap()
        {
            // Create an instance of the open file dialog box.
            FileOpenPicker openFileDialog1 = new FileOpenPicker();

            // Set filter options and filter index.

            openFileDialog1.FileTypeFilter.Add(".ppm");

            openFileDialog1.FileTypeFilter.Add(".mxm");

            Windows.Storage.StorageFile file = await openFileDialog1.PickSingleFileAsync();

            // Process input if the user clicked OK.
            if (file != null)
            {
                await filemanager.ReadFile(file.Path, file);
                GlobalNodeHandler.viewNode = GlobalNodeHandler.masterNode;
                GlobalNodeHandler.viewNode.UpdateViewRepresentation();
            }
        }

        public void newMap()
        {
            GlobalNodeHandler.masterNode = new MindNode(0, 0, 0, 0, 0, false);
            GlobalNodeHandler.viewNode = GlobalNodeHandler.masterNode;
        }

        /*var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                this.textBlock.Text = "Picked photo: " + file.Name;
            }
            else
            {
                this.textBlock.Text = "Operation cancelled.";
            }*/

        public async void exportMap(CanvasControl sender)
        {
            /*Bitmap myBitmap;
            ImageCodecInfo myImageCodecInfo;
            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;*/

            int imageBorder = (int)(100 * GlobalNodeHandler.viewNode.scale);
            double sourceScale = GlobalNodeHandler.viewNode.scale;
            //double exportScale = 1.0d;

            FileSavePicker saveFileDialog1 = new FileSavePicker();

            int height = GlobalNodeHandler.viewNode.GetMinMaxSpanY();
            int width = GlobalNodeHandler.viewNode.GetMinMaxSpanX();
            int minx = GlobalNodeHandler.viewNode.GetMinChildX().xpos;
            int miny = GlobalNodeHandler.viewNode.GetMinChildY().ypos;

            // Set filter options and filter index.
            saveFileDialog1.FileTypeChoices.Add("Image File| JPG", new List<string>() { ".jpeg" });
            saveFileDialog1.FileTypeChoices.Add("Image File| JPEG", new List<string>() { ".jpg" });
            saveFileDialog1.FileTypeChoices.Add("Image File| PNG", new List<string>() { ".png" });
            saveFileDialog1.FileTypeChoices.Add("Image File| GIF", new List<string>() { ".gif" });
            saveFileDialog1.FileTypeChoices.Add("Image File| BMP", new List<string>() { ".bmp" });
            saveFileDialog1.FileTypeChoices.Add("Image File| Tiff", new List<string>() { ".tiff" });
            //saveFileDialog1.FileTypeChoices.Add("PDF Document", new List<string>() { ".pdf" });

            if (height > -1 && width > -1)
            {
                CanvasDevice device = CanvasDevice.GetSharedDevice();
                //CanvasRenderTarget savemap = new CanvasRenderTarget(device, width, height, 96);
                CanvasRenderTarget savemap = new CanvasRenderTarget(device, width + 3* imageBorder, height + 2* imageBorder, 96);

                using (CanvasDrawingSession g2d = savemap.CreateDrawingSession())
                {
                    g2d.Clear(Colors.White);

                    g2d.Antialiasing = CanvasAntialiasing.Antialiased;

                    //GlobalNodeHandler.viewNode.SetNodeScale(exportScale);
                    //GlobalNodeHandler.viewNode.UpdateViewRepresentation();

                    height = GlobalNodeHandler.viewNode.GetMinMaxSpanY();
                    width = GlobalNodeHandler.viewNode.GetMinMaxSpanX();
                    minx = GlobalNodeHandler.viewNode.GetMinChildX().xpos;
                    miny = GlobalNodeHandler.viewNode.GetMinChildY().ypos;

                    if(GlobalNodeHandler.viewNode.height != 0 && GlobalNodeHandler.viewNode.width != 0)
                        GlobalNodeHandler.viewNode.DrawRepresentationAt(g2d, 5, 5);
                    GlobalNodeHandler.viewNode.drawView(sender, g2d, -minx + imageBorder, -miny+ imageBorder);
                }

                //GlobalNodeHandler.viewNode.SetNodeScale(sourceScale);
                //GlobalNodeHandler.viewNode.UpdateViewRepresentation();

                Windows.Storage.StorageFile newfile = await saveFileDialog1.PickSaveFileAsync();

                if (newfile != null)
                {
                    CanvasBitmapFileFormat fileformat = CanvasBitmapFileFormat.Bmp;
                    if (newfile.FileType.Equals(".jpeg") || newfile.FileType.Equals(".jpg"))
                    {
                        fileformat = CanvasBitmapFileFormat.JpegXR;
                    }
                    if (newfile.FileType.Equals(".png"))
                    {
                        fileformat = CanvasBitmapFileFormat.Png;
                    }
                    if (newfile.FileType.Equals(".bmp"))
                    {
                        fileformat = CanvasBitmapFileFormat.Bmp;
                    }
                    if (newfile.FileType.Equals(".gif"))
                    {
                        fileformat = CanvasBitmapFileFormat.Gif;
                    }
                    if (newfile.FileType.Equals(".tiff"))
                    {
                        fileformat = CanvasBitmapFileFormat.Tiff;
                    }

                    using (var output = await newfile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await savemap.SaveAsync(output, fileformat);
                        await output.FlushAsync();
                    }
                }


            }
            
                /*myImageCodecInfo = GetEncoderInfo("image/jpeg");

                if (ext.Equals("jpg"))
                {
                    // Get an ImageCodecInfo object that represents the JPEG codec.
                    myImageCodecInfo = GetEncoderInfo("image/jpeg");

                }
                if (ext.Equals("bmp"))
                {
                    // Get an ImageCodecInfo object that represents the JPEG codec.
                    myImageCodecInfo = GetEncoderInfo("image/bmp");

                }

                // for the Quality parameter category.
                myEncoder = System.Drawing.Imaging.Encoder.Quality;

                myEncoderParameters = new EncoderParameters(1);


                // Save the bitmap as a JPEG file with quality level 75.
                myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                myBitmap.Save(saveFileDialog1.InitialDirectory + saveFileDialog1.FileName, myImageCodecInfo, myEncoderParameters);
                
            }*/
        }

        /*public static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }*/

        public void ShowMapStatistics()
        {
            /*Form frm = new Form();

            Label l = new Label();
            l.Text = "NodeCount:" + MindNoder.masterNode.CountNodes();
            l.Location = new Point(10, 10);
            frm.Controls.Add(l);

            Label l2 = new Label();
            l2.Text = "ConnectionCount:" + MindNoder.masterNode.CountConns();
            l2.Location = new Point(10, 20);
            frm.Controls.Add(l2);

            Label l3 = new Label();
            l3.Text = "ViewNodeCount:" + MindNoder.viewNode.CountViewNodes();
            l3.Location = new Point(10, 30);
            frm.Controls.Add(l3);

            frm.Show();*/
        }

        public void CopyToClipboard(MindNode copyNode)
        {
            DataPackage package = new DataPackage();
            package.SetText(copyNode.GetRepresentationText());
            Clipboard.SetContent(package);
        }

        public async Task RestoreFromClipboard(int posx, int posy)
        {
            DataPackageView dataPackage = Clipboard.GetContent();
            if (dataPackage != null)
            {
                if (dataPackage.Contains(StandardDataFormats.Text))
                {
                    String NodeText = await dataPackage.GetTextAsync();
                    MindNode clipNode = filemanager.RestoreNode(NodeText);
                    //string test = "";
                    if (clipNode.height >0 && clipNode.width >0)
                    {
                        clipNode.SetPosition(posx, posy, true);
                        clipNode.updateRepresentation();
                        clipNode.ReIDChilds();
                        GlobalNodeHandler.viewNode.AddChild(clipNode, false);
                        GlobalNodeHandler.actionLog.AddAction(new MindNodeAction(0, "CreateNode", clipNode));
                    }
                }
            }
        }
    }
}
