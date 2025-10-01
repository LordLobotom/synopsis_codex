using System.Collections.Generic;
using FluentAssertions;
using ReportGenerator.Infrastructure.Expressions;
using Xunit;

namespace ReportGenerator.Domain.Tests;

public class ExpressionEvaluatorTests
{
    [Theory]
    [InlineData("1+2*3", 7)]
    [InlineData("(10-3)*2", 14)]
    public void Evaluate_ShouldComputeArithmetic(string expr, object expected)
    {
        var eval = new NCalcExpressionEvaluator();
        var result = eval.Evaluate(expr, new Dictionary<string, object?>());
        result.Should().Be(expected);
    }

    [Fact]
    public void Evaluate_ShouldUseParameters()
    {
        var eval = new NCalcExpressionEvaluator();
        var result = eval.Evaluate("a + b", new Dictionary<string, object?>
        {
            ["a"] = 5,
            ["b"] = 7
        });
        result.Should().Be(12);
    }
}

