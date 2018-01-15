using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MindNoderPort
{
    public class Font
    {
        public int Size = 0;
        public String fontfamily = "";

        public Font(String FontFamily, int Size)
        {
            this.Size = Size;
            this.fontfamily = FontFamily;
        }

        /*public static CalcPoint MeasureText(string candidate)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(this.textBlock.FontFamily, this.textBlock.FontStyle, this.textBlock.FontWeight, this.textBlock.FontStretch),
                this.textBlock.FontSize, Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }*/
    }
}
