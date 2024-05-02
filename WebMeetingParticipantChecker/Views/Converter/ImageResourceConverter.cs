using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WebMeetingParticipantChecker.Models.Config;
using WebMeetingParticipantChecker.Models.Theme;

namespace WebMeetingParticipantChecker.Views.Converter
{
    public class ImageResourceConverter : IValueConverter
    {
        private readonly string BasePath = "../Resources/Images/Action";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">基本的には未使用。分岐が必要な場合のみparameterではなくvalueの値を用いて結果を生成する</param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not string ParameterString)
            {
                return System.Windows.DependencyProperty.UnsetValue;
            }

            if (string.IsNullOrEmpty(ParameterString))
            {
                ParameterString = (string)value;
            }

            var currentId = AppSettingsManager.CurrentThemeId;
            if (currentId == null)
            {
                return $"{BasePath}/Light/{ParameterString}";
            }
            return currentId == (int)ThemeDefine.ThmeValue.Dark ? $"{BasePath}/Dark/{ParameterString}" : $"{BasePath}/Light/{ParameterString}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter is not string ParameterString ? System.Windows.DependencyProperty.UnsetValue : Enum.Parse(targetType, ParameterString);
        }
    }
}
