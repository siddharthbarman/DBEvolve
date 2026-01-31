using System;
using System.Collections.Generic;
using System.Text;

namespace SByteStream.DBEvolve
{
    public class DBEvolveException : ApplicationException
    {
        public DBEvolveException(string message) : base(message)
        {
        }
    }

    public class ScriptModifiedException : DBEvolveException
    {
        public ScriptModifiedException(string message) : base(message)
        {
        }
    }

    public class ScriptNotFoundException : DBEvolveException
    {
        public ScriptNotFoundException(string message) : base(message)
        {
        }
    }
}
