using System.Globalization;

namespace MassageBookingApp.Mobile.Helpers
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var isCurrentMonth = value is bool b && b;

            return isCurrentMonth
                ? Color.FromArgb("#FFFFFF")
                : Color.FromArgb("#F4F4F4");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is bool b ? !b : true;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectedToBackgroundColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var isSelected = value is bool b && b;
            return isSelected ? Color.FromArgb("#3FA7D6") : Color.FromArgb("#E8EEF7");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectedToTextColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var isSelected = value is bool b && b;
            return isSelected ? Colors.White : Color.FromArgb("#1F2937");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectedPageMultiConverter : IMultiValueConverter
    {
        public object Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values is not null && values.Length == 2)
            {
                var pageNumber = values[0];
                var selectedPage = values[1];

                return pageNumber?.Equals(selectedPage) == true
                    ? Colors.CornflowerBlue
                    : Colors.LightGray;
            }

            return Colors.LightGray;
        }

        public object?[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }

    public class NotNullConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToIsEnabledCollapseColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool isEnabled = value is bool b && b;
            if (parameter?.ToString() == "Color")
            {
                return isEnabled ? Colors.CadetBlue : Colors.LightGrey;
            }
            else
            {
                return isEnabled ? "CadetBlue" : "LightGrey";
            }
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
