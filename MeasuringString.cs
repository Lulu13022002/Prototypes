using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CheckURI.Tools
{
    public static class MeasuringString
    {
        public static Size MeasureString(this TextBox tb) => MeasureString(tb, tb.Text);
        public static Size MeasureString(this TextBox tb, string text)
        {
            var formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch), tb.FontSize, Brushes.Black,
                new NumberSubstitution(NumberCultureSource.User, CultureInfo.CurrentCulture, NumberSubstitutionMethod.Traditional), 1);

            return new Size(formattedText.Width, formattedText.Height);
        }

        public static Size MeasureString(this TextBlock tb) => MeasureString(tb, tb.Text);
        public static Size MeasureString(this TextBlock tb, string text)
        {
            var formattedText = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch), tb.FontSize, Brushes.Black,
                new NumberSubstitution(NumberCultureSource.User, CultureInfo.CurrentCulture, NumberSubstitutionMethod.Traditional), 1);

            return new Size(formattedText.Width, formattedText.Height);
        }
    }
}
