using System;
using System.Linq.Expressions;

namespace RulesService.Model
{
    public class Rule<P> : RuleBase
    {
        private Func<P, bool> _match;

        public Func<P, bool> Match
        {
            get
            {
                // Check if match delegate is not set.
                if (_match == null)
                {
                    try
                    {
                        // Set match expression to true if null or empty.
                        var matchExpression = !string.IsNullOrEmpty(MatchExpression) ? MatchExpression : "true";

                        // Define expression parameter for the object to match on.
                        var parameter1 = Expression.Parameter(typeof(P), "p");

                        // Parse match expression with parameter into lambda expression tree.
                        var expressionTree = System.Linq.Dynamic.DynamicExpression.ParseLambda(new ParameterExpression[] { parameter1 }, null, matchExpression, null);

                        // Compile expression tree and assign to match delegate.
                        _match = expressionTree.Compile() as Func<P, bool>;
                    }
                    catch (Exception ex)
                    {
                        // Log error and continue with false.
                        throw new Exception($"Rule invalid with ID {this.RuleId}; {this.MatchExpression}. {ex.Message}.");
                    }
                }

                // Return match delegate.
                return _match;
            }
        }
    }
}
