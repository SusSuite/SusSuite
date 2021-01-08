using System;

namespace SusSuite.Core.Models
{
    public class SusSuiteGame
    {
        public string GameCode { get; init; }
        public SusSuitePlugin SusSuitePlugin { get; set; }
        private object _data;
        public bool TryGetData<T>(out T data)
        {
            data = default(T);

            switch (_data)
            {
                case null:
                    return false;
                case T result:
                    data = result;
                    return true;
                default:
                    try
                    {
                        data = (T)Convert.ChangeType(_data, typeof(T));
                        return true;
                    }
                    catch (InvalidCastException)
                    {
                        return false;
                    }
            }
        }
        public void SetData<T>(T data)
        {
            _data = data;
        }
    }
}
