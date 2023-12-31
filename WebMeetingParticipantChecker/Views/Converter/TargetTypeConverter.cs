﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace WebMeetingParticipantChecker.Views.Converter
{
    public class TargetTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not string ParameterString)
            {
                return System.Windows.DependencyProperty.UnsetValue;
            }

            if (Enum.IsDefined(value.GetType(), value) == false)
            {
                return System.Windows.DependencyProperty.UnsetValue;
            }

            object paramvalue = Enum.Parse(value.GetType(), ParameterString);

            return (int)paramvalue == (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter is not string ParameterString ? System.Windows.DependencyProperty.UnsetValue : Enum.Parse(targetType, ParameterString);
        }
    }
}
