using System;
using System.Collections.Generic;
using System.Text;

namespace DbfDotNet.Core
{
    [Record(FieldMapping = FieldMapping.ExplicitColumnsOnly)]
    internal class DbfColumnHeader
    {
        [Column(Width = 11, Type = DbfDotNet.ColumnType.CHARACTER)]
        public string ColumnName;

        [Column(Type = DbfDotNet.ColumnType.CHAR)]
        public char ColumnType;

        [Column(Type = DbfDotNet.ColumnType.DELAYED, Width = 4)]
        public byte[] FieldDataAddress;

        [Column(Type = DbfDotNet.ColumnType.BYTE)]
        public byte ColumnWidth;

        [Column(Type = DbfDotNet.ColumnType.BYTE)]
        public byte Decimals;

        [Column(Type = DbfDotNet.ColumnType.DELAYED, Width = 14)]
        public byte[] Reserved2;

        public DbfColumnHeader()
        {
        }

        public void a()
        {
            FieldDataAddress = null;
            Reserved2 = null;
        }
    }
}
