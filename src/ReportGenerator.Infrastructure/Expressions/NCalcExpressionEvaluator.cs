using System.Collections.Generic;
using ReportGenerator.Domain.Interfaces;

namespace ReportGenerator.Infrastructure.Expressions;

public class NCalcExpressionEvaluator : IExpressionEvaluator
{
    public object? Evaluate(string expression, IDictionary<string, object?> parameters)
    {
        // NCalc.Core 4.x no longer exposes NCalc.Expression; evaluate via AST + custom visitor
        var context = new NCalc.ExpressionContext();
        if (parameters != null)
        {
            context.Parameters ??= new Dictionary<string, object>();
            foreach (var kv in parameters)
            {
                context.Parameters[kv.Key] = kv.Value!;
            }
        }

        var ast = NCalc.Factories.LogicalExpressionFactory.Create(expression, context);
        var visitor = new SimpleEvaluator(context);
        ast.Accept(visitor);
        return visitor.Result;
    }
}

internal sealed class SimpleEvaluator : NCalc.Visitors.ILogicalExpressionVisitor
{
    private readonly NCalc.ExpressionContext _context;
    public object? Result { get; private set; }

    public SimpleEvaluator(NCalc.ExpressionContext context)
    {
        _context = context;
    }

    public void Visit(NCalc.Domain.ValueExpression value)
    {
        Result = value.Value;
    }

    public void Visit(NCalc.Domain.Identifier identifier)
    {
        if (_context.Parameters != null && _context.Parameters.TryGetValue(identifier.Name, out var val))
        {
            Result = val;
            return;
        }
        throw new NCalc.Exceptions.NCalcParameterNotDefinedException(identifier.Name);
    }

    public void Visit(NCalc.Domain.UnaryExpression expression)
    {
        expression.Expression.Accept(this);
        var v = Result;
        switch (expression.Type)
        {
            case NCalc.Domain.UnaryExpressionType.Negate:
                Result = Negate(v);
                break;
            case NCalc.Domain.UnaryExpressionType.Positive:
                Result = ToNumber(v);
                break;
            case NCalc.Domain.UnaryExpressionType.Not:
                Result = !ToBoolean(v);
                break;
            case NCalc.Domain.UnaryExpressionType.BitwiseNot:
                Result = ~ToInt64(v);
                break;
            default:
                throw new NotSupportedException($"Unary operator '{expression.Type}' is not supported.");
        }
    }

    public void Visit(NCalc.Domain.BinaryExpression expression)
    {
        expression.LeftExpression.Accept(this);
        var left = Result;
        expression.RightExpression.Accept(this);
        var right = Result;

        switch (expression.Type)
        {
            case NCalc.Domain.BinaryExpressionType.Plus:
                Result = Add(left, right);
                break;
            case NCalc.Domain.BinaryExpressionType.Minus:
                Result = Subtract(left, right);
                break;
            case NCalc.Domain.BinaryExpressionType.Times:
                Result = Multiply(left, right);
                break;
            case NCalc.Domain.BinaryExpressionType.Div:
                Result = Divide(left, right);
                break;
            case NCalc.Domain.BinaryExpressionType.Modulo:
                Result = Modulo(left, right);
                break;
            case NCalc.Domain.BinaryExpressionType.Exponentiation:
                Result = Pow(left, right);
                break;
            case NCalc.Domain.BinaryExpressionType.Equal:
                Result = Compare(left, right) == 0;
                break;
            case NCalc.Domain.BinaryExpressionType.NotEqual:
                Result = Compare(left, right) != 0;
                break;
            case NCalc.Domain.BinaryExpressionType.Greater:
                Result = Compare(left, right) > 0;
                break;
            case NCalc.Domain.BinaryExpressionType.GreaterOrEqual:
                Result = Compare(left, right) >= 0;
                break;
            case NCalc.Domain.BinaryExpressionType.Lesser:
                Result = Compare(left, right) < 0;
                break;
            case NCalc.Domain.BinaryExpressionType.LesserOrEqual:
                Result = Compare(left, right) <= 0;
                break;
            case NCalc.Domain.BinaryExpressionType.And:
                Result = ToBoolean(left) && ToBoolean(right);
                break;
            case NCalc.Domain.BinaryExpressionType.Or:
                Result = ToBoolean(left) || ToBoolean(right);
                break;
            case NCalc.Domain.BinaryExpressionType.BitwiseAnd:
                Result = ToInt64(left) & ToInt64(right);
                break;
            case NCalc.Domain.BinaryExpressionType.BitwiseOr:
                Result = ToInt64(left) | ToInt64(right);
                break;
            case NCalc.Domain.BinaryExpressionType.BitwiseXOr:
                Result = ToInt64(left) ^ ToInt64(right);
                break;
            case NCalc.Domain.BinaryExpressionType.LeftShift:
                Result = ToInt64(left) << ToInt32(right);
                break;
            case NCalc.Domain.BinaryExpressionType.RightShift:
                Result = ToInt64(left) >> ToInt32(right);
                break;
            default:
                throw new NotSupportedException($"Binary operator '{expression.Type}' is not supported.");
        }
    }

    public void Visit(NCalc.Domain.TernaryExpression expression)
    {
        expression.LeftExpression.Accept(this);
        var cond = ToBoolean(Result);
        if (cond)
        {
            expression.MiddleExpression.Accept(this);
        }
        else
        {
            expression.RightExpression.Accept(this);
        }
    }

    public void Visit(NCalc.Domain.Function function)
    {
        // No built-in functions supported in this minimal evaluator
        throw new NCalc.Exceptions.NCalcFunctionNotFoundException(function.Identifier.Name, function.Identifier.Name);
    }

    private static object Negate(object? v)
    {
        var n = ToNumber(v);
        return n switch
        {
            decimal d => -d,
            double db => -db,
            long l => -l,
            _ => throw new NotSupportedException($"Unsupported numeric type: {n?.GetType().Name ?? "null"}")
        };
    }

    private static object Add(object? a, object? b)
    {
        if (a is string || b is string)
            return Convert.ToString(a) + Convert.ToString(b);
        return PromoteAndApply(a, b, (x, y) => x + y);
    }

    private static object Subtract(object? a, object? b) => PromoteAndApply(a, b, (x, y) => x - y);
    private static object Multiply(object? a, object? b) => PromoteAndApply(a, b, (x, y) => x * y);
    private static object Divide(object? a, object? b) => PromoteAndApply(a, b, (x, y) => x / y);
    private static object Modulo(object? a, object? b) => PromoteAndApply(a, b, (x, y) => x % y);
    private static object Pow(object? a, object? b)
    {
        var (x, y, kind) = Promote(a, b);
        return kind switch
        {
            NumberKind.Decimal => (object)(decimal)Math.Pow((double)x, (double)y),
            NumberKind.Double => Math.Pow((double)x, (double)y),
            NumberKind.Integer => (long)Math.Pow((double)x, (double)y),
            _ => throw new NotSupportedException()
        };
    }

    private static int Compare(object? a, object? b)
    {
        if (IsNumber(a) && IsNumber(b))
        {
            var (x, y, kind) = Promote(a, b);
            return kind switch
            {
                NumberKind.Decimal => ((decimal)x).CompareTo((decimal)y),
                NumberKind.Double => ((double)x).CompareTo((double)y),
                NumberKind.Integer => ((long)x).CompareTo((long)y),
                _ => 0
            };
        }
        if (a is bool ba && b is bool bb) return ba.CompareTo(bb);
        var sa = Convert.ToString(a) ?? string.Empty;
        var sb = Convert.ToString(b) ?? string.Empty;
        return string.CompareOrdinal(sa, sb);
    }

    private static (object x, object y, NumberKind kind) Promote(object? a, object? b)
    {
        // Promote to the widest numeric type present among decimal > double > long
        var ak = ClassifyNumber(a);
        var bk = ClassifyNumber(b);
        var kind = (NumberKind)Math.Max((int)ak, (int)bk);
        return kind switch
        {
            NumberKind.Decimal => (ToDecimal(a), ToDecimal(b), kind),
            NumberKind.Double => (ToDouble(a), ToDouble(b), kind),
            NumberKind.Integer => (ToInt64(a), ToInt64(b), kind),
            _ => throw new NotSupportedException("Non-numeric operands")
        };
    }

    private static object PromoteAndApply(object? a, object? b, Func<decimal, decimal, decimal> opDec)
    {
        var (x, y, kind) = Promote(a, b);
        return kind switch
        {
            NumberKind.Decimal => opDec((decimal)x, (decimal)y),
            NumberKind.Double => (double)opDec((decimal)(double)x, (decimal)(double)y),
            NumberKind.Integer => (long)opDec((decimal)(long)x, (decimal)(long)y),
            _ => throw new NotSupportedException()
        };
    }

    private static NumberKind ClassifyNumber(object? v)
    {
        return v switch
        {
            decimal => NumberKind.Decimal,
            double or float => NumberKind.Double,
            sbyte or byte or short or ushort or int or uint or long or ulong => NumberKind.Integer,
            _ => NumberKind.None
        };
    }

    private static object ToNumber(object? v)
    {
        return ClassifyNumber(v) switch
        {
            NumberKind.Decimal => (object)ToDecimal(v),
            NumberKind.Double => ToDouble(v),
            NumberKind.Integer => ToInt64(v),
            _ => 0m
        };
    }

    private static bool IsNumber(object? v) => ClassifyNumber(v) != NumberKind.None;
    private static decimal ToDecimal(object? v) => v switch
    {
        null => 0m,
        decimal d => d,
        double db => (decimal)db,
        float f => (decimal)f,
        sbyte sb => sb,
        byte b => b,
        short s => s,
        ushort us => us,
        int i => i,
        uint ui => ui,
        long l => l,
        ulong ul => (decimal)ul,
        string s when decimal.TryParse(s, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dv) => dv,
        _ => Convert.ToDecimal(v, System.Globalization.CultureInfo.InvariantCulture)
    };

    private static double ToDouble(object? v) => Convert.ToDouble(v, System.Globalization.CultureInfo.InvariantCulture);
    private static long ToInt64(object? v) => Convert.ToInt64(v, System.Globalization.CultureInfo.InvariantCulture);
    private static int ToInt32(object? v) => Convert.ToInt32(v, System.Globalization.CultureInfo.InvariantCulture);
    private static bool ToBoolean(object? v)
    {
        return v switch
        {
            null => false,
            bool b => b,
            string s when bool.TryParse(s, out var res) => res,
            _ => Convert.ToDouble(v, System.Globalization.CultureInfo.InvariantCulture) != 0.0
        };
    }

    private enum NumberKind { None = 0, Integer = 1, Double = 2, Decimal = 3 }
}
