//using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using CoreWebAPI.Common.Helper;

namespace CoreWebAPI.Common.DB
{
    public static class PostgresqlDbTypeClass
    {
        public static List<KeyValuePair<string, SqlSugar.CSharpDataType>> MappingTypesConst = new List<KeyValuePair<string, SqlSugar.CSharpDataType>>(){

            new KeyValuePair<string, SqlSugar.CSharpDataType>("int2",SqlSugar.CSharpDataType.@short),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("smallint",SqlSugar.CSharpDataType.@short),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("int4",SqlSugar.CSharpDataType.@int),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("integer",SqlSugar.CSharpDataType.@int),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("int8",SqlSugar.CSharpDataType.@long),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("bigint",SqlSugar.CSharpDataType.@long),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("float4",SqlSugar.CSharpDataType.@float),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("real",SqlSugar.CSharpDataType.@float),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("float8",SqlSugar.CSharpDataType.@double),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("double precision",SqlSugar.CSharpDataType.@int),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("numeric",SqlSugar.CSharpDataType.@decimal),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("decimal",SqlSugar.CSharpDataType.@decimal),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("path",SqlSugar.CSharpDataType.@decimal),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("point",SqlSugar.CSharpDataType.@decimal),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("polygon",SqlSugar.CSharpDataType.@decimal),

            new KeyValuePair<string, SqlSugar.CSharpDataType>("boolean",SqlSugar.CSharpDataType.@bool),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("bool",SqlSugar.CSharpDataType.@bool),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("box",SqlSugar.CSharpDataType.@bool),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("bytea",SqlSugar.CSharpDataType.@bool),

            new KeyValuePair<string, SqlSugar.CSharpDataType>("varchar",SqlSugar.CSharpDataType.@string),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("character varying",SqlSugar.CSharpDataType.@string),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("geometry",SqlSugar.CSharpDataType.@string),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("name",SqlSugar.CSharpDataType.@string),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("text",SqlSugar.CSharpDataType.@string),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("char",SqlSugar.CSharpDataType.@string),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("character",SqlSugar.CSharpDataType.@string),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("cidr",SqlSugar.CSharpDataType.@string),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("circle",SqlSugar.CSharpDataType.@string),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("tsquery",SqlSugar.CSharpDataType.@string),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("tsvector",SqlSugar.CSharpDataType.@string),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("txid_snapshot",SqlSugar.CSharpDataType.@string),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("uuid",SqlSugar.CSharpDataType.Guid),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("xml",SqlSugar.CSharpDataType.@string),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("json",SqlSugar.CSharpDataType.@string),

            new KeyValuePair<string, SqlSugar.CSharpDataType>("interval",SqlSugar.CSharpDataType.@decimal),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("lseg",SqlSugar.CSharpDataType.@decimal),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("macaddr",SqlSugar.CSharpDataType.@decimal),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("money",SqlSugar.CSharpDataType.@decimal),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("timestamp",SqlSugar.CSharpDataType.DateTime),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("timestamp with time zone",SqlSugar.CSharpDataType.DateTime),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("timestamptz",SqlSugar.CSharpDataType.DateTime),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("timestamp without time zone",SqlSugar.CSharpDataType.DateTime),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("date",SqlSugar.CSharpDataType.DateTime),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("time",SqlSugar.CSharpDataType.DateTime),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("time with time zone",SqlSugar.CSharpDataType.DateTime),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("timetz",SqlSugar.CSharpDataType.DateTime),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("time without time zone",SqlSugar.CSharpDataType.DateTime),

            new KeyValuePair<string, SqlSugar.CSharpDataType>("bit",SqlSugar.CSharpDataType.byteArray),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("bit varying",SqlSugar.CSharpDataType.byteArray),
            new KeyValuePair<string, SqlSugar.CSharpDataType>("varbit",SqlSugar.CSharpDataType.@byte),

        };


        public static object GetDbValue(string value, string type)
        {
            return (type.GetCString().ToLower()) switch
            {
                "ansistring" => DbType.AnsiString,
                "binary" => DbType.Binary,
                "byte" => DbType.Byte,
                "boolean" => value.GetCBool(),
                "currency" => DbType.Currency,
                "date" => value.GetCDate(),
                "datetime" => value.GetCDate(),
                "decimal" => value.GetCDecimal(),
                "double" => value.GetCDecimal(),
                "guid" => DbType.Guid,
                "int16" => value.GetCInt(),
                "int32" => value.GetCInt(),
                "int64" => value.GetCInt(),
                "object" => DbType.Object,
                "sbyte" => DbType.SByte,
                "single" => DbType.Single,
                "string" => value.GetCString(),
                "time" => DbType.Time,
                "uint16" => DbType.UInt16,
                "uint32" => DbType.UInt32,
                "uint64" => DbType.UInt64,
                "varnumeric" => DbType.VarNumeric,
                "ansistringfixedlength" => DbType.AnsiStringFixedLength,
                "stringfixedlength" => DbType.StringFixedLength,
                "xml" => DbType.Xml,
                "datetime2" => DbType.DateTime2,
                "datetimeoffset" => DbType.DateTimeOffset,
                _ => value.GetCString(),
            };
            ;
        }


        public static object GetDbValue(string value, DbType type)
        {
            return type switch
            {
                DbType.AnsiString => DbType.AnsiString,
                DbType.Binary => DbType.Binary,
                DbType.Byte => DbType.Byte,
                DbType.Boolean => value.GetCBool(),
                DbType.Currency => DbType.Currency,
                DbType.Date => value.GetCDate(),
                DbType.DateTime => value.GetCDate(),
                DbType.Decimal => value.GetCDecimal(),
                DbType.Double => value.GetCDecimal(),
                DbType.Guid => DbType.Guid,
                DbType.Int16 => value.GetCInt(),
                DbType.Int32 => value.GetCInt(),
                DbType.Int64 => value.GetCInt(),
                DbType.Object => DbType.Object,
                DbType.SByte => DbType.SByte,
                DbType.Single => DbType.Single,
                DbType.String => value.GetCString(),
                DbType.Time => DbType.Time,
                DbType.UInt16 => DbType.UInt16,
                DbType.UInt32 => DbType.UInt32,
                DbType.UInt64 => DbType.UInt64,
                DbType.VarNumeric => DbType.VarNumeric,
                DbType.AnsiStringFixedLength => DbType.AnsiStringFixedLength,
                DbType.StringFixedLength => DbType.StringFixedLength,
                DbType.Xml => DbType.Xml,
                DbType.DateTime2 => DbType.DateTime2,
                DbType.DateTimeOffset => DbType.DateTimeOffset,
                _ => value.GetCString(),
            };
            ;
        }

        // 判断数据是否匹配类型
        public static bool GetDbValueIsType(object value, DbType type)
        {
            return (type.GetCString().ToLower()) switch
            {
                "ansistring" => true,
                "binary" => true,
                "byte" => true,
                "boolean" => bool.TryParse(value.ToString(), out _),
                "currency" => true,
                "date" => DateTime.TryParse(value.ToString(), out _),
                "datetime" => DateTime.TryParse(value.ToString(), out _),
                "decimal" => decimal.TryParse(value.ToString(), out _),
                "double" => double.TryParse(value.ToString(), out _),
                "guid" => true,
                "int16" => int.TryParse(value.ToString(),out _),
                "int32" => int.TryParse(value.ToString(), out _),
                "int64" => int.TryParse(value.ToString(), out _),
                "object" => true,
                "sbyte" => true,
                "single" => true,
                "string" => true,
                "time" => true,
                "uint16" => true,
                "uint32" => true,
                "uint64" => true,
                "varnumeric" => true,
                "ansistringfixedlength" => true,
                "stringfixedlength" => true,
                "xml" => true,
                "datetime2" => true,
                "datetimeoffset" => true,
                _ => true,
            };
        }
    }
}
