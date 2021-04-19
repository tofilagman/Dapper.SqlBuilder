namespace Dapper.SqlBuilder.ValueObjects
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
        YEAR = 1,
        MONTH = 2,
        DAY = 3,
        DAYOFYEAR = 4,
        HOUR = 5,
        MINUTE = 6,
        SECOND = 7,
        MILLISECOND = 8,
        MICROSECOND = 9,
        WEEK = 10,
        WEEKDAY = 11
    }
}
