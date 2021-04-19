namespace Dapper.SqlBuilder.Builder
{
    /// <summary>
    /// Implements the expression building for the UPDATE statement
    /// </summary>
    internal partial class SqlQueryBuilder
    { 
        public void UpdateAssignField(string sourceAlias, string fieldName, object value)
        {
            var paramId = NextParamId();
            AddParameter(paramId, value);
            var updateValue = $"{sourceAlias}.{Adapter.Field(fieldName)} = {Adapter.Parameter(paramId)}";
            _updateValues.Add(updateValue);
        }
         
        public void UpdateFieldReplaceString(string sourceAlias, string fieldName, object findWhat, object replaceWith)
        {
            var findWhatParam = NextParamId();
            AddParameter(findWhatParam, findWhat);

            var replaceWithParam = NextParamId();
            AddParameter(replaceWithParam, replaceWith);
            var updateValue =
                $"{sourceAlias}.{Adapter.Field(fieldName)} = REPLACE({sourceAlias}.{Adapter.Field(fieldName)}, {Adapter.Parameter(findWhatParam)}, {Adapter.Parameter(replaceWithParam)})";
            _updateValues.Add(updateValue);
        }
         
        public void UpdateFieldWithOperation(string sourceAlias, string fieldName, object operandValue, string operation)
        {
            var paramId = NextParamId();
            AddParameter(paramId, operandValue);
            var updateValue = $"{sourceAlias}.{Adapter.Field(fieldName)} = {SourceAlias}.{Adapter.Field(fieldName)} {operation} {Adapter.Parameter(paramId)}";
            _updateValues.Add(updateValue);
        }

        public void SelectFieldFormatString(string fieldName, string format)
        {
            var formatValue = $"FORMAT({fieldName}, '{format}')";
        }
    }
}