using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Utils
{
    public static class utils
    {
        private static CanvasController _canvasController;
        public static CanvasController CanvasController
        {
            get
            {
                return _canvasController;
            }
            set { _canvasController = value; }
        }

        public static string GetDataValue(string data, string index)
        {
            string value = data.Substring(data.IndexOf(index, StringComparison.Ordinal) + index.Length);
            if (value.Contains("|"))
                value = value.Remove(value.IndexOf('|'));
            return value;
        }
        public static int GetIntDataValue(string data, string index)
        {
            int numb;
            var success = int.TryParse(GetDataValue(data, index), out numb);

            return success ? numb : 0;
        }
        public static bool GetBoolDataValue(string data, string index)
        {
            var value = GetDataValue(data, index);
            if (string.IsNullOrEmpty(value) || value.Equals("0"))
                return false;
            return true;
        }
        public static long GetLongDataValue(string data, string index)
        {
            long numb;
            var success = long.TryParse(GetDataValue(data, index), out numb);

            return success ? numb : 0;
        }

        public static long GetLongDataValue(string data)
        {
            long numb;
            var success = long.TryParse(data, out numb);

            return success ? numb : 0;
        }
    }
}
