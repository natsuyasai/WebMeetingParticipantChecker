using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace WebMeetingParticipantChecker.Models.Preset
{
    /// <summary>
    /// プリセット情報
    /// </summary>
    internal partial class PresetInfo
    {
        /// <summary>
        /// 識別子 
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// ファイルパス
        /// </summary>
        public string FilePath { get; private set; }
        /// <summary>
        /// プリセット名
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// プリセットデータ
        /// </summary>
        public IEnumerable<string> Data { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filepath"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public PresetInfo(int id, string filepath, string name, IEnumerable<string> data)
        {
            Id = id;
            FilePath = filepath;
            Name = name;
            Data = data;
        }

        /// <summary>
        /// 自身か
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public bool IsMine(int targetId)
            => Id == targetId;

        /// <summary>
        /// 自然順ソート用
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        internal static partial class SafeNativeMethods
        {
            [LibraryImport("shlwapi.dll", StringMarshalling = StringMarshalling.Utf16)]
            public static partial int StrCmpLogicalW(string psz1, string psz2);
        }

        public sealed class PresetInfoNaturalStringComparer : IComparer<PresetInfo>
        {
            public int Compare(PresetInfo? a, PresetInfo? b)
            {
                return SafeNativeMethods.StrCmpLogicalW(a?.Name ?? "", b?.Name ?? "");
            }
        }
    }

    /// <summary>
    /// エラー時のプリセット
    /// </summary>
    internal class ErrorPresetInfo : PresetInfo
    {
        public ErrorPresetInfo() : base(0, "", "読み込み失敗", new List<string>())
        {
        }
    }
}
