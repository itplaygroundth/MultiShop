using System;
using System.Collections.Generic;
using System.Text;

namespace DbfDotNet.Core
{
    internal enum OpenFileMode
    {
        OpenReadOnly,
        OpenReadWrite,
        OpenOrCreate
    }

    public enum BufferType
    {
        ReadBuffer,
        WriteBuffer
    }

}
