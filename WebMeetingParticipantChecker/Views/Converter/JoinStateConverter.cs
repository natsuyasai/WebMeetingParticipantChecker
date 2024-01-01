using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static WebMeetingParticipantChecker.Models.Monitoring.MonitoringInfo;

namespace WebMeetingParticipantChecker.Views.Converter
{
    public class JoinStateConverter : IValueConverter
    {
        private readonly string[] JoinStatusImage = new string[(int)JoinState.Max]
        {
            "/Resources/Images/join.png",
            "/Resources/Images/unjoin.png",
            "/Resources/Images/manual_join.png",
            "/Resources/Images/manual_unjoin.png",
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Enum.IsDefined(value.GetType(), value) == false)
            {
                return System.Windows.DependencyProperty.UnsetValue;
            }

            return JoinStatusImage[(int)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter is not string ParameterString ? System.Windows.DependencyProperty.UnsetValue : Enum.Parse(targetType, ParameterString);
        }
    }
}
