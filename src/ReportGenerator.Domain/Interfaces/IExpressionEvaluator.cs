using System.Collections.Generic;

namespace ReportGenerator.Domain.Interfaces;

public interface IExpressionEvaluator
{
    object? Evaluate(string expression, IDictionary<string, object?> parameters);
}

