using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI.Text;
using System.Diagnostics;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel;


// Die Vorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 dokumentiert.

namespace MindNoderPort
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        static Rect selectionRec = new Rect(0, 0, 0, 0);

        MouseHandler mouse;
        KeyHandler keymanager;
        ButtonManager btnMngr;
        List<FontClass> MyFontPicker;
        List<int> MyFontSizePicker;

        Rect sourcesize;
        int refheight = 1080;
        int refwidth = 1920;

        
        public MainPage()
        {
            this.InitializeComponent();

            sourcesize = Window.Current.CoreWindow.Bounds;

            GlobalNodeHandler.standardtextFormat = new CanvasTextFormat();

            MyFontPicker = new List<FontClass>();
            MyFontPicker.Add(new FontClass() { MyFontFamily = new FontFamily("Arial"), FontFamilyValue = "Arial" });
            MyFontPicker.Add(new FontClass() { MyFontFamily = new FontFamily("Times New Roman"), FontFamilyValue = "Times New Roman" });
            MyFontPicker.Add(new FontClass() { MyFontFamily = new FontFamily("Verdana"), FontFamilyValue = "Verdana" });
            MyFontPicker.Add(new FontClass() { MyFontFamily = new FontFamily("Calibri"), FontFamilyValue = "Calibri" });
            MyFontPicker.Add(new FontClass() { MyFontFamily = new FontFamily("Rockwell"), FontFamilyValue = "Rockwell" });
            MyFontPicker.Add(new FontClass() { MyFontFamily = new FontFamily("Twentieth Century"), FontFamilyValue = "Twentieth Century" });
            MyFontPicker.Add(new FontClass() { MyFontFamily = new FontFamily("Helvetica"), FontFamilyValue = "Helvetica" });
            MyFontPicker.Add(new FontClass() { MyFontFamily = new FontFamily("Trajan"), FontFamilyValue = "Trajan" });
            MyFontPicker.Add(new FontClass() { MyFontFamily = new FontFamily("Optima Std"), FontFamilyValue = "Optima Std" });
            MyFontPicker.Add(new FontClass() { MyFontFamily = new FontFamily("Franklin Gothic"), FontFamilyValue = "Franklin Gothic" });
            MyFontPicker.Add(new FontClass() { MyFontFamily = new FontFamily("Futura"), FontFamilyValue = "Futura" });
            MyFontPicker.Add(new FontClass() { MyFontFamily = new FontFamily("Bickham Script"), FontFamilyValue = "Bickham Script" });
            MyFontPicker.Add(new FontClass() { MyFontFamily = new FontFamily("Gill Sans"), FontFamilyValue = "Gill Sans" });
            MyFontPicker.Add(new FontClass() { MyFontFamily = new FontFamily("Sans Serif"), FontFamilyValue = "Sans Serif" });
            MyFontPicker.Add(new FontClass() { MyFontFamily = new FontFamily("Gotham"), FontFamilyValue = "Gotham" });
            FontStyleBox.ItemsSource = MyFontPicker;
            
            
            MyFontSizePicker = new List<int>();
            for (int i = 6; i < 45; i++)
            {
                MyFontSizePicker.Add(i);
            }
            FontSizeBox.ItemsSource = MyFontSizePicker;

            GlobalNodeHandler.standardtextFormat.FontSize = 18;
            GlobalNodeHandler.standardtextFormat.FontStyle = FontStyle.Normal;
            GlobalNodeHandler.standardtextFormat.FontFamily = new FontFamily("Arial").Source;
            FontSizeBox.SelectedItem = 18;
            FontStyleBox.SelectedIndex = 0;

            GlobalNodeHandler.pick1 = new ColorPicker(5, 5, new Rect(1920 - 200, 50, 200, 260));

            AttributePanel.Margin = new Thickness(0, 260, refwidth - sourcesize.Width, 0);
            UseModePanel.Margin = new Thickness(0, 0, refwidth - sourcesize.Width, 0);

            //AttributePanel.Margin = new Thickness(0, 260, refwidth - sourcesize.Width, 0);
            GlobalNodeHandler.pick1.boundingbox.X = refwidth - (refwidth - sourcesize.Width) - GlobalNodeHandler.pick1.boundingbox.Width;
            repaint(this, new EventArgs());

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

            GlobalNodeHandler.masterNode = new MindNode(GlobalNodeHandler.id, 0, 0, 0, 0, false);
            GlobalNodeHandler.id++;
            GlobalNodeHandler.masterNode.text = "masterNode";

            GlobalNodeHandler.viewNode = GlobalNodeHandler.masterNode;

            Application.Current.DebugSettings.EnableFrameRateCounter = false;

            activetext.KeyDown += new KeyEventHandler(activeTextfield_KeyDown);
            activetext.Visibility = Visibility.Collapsed;

            DispatcherTimer timer1 = new DispatcherTimer();
            timer1.Interval = new TimeSpan(1000 / 8);
            timer1.Start();
            timer1.Tick += this.timer1_Tick;


            mouse = new MouseHandler(activetext);
            mouse.repainted += this.repaint;

            keymanager = new KeyHandler(activetext);
            keymanager.repainted += this.repaint;

            btnMngr = new ButtonManager();

            viewPane.PointerPressed += this.viewPane_MouseDown;
            viewPane.PointerReleased += this.viewPane_MouseUp;

            AddNode.AddHandler(PointerPressedEvent, new PointerEventHandler(AddNode_PointerPressed), true);
            ConnectNodes.AddHandler(PointerPressedEvent, new PointerEventHandler(ConnectNodes_PointerPressed), true);
            DeConnectNodes.AddHandler(PointerPressedEvent, new PointerEventHandler(DeConnectNodes_PointerPressed), true);
            TransformNode.AddHandler(PointerPressedEvent, new PointerEventHandler(TransformNode_PointerPressed), true);
            MoveNodes.AddHandler(PointerPressedEvent, new PointerEventHandler(MoveNodes_PointerPressed), true);
            DeleteNode.AddHandler(PointerPressedEvent, new PointerEventHandler(DeleteNode_PointerPressed), true);
            SelectNodes.AddHandler(PointerPressedEvent, new PointerEventHandler(SelectNodes_PointerPressed), true);
            UnDo.AddHandler(PointerPressedEvent, new PointerEventHandler(UnDo_PointerPressed), true);
            ReDo.AddHandler(PointerPressedEvent, new PointerEventHandler(ReDo_PointerPressed), true);
            Copy.AddHandler(PointerPressedEvent, new PointerEventHandler(Copy_PointerPressed), true);
            Cut.AddHandler(PointerPressedEvent, new PointerEventHandler(Cut_PointerPressed), true);
            Paste.AddHandler(PointerPressedEvent, new PointerEventHandler(Paste_PointerPressed), true);
            PlaceLabel.AddHandler(PointerPressedEvent, new PointerEventHandler(PlaceLabel_PointerPressed), true);
            JumpInto.AddHandler(PointerPressedEvent, new PointerEventHandler(JumpInto_PointerPressed), true);
            JumpOutof.AddHandler(PointerPressedEvent, new PointerEventHandler(JumpOutof_PointerPressed), true);
            ColorNode.AddHandler(PointerPressedEvent, new PointerEventHandler(ColorNode_PointerPressed), true);

            UNDOMENU.AddHandler(PointerPressedEvent, new PointerEventHandler(UnDo_PointerPressed), true);
            REDOMENU.AddHandler(PointerPressedEvent, new PointerEventHandler(ReDo_PointerPressed), true);
            CUTMENU.AddHandler(PointerPressedEvent, new PointerEventHandler(Cut_PointerPressed), true);
            COPYMENU.AddHandler(PointerPressedEvent, new PointerEventHandler(Copy_PointerPressed), true);
            PASTEMENU.AddHandler(PointerPressedEvent, new PointerEventHandler(Paste_PointerPressed), true);
            TRANSFORMMENU.AddHandler(PointerPressedEvent, new PointerEventHandler(TransformNode_PointerPressed), true);
            EDITMENU.AddHandler(PointerPressedEvent, new PointerEventHandler(PlaceLabel_PointerPressed), true);
            JUMPINMENU.AddHandler(PointerPressedEvent, new PointerEventHandler(JumpInto_PointerPressed), true);
            JUMPOUTMENU.AddHandler(PointerPressedEvent, new PointerEventHandler(JumpOutof_PointerPressed), true);
            CHANGECOLORMENU.AddHandler(PointerPressedEvent, new PointerEventHandler(ColorNode_PointerPressed), true);

            //FileButtonFlyout.AddHandler(PointerPressedEvent, new PointerEventHandler(FileButton_PointerPressed), true);
            this.NEW.AddHandler(PointerPressedEvent, new PointerEventHandler(NEW_PointerPressed), true);
            this.OPEN.AddHandler(PointerPressedEvent, new PointerEventHandler(OPEN_PointerPressed), true);
            this.SAVE.AddHandler(PointerPressedEvent, new PointerEventHandler(SAVE_PointerPressed), true);
            this.SAVEAS.AddHandler(PointerPressedEvent, new PointerEventHandler(SAVEAS_PointerPressed), true);
            this.EXPORT.AddHandler(PointerPressedEvent, new PointerEventHandler(EXPORT_PointerPressed), true);
            this.CLOSE.AddHandler(PointerPressedEvent, new PointerEventHandler(CLOSE_PointerPressed), true);
            this.EXIT.AddHandler(PointerPressedEvent, new PointerEventHandler(EXIT_PointerPressed), true);

            Application.Current.Suspending += AppSuspending;

            STYLESBAR.AddHandler(PointerPressedEvent, new PointerEventHandler(STYLESBAR_PointerPressed), true);
            ZOOMIN.AddHandler(PointerPressedEvent, new PointerEventHandler(ZOOMIN_PointerPressed), true);
            ZOOMOUT.AddHandler(PointerPressedEvent, new PointerEventHandler(ZOOMOUT_PointerPressed), true);

            ABOUT.AddHandler(PointerPressedEvent, new PointerEventHandler(ABOUT_PointerPressed), true);
            DOCU.AddHandler(PointerPressedEvent, new PointerEventHandler(DOCU_PointerPressed), true);

            BoldText.AddHandler(PointerPressedEvent, new PointerEventHandler(UpdateFontStyle), true);
            ItalicText.AddHandler(PointerPressedEvent, new PointerEventHandler(UpdateFontStyle), true);
            UnderlineText.AddHandler(PointerPressedEvent, new PointerEventHandler(UpdateFontStyle), true);

            AboutButton.AddHandler(PointerPressedEvent, new PointerEventHandler(AboutButton_PointerPressed), true);


            viewPane.PointerMoved += viewPane_UpdateMousePos;
            viewPane.PointerWheelChanged += OnMouseScroll;

            timer1.Tick += mouse.timer1_Tick;

            OpenAutoSave();

            repaint(this, new EventArgs());

            /*Random r = new Random();

            for (int x = 0; x < 100; x++) {
                for (int y = 0; y < 100; y++)
                {
                    //mouse.CreateNewNode((int)(r.NextDouble() * 1000), (int)(r.NextDouble() * 1000), true);

                    GlobalNodeHandler.CreateNewNode((int)(y * 50), (int)(x * 30), true);

                    mouse.activetext.Text = "n:";
                    keymanager.EnterTextBox();
                    repaint(this,new EventArgs());
                }
            }*/

            GlobalNodeHandler.TabletSelected = true;
            this.Focus(FocusState.Keyboard);
        }
        
        private async void OpenAutoSave()
        {
            await GlobalNodeHandler.settings.openAutoSave();
            repaint(this, new EventArgs());
        }

        async protected void AppSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            await GlobalNodeHandler.settings.autoSaveMap();
            deferral.Complete();
        }

        void timer_Tick(object sender, object e)
        {
        }
        
        public void activeTextfield_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                keymanager.EnterTextBox();
            }
        }
        

        CalcPoint lastMousePosition = new CalcPoint();
        private void viewPane_UpdateMousePos(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Input.PointerPoint ptrPt = e.GetCurrentPoint((UIElement)(sender));
            lastMousePosition = new CalcPoint(ptrPt.Position);

            //status += "Posx:" + mousePos.X + "/ " + mousePos.Y + "\n";
        }

        private void timer1_Tick(object sender, object e)
        {
            int x = lastMousePosition.X;
            int y = lastMousePosition.Y;
            mouse.currentViewPanePosition = lastMousePosition;
            mouse.currentFormPosition = lastMousePosition;
            //viewPane.Invalidate();
        }

        public static int scrollcnt = 0;
        public static int startcnt = 0;
        public static bool scrolling = false;
        public void OnMouseScroll(object sender, PointerRoutedEventArgs e)
        {
            /*Windows.UI.Input.PointerPoint ptrPt = e.GetCurrentPoint((UIElement)(sender));
            CalcPoint mousePos = new CalcPoint(ptrPt.Position);
            double delta = ptrPt.Properties.MouseWheelDelta;

            startcnt = mouse.tickcnt;
            scrolling = true;

            scrollcnt++;
            if (scrollcnt % 6 == 0)
            {
                GlobalNodeHandler.Zoom((mousePos), delta);
                repaint(this, new EventArgs()); ;
            }
            else
            {
                GlobalNodeHandler.PseudoZoom((mousePos), delta);
                repaint(this, new EventArgs()); ;
            }*/
        }

        public void repaint(object sender, EventArgs e)
        {
            this.viewPane.Invalidate();
        }

        private void viewPane_Draw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args)
        {
            //CanvasDrawingSession drawSurface = args.DrawingSession;

            GlobalNodeHandler.viewNode.DrawRepresentationAt(args.DrawingSession, 10+(int)LeftStackP.Width, 10+(int)TopStackP.Height);

            GlobalNodeHandler.viewNode.drawView(sender, args.DrawingSession);

            if (mouse.dragRec)
            {
                args.DrawingSession.DrawRectangle(mouse.selectionRec, Colors.Blue, 5);
            }

            GlobalNodeHandler.pick1.draw(args.DrawingSession);

            //drawSurface.DrawText(status, 10, 400, Colors.White);


            //args.DrawingSession.DrawText(status2, 800, 0, Colors.Black);

        }
        public static String status = "";
        public static String status2 = "";
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += this_KeyDown;
            Window.Current.CoreWindow.KeyDown += this_KeyUp;
            Window.Current.CoreWindow.SizeChanged += this_SetControlPos;

        }

        public void this_SetControlPos(CoreWindow sender, WindowSizeChangedEventArgs e)
        {
            //status2 += "" + sender.Bounds.Width + "\n";
            AttributePanel.Margin = new Thickness(0,260, refwidth - sender.Bounds.Width, 0);
            UseModePanel.Margin = new Thickness(0, 0, refwidth - sender.Bounds.Width, 0);

            GlobalNodeHandler.pick1.boundingbox.X = refwidth - (refwidth - sender.Bounds.Width) - GlobalNodeHandler.pick1.boundingbox.Width;
            repaint(this,new EventArgs());
        }

        Point arrowdir = new Point(0, 0);
        public void this_KeyDown(CoreWindow sender, KeyEventArgs e)
        {

            Key k = Key.NONE;

            if (e.VirtualKey == Windows.System.VirtualKey.N)
            {
                k = Key.N;
            }
            if (e.VirtualKey == Windows.System.VirtualKey.I)
            {
                k = Key.I;
            }
            if (e.VirtualKey == Windows.System.VirtualKey.O)
            {
                k = Key.O;
            }
            if (e.VirtualKey == Windows.System.VirtualKey.C)
            {
                k = Key.C;
            }
            if (e.VirtualKey == Windows.System.VirtualKey.D)
            {
                k = Key.D;
            }
            if (e.VirtualKey == Windows.System.VirtualKey.A)
            {
                k = Key.A;
            }
            if (e.VirtualKey == Windows.System.VirtualKey.K)
            {
                k = Key.K;
            }
            if (e.VirtualKey == Windows.System.VirtualKey.U)
            {
                k = Key.U;
            }
            if (e.VirtualKey == Windows.System.VirtualKey.R)
            {
                k = Key.R;
            }
            if (e.VirtualKey == Windows.System.VirtualKey.Right)
            {
                k = Key.RIGHT;
            }
            if (e.VirtualKey == Windows.System.VirtualKey.Left)
            {
                k = Key.LEFT;
            }
            if (e.VirtualKey == Windows.System.VirtualKey.Up)
            {
                k = Key.UP;
            }
            if (e.VirtualKey == Windows.System.VirtualKey.Down)
            {
                k = Key.DOWN;
            }

            KeyObject kevent = new KeyObject(k, lastMousePosition);

            keymanager.thisKeyDownExec(kevent);

        }

        public void this_KeyUp(CoreWindow sender, KeyEventArgs e)
        {
            Key k = Key.NONE;
            if (e.VirtualKey == Windows.System.VirtualKey.Right)
            {
                k = Key.RIGHT;
            }
            if (e.VirtualKey == Windows.System.VirtualKey.Left)
            {
                k = Key.LEFT;
            }
            if (e.VirtualKey == Windows.System.VirtualKey.Up)
            {
                k = Key.UP;
            }
            if (e.VirtualKey == Windows.System.VirtualKey.Down)
            {
                k = Key.DOWN;
            }

            KeyObject kevent = new KeyObject(k, lastMousePosition);

            keymanager.thisKeyUpExec(kevent);
        }

        private Pointer leftpointer = null;
        private Pointer rightpointer = null;
        private void viewPane_MouseDown(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Input.PointerPoint ptrPt = e.GetCurrentPoint((UIElement)(sender));
            CalcPoint mousePos = new CalcPoint(ptrPt.Position);
            MouseButton mB = MouseButton.NONE;
            MouseObject mousePoint;

            //status2 += "Mouse Down" + "\n";

            if (ptrPt.Properties.IsRightButtonPressed)
            {
                rightpointer = e.Pointer;
                mB = MouseButton.RIGHT;
            }

            if (ptrPt.Properties.IsLeftButtonPressed)
            {
                leftpointer = e.Pointer;
                mB = MouseButton.LEFT;
            }

            if (ptrPt.Properties.IsMiddleButtonPressed)
            {
                leftpointer = e.Pointer;
                mB = MouseButton.MIDDLE;
            }

            mousePoint = new MouseObject(mB, mousePos);

            mouse.viewPane_Pressed(mousePoint);

        }
        private void viewPane_MouseUp(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Input.PointerPoint ptrPt = e.GetCurrentPoint((UIElement)(sender));
            CalcPoint mousePos = new CalcPoint(ptrPt.Position);
            MouseButton mB = MouseButton.NONE;

            MouseObject mousePoint;

            if (leftpointer != null)
            {
                if (leftpointer.PointerId.Equals(e.Pointer.PointerId))
                {
                    leftpointer = null;
                    mB = MouseButton.LEFT;
                }
            }

            if (rightpointer != null)
            {
                if (rightpointer.PointerId.Equals(e.Pointer.PointerId))
                {
                    rightpointer = null;
                    mB = MouseButton.RIGHT;
                }
            }

            mousePoint = new MouseObject(mB, mousePos);

            mouse.viewPane_Released(mousePoint);

        }

        Color activecolor = Colors.DarkGray;
        Color inactivecolor = Colors.Transparent;
        private void AddNode_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ResetButtons();
            if (btnMngr.ButtonCLicked(Button.ADD))
            {
                this.AddNode.Background = new SolidColorBrush(activecolor);
            }
            else
            {
                this.AddNode.Background = new SolidColorBrush(inactivecolor);
            }
        }

        private void ConnectNodes_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ResetButtons();
            if (btnMngr.ButtonCLicked(Button.CONNECT))
            {
                this.ConnectNodes.Background = new SolidColorBrush(activecolor);
            }
            else
            {
                this.ConnectNodes.Background = new SolidColorBrush(inactivecolor);
            }
        }

        private void DeConnectNodes_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ResetButtons();
            if (btnMngr.ButtonCLicked(Button.DISCONN))
            {
                this.DeConnectNodes.Background = new SolidColorBrush(activecolor);
            }
            else
            {
                this.DeConnectNodes.Background = new SolidColorBrush(inactivecolor);
            }
        }

        private void DeleteNode_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ResetButtons();
            if (btnMngr.ButtonCLicked(Button.DELETE))
            {
                this.DeleteNode.Background = new SolidColorBrush(activecolor);
            }
            else
            {
                this.DeleteNode.Background = new SolidColorBrush(inactivecolor);
            }
        }

        private void MoveNodes_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ResetButtons();
            if (btnMngr.ButtonCLicked(Button.MOVE))
            {
                this.MoveNodes.Background = new SolidColorBrush(activecolor);
            }
            else
            {
                this.MoveNodes.Background = new SolidColorBrush(inactivecolor);
            }
        }

        private void TransformNode_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ResetButtons();
            if (btnMngr.ButtonCLicked(Button.TRANSFORM))
            {
                this.TransformNode.Background = new SolidColorBrush(activecolor);
                GlobalNodeHandler.viewNode.UpdateViewRepresentation();
                repaint(this, new EventArgs());
            }
            else
            {
                this.TransformNode.Background = new SolidColorBrush(inactivecolor);
            }
        }

        private void SelectNodes_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ResetButtons();
            if (btnMngr.ButtonCLicked(Button.SELECT))
            {
                this.SelectNodes.Background = new SolidColorBrush(activecolor);
            }
            else
            {
                this.SelectNodes.Background = new SolidColorBrush(inactivecolor);
            }
        }

        private void UnDo_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            GlobalNodeHandler.actionLog.UndoLast();
            repaint(this,new EventArgs());
        }

        private void ReDo_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            GlobalNodeHandler.actionLog.RedoLast();
            repaint(this, new EventArgs());
        }

        private void JumpInto_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ResetButtons();
            if (btnMngr.ButtonCLicked(Button.JUMPIN))
            {
                this.JumpInto.Background = new SolidColorBrush(activecolor);
            }
            else
            {
                this.JumpInto.Background = new SolidColorBrush(inactivecolor);
            }
        }

        private void JumpOutof_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            GlobalNodeHandler.JumpOut();
            repaint(this, new EventArgs());
        }

        private void Copy_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ResetButtons();
            if (btnMngr.ButtonCLicked(Button.COPY))
            {
                this.Copy.Background = new SolidColorBrush(activecolor);
            }
            else
            {
                this.Copy.Background = new SolidColorBrush(inactivecolor);
            }
        }

        private void Cut_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ResetButtons();
            if (btnMngr.ButtonCLicked(Button.CUT))
            {
                this.Cut.Background = new SolidColorBrush(activecolor);
            }
            else
            {
                this.Cut.Background = new SolidColorBrush(inactivecolor);
            }
        }

        private void Paste_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ResetButtons();
            if (btnMngr.ButtonCLicked(Button.PASTE))
            {
                this.Paste.Background = new SolidColorBrush(activecolor);
            }
            else
            {
                this.Paste.Background = new SolidColorBrush(inactivecolor);
            }
        }

        private void PlaceLabel_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ResetButtons();
            if (btnMngr.ButtonCLicked(Button.ADDLABEL))
            {
                this.PlaceLabel.Background = new SolidColorBrush(activecolor);
            }
            else
            {
                this.PlaceLabel.Background = new SolidColorBrush(inactivecolor);
            }
        }

        public void RestButtonColors()
        {
            this.AddNode.Background = new SolidColorBrush(inactivecolor);
            this.ConnectNodes.Background = new SolidColorBrush(inactivecolor);
            this.DeConnectNodes.Background = new SolidColorBrush(inactivecolor);
            this.DeleteNode.Background = new SolidColorBrush(inactivecolor);
            this.TransformNode.Background = new SolidColorBrush(inactivecolor);
            this.MoveNodes.Background = new SolidColorBrush(inactivecolor);
            this.SelectNodes.Background = new SolidColorBrush(inactivecolor);
            this.PlaceLabel.Background = new SolidColorBrush(inactivecolor);
            this.Cut.Background = new SolidColorBrush(inactivecolor);
            this.Paste.Background = new SolidColorBrush(inactivecolor);
            this.Copy.Background = new SolidColorBrush(inactivecolor);
            this.JumpInto.Background = new SolidColorBrush(inactivecolor);
            this.ColorNode.Background = new SolidColorBrush(inactivecolor);
            repaint(this, new EventArgs());
        }

        public void ResetButtons()
        {
            RestButtonColors();
            GlobalNodeHandler.ResetSelection();
            if (GlobalNodeHandler.selectedButton != Button.NONE)
            {
                btnMngr.ButtonCLicked(GlobalNodeHandler.selectedButton);
            }
        }

        private void MyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFontStyle();
        }

        public class FontClass
        {
            public FontFamily MyFontFamily { get; set; }
            public string FontFamilyValue { get; set; }

        }

        private void FontSizeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateFontStyle();
        }

        private void StackPanel_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        private void FileButton_PointerPressed_1(object sender, PointerRoutedEventArgs e)
        {

        }

        private void StackPanel_PointerPressed_1(object sender, PointerRoutedEventArgs e)
        {

        }

        private void NEW_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            GlobalNodeHandler.settings.newMap();
            repaint(this, new EventArgs());
        }

        private async void OPEN_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            await GlobalNodeHandler.settings.openMap();
            repaint(this, new EventArgs());
        }

        private void SAVE_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            GlobalNodeHandler.settings.saveMap();
        }

        private void SAVEAS_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            GlobalNodeHandler.settings.saveMapAs();
        }

        private void EXPORT_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            GlobalNodeHandler.settings.exportMap(viewPane);
        }

        private void CLOSE_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            GlobalNodeHandler.settings.autoSaveMap();
            GlobalNodeHandler.settings.newMap();
            repaint(this, new EventArgs());
        }

        private void EXIT_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            GlobalNodeHandler.settings.autoSaveMap();
            Application.Current.Exit();
        }

        public void UpdateFontStyle(object sender, PointerRoutedEventArgs e)
        {
            if (FontSizeBox.SelectedItem != null)
            {
                int size = (int)FontSizeBox.SelectedItem;
                GlobalNodeHandler.standardtextFormat.FontSize = size;
            }
            if (FontStyleBox != null)
            {
                FontClass fstyle = (FontClass)FontStyleBox.SelectedItem;
                GlobalNodeHandler.standardtextFormat.FontFamily = fstyle.MyFontFamily.Source;
            }
            GlobalNodeHandler.standardtextFormat.FontStyle = FontStyle.Normal;
            if (GlobalNodeHandler.textbold)
            {
                GlobalNodeHandler.standardtextFormat.FontWeight = FontWeights.Bold;
            }
            if (GlobalNodeHandler.textitalic)
            {
                GlobalNodeHandler.standardtextFormat.FontStyle = FontStyle.Italic;
            }
            if (GlobalNodeHandler.textunderlined)
            {
            }
            if (GlobalNodeHandler.typing)
            {
                if (GlobalNodeHandler.clickedNode != null)
                {
                    //GlobalNodeHandler.clickedNode.SetTextStyle(GlobalNodeHandler.standardtextFormat);
                }
                else if (GlobalNodeHandler.clickedLabel != null)
                {
                    //GlobalNodeHandler.clickedLabel.SetTextStyle(GlobalNodeHandler.standardtextFormat);
                }
            }

        }

        public void UpdateFontStyle()
        {
            if (FontSizeBox.SelectedItem != null)
            {
                int size = (int)FontSizeBox.SelectedItem;
                GlobalNodeHandler.standardtextFormat.FontSize = size;
            }
            if (FontStyleBox.SelectedItem != null)
            {
                FontClass fstyle = (FontClass)FontStyleBox.SelectedItem;
                GlobalNodeHandler.standardtextFormat.FontFamily = fstyle.MyFontFamily.Source;
            }
            GlobalNodeHandler.standardtextFormat.FontStyle = FontStyle.Normal;
            if (GlobalNodeHandler.textbold)
            {
                GlobalNodeHandler.standardtextFormat.FontWeight = FontWeights.Bold;
            }
            if (GlobalNodeHandler.textitalic)
            {
                GlobalNodeHandler.standardtextFormat.FontStyle = FontStyle.Italic;
            }
            if (GlobalNodeHandler.textunderlined)
            {
            }

        }

        public void UpdateDrawingStyle()
        {
            if (((ComboBoxItem)StyleBox.SelectedValue).Content.Equals("ELLIPSE"))
            {
                GlobalNodeHandler.nodeStyle = DrawingStyle.ELLIPSE;
            }
            if (((ComboBoxItem)StyleBox.SelectedValue).Content.Equals("BUTTON"))
            {
                GlobalNodeHandler.nodeStyle = DrawingStyle.BUTTON;
            }
            if (((ComboBoxItem)StyleBox.SelectedValue).Content.Equals("CIRCLE"))
            {
                GlobalNodeHandler.nodeStyle = DrawingStyle.CIRCLE;
            }
            if (((ComboBoxItem)StyleBox.SelectedValue).Content.Equals("RECTANGLE"))
            {
                GlobalNodeHandler.nodeStyle = DrawingStyle.RECTANGLE;
            }
            if (((ComboBoxItem)StyleBox.SelectedValue).Content.Equals("ELLIPSEEDGE"))
            {
                GlobalNodeHandler.nodeStyle = DrawingStyle.ELLIPSEEDGE;
            }

        }

        private void StyleBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDrawingStyle();
        }

        private void STYLESBAR_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (AttributePanel.Visibility.Equals(Visibility.Visible))
            {
                AttributePanel.Visibility = Visibility.Collapsed;
                GlobalNodeHandler.pick1.isVisible = false;
                AttributePanel.IsHitTestVisible = false;
            }
            else
            {
                AttributePanel.Visibility = Visibility.Visible;
                GlobalNodeHandler.pick1.isVisible = true;
                AttributePanel.IsHitTestVisible = true;
            }
            repaint(this, new EventArgs());
        }

        private void SHORTCUTS_PointerPressed(object sender, PointerRoutedEventArgs e)
        {

        }

        private void ZOOMIN_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            double delta = 400d;
            CalcPoint mousePos = new CalcPoint(Window.Current.CoreWindow.Bounds.Width/2, Window.Current.CoreWindow.Bounds.Height / 2);
            
            GlobalNodeHandler.Zoom((mousePos), delta);
            repaint(this, new EventArgs());

        }

        private void ZOOMOUT_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            double delta = -200d;
            CalcPoint mousePos = new CalcPoint(Window.Current.CoreWindow.Bounds.Width / 2, Window.Current.CoreWindow.Bounds.Height / 2);

            GlobalNodeHandler.Zoom((mousePos), delta);
            repaint(this, new EventArgs());
        }

        private void ABOUT_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if(!AboutPopUp.IsOpen)
                AboutPopUp.IsOpen = true;

            Rect windowbounds = Window.Current.CoreWindow.Bounds;
            AboutPopUp.Margin = new Thickness(0, 150, (refwidth - windowbounds.Width) + 500, 0);
        }

        private void DOCU_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
        }

        private void AboutButton_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (AboutPopUp.IsOpen)
                AboutPopUp.IsOpen = false;
        }

        private void ColorNode_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ResetButtons();
            if (btnMngr.ButtonCLicked(Button.COLORNODE))
            {
                this.ColorNode.Background = new SolidColorBrush(activecolor);
            }
            else
            {
                this.ColorNode.Background = new SolidColorBrush(inactivecolor);
            }
        }

        private void bridgecontrol_Checked(object sender, RoutedEventArgs e)
        {
            GlobalNodeHandler.bridgecontrol = this.bridgecontrol.IsChecked.Value;
            if (this.viewPane != null)
            {
                repaint(this, new EventArgs());
            }
        }

        private void bridgecontrol_Unchecked(object sender, RoutedEventArgs e)
        {
            if (this.bridgecontrol != null)
            {
                GlobalNodeHandler.bridgecontrol = this.bridgecontrol.IsChecked.Value;
                if (this.viewPane != null)
                {
                    repaint(this, new EventArgs());
                }
            }
        }

        private void lightText_Checked(object sender, RoutedEventArgs e)
        {
            GlobalNodeHandler.lighttext = true;
        }

        private void lightText_Unchecked(object sender, RoutedEventArgs e)
        {
            GlobalNodeHandler.lighttext = false;
        }
    }

}
