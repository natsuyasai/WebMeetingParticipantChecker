using System.Text.RegularExpressions;

namespace WebMeetingParticipantChecker.Utils
{

    public static class StringUtils
    {
        /// <summary>
        /// スペースを除去した文字列を取得
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static string RemoveSpace(string src)
            => Regex.Replace(src, @"[(\s|　)]+", "");

        /// <summary>
        /// スペースの前後を入れ替える
        /// </summary>
        /// <returns></returns>
        public static bool TrySwapBeforeAndAfterTheSpace(string src, out string result)
        {
            var trimSrc = src.Trim();
            var fwSpaceIndex = trimSrc.IndexOf('　');
            var hwSpaceIndex = trimSrc.IndexOf(' ');

            // 両方あれば半角スペースで処理する
            if (hwSpaceIndex > -1)
            {
                var a = trimSrc.Substring(0, hwSpaceIndex);
                var b = trimSrc.Substring(hwSpaceIndex + 1, trimSrc.Length - (hwSpaceIndex + 1));
                result = b + a;
                return true;
            }
            else if (fwSpaceIndex > -1)
            {
                var a = trimSrc.Substring(0, fwSpaceIndex);
                var b = trimSrc.Substring(fwSpaceIndex + 1, trimSrc.Length - (fwSpaceIndex + 1));
                result = b + a;
                return true;
            }
            else
            {
                result = src;
                return false;
            }
        }
    }
}