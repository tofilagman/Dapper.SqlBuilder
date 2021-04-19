using Dapper.SqlBuilder.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.SqlBuilder.Resolver.ExpressionTree
{
    class NullNode : Node
    { 
        public NullMethod Method { get; set; }
        public MemberNode MemberNode { get; set; } 
    }
     
}
