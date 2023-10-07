using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMeetingParticipantChecker.Utils
{
    public class InstanceCopyUtils
    {
        public static T? DeepCopy<T>(T? self)
        {
            if (self == null)
            {
                return default;
            }
            // インスタンスを作成
            var instance = Activator.CreateInstance(typeof(T));
            var type = self.GetType();

            // データクラスはプロパティで構成されているのでプロパティを取得しています。
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                property.SetValue(instance, property.GetValue(self));
            }
            return (T?)instance;
        }
    }
}
