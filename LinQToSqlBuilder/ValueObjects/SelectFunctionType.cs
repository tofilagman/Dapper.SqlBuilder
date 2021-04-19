﻿namespace Dapper.SqlBuilder.ValueObjects
{
    /// <summary>
    /// An enumeration of the supported aggregate SQL functions. The item names should match the related function names
    /// </summary>
    public enum SelectFunctionType
    {
        COUNT,
        DISTINCT,
        SUM,
        MIN,
        MAX,
        AVG,
        CUSTOM
    }

    public enum DatePart
    {
        YEAR,
        MONTH,
        DAY,
        DAYOFYEAR,
        HOUR,
        MINUTE,
        SECOND,
        MILLISECOND,
        MICROSECOND,
        WEEK,
        WEEKDAY
    }
}