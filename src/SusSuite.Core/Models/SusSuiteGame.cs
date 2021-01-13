using System;
using System.Collections.Generic;
using Impostor.Api.Games;

namespace SusSuite.Core.Models
{
    public class SusSuiteGame
    {
        public string GameCode { get; init; }
        public SusSuitePlugin SusSuitePlugin { get; set; }

        private object _data = new();
        internal bool TryGetData<T>(out T data)
        {
            data = default(T);

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
        internal void SetData(object data)
        {
            _data = data;
        }
    }
}
