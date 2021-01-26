namespace Dapper.SqlBuilder.Resolver.ExpressionTree
{
    class MemberNode : Node
    {
        public string TableName { get; set; }
        public string FieldName { get; set; }
    }
}
