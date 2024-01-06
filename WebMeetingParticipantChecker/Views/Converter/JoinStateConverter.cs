using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Models.Theme;
using static WebMeetingParticipantChecker.Models.Monitoring.MonitoringInfo;

namespace WebMeetingParticipantChecker.Views.Converter
{
    public class JoinStateConverter : IValueConverter
    {
        private readonly string[] JoinStatusImage_Dark = new string[(int)JoinState.Max]
        {
            "/Resources/Images/join.png",
            "/Resources/Images/unjoin.png",
            "/Resources/Images/manual_join0.png",
            "/Resources/Images/manual_unjoin0.png",
        };

        private readonly string[] JoinStatusImage_Light = new string[(int)JoinState.Max]
        {
            "/Resources/Images/join.png",
            "/Resources/Images/unjoin.png",
            "/Resources/Images/manual_join1.png",
            "/Resources/Images/manual_unjoin1.png",
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Enum.IsDefined(value.GetType(), value) == false)
            {
                return System.Windows.DependencyProperty.UnsetValue;
            }

            var currentId = AppSettingsManager.CurrentThemeId;
            if (currentId == null)
            {
                return JoinStatusImage_Light[(int)value];
            }
            return currentId == (int)ThemeDefine.ThmeValue.Dark ? JoinStatusImage_Dark[(int)value] : JoinStatusImage_Light[(int)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter is not string ParameterString ? System.Windows.DependencyProperty.UnsetValue : Enum.Parse(targetType, ParameterString);
        }
    }
}
