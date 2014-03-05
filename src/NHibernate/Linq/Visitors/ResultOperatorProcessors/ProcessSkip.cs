using System.Linq.Expressions;
using NHibernate.Engine.Query;
using NHibernate.Param;
using Remotion.Linq.Clauses.ResultOperators;

namespace NHibernate.Linq.Visitors.ResultOperatorProcessors
{
	public class ProcessSkip : IResultOperatorProcessor<SkipResultOperator>
	{
		#region IResultOperatorProcessor<SkipResultOperator> Members

		public void Process(SkipResultOperator resultOperator, QueryModelVisitor queryModelVisitor, IntermediateHqlTree tree)
		{
			VisitorParameters parameters = queryModelVisitor.VisitorParameters;
			NamedParameter namedParameter;

            /** https://nhibernate.jira.com/browse/NH-3021
            * Temporary fix for Skip for dialects that support constant limiting only
            * */
            if (parameters.SessionFactory.Dialect.SupportsLimit && !parameters.SessionFactory.Dialect.SupportsVariableLimit)
            {
                tree.AddSkipClause(tree.TreeBuilder.Constant(resultOperator.GetConstantCount()));
                return;
            }

			if (parameters.ConstantToParameterMap.TryGetValue(resultOperator.Count as ConstantExpression, out namedParameter))
			{
				parameters.RequiredHqlParameters.Add(new NamedParameterDescriptor(namedParameter.Name, null, false));
            //    tree.AddSkipClause(tree.TreeBuilder.Parameter(namedParameter.Name));
            //}
            //else
            //{
            //    tree.AddSkipClause(tree.TreeBuilder.Constant(resultOperator.GetConstantCount()));

                 tree.AddSkipClause(tree.TreeBuilder.Parameter(namedParameter.Name));
 			}
            else
            {
                tree.AddSkipClause(tree.TreeBuilder.Constant(resultOperator.GetConstantCount()));
            //}

                /* end of fix */

			}
		}

		#endregion
	}
}
