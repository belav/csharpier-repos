//------------------------------------------------------------------------------
// <copyright file="FilterException.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">Microsoft</owner>
// <owner current="true" primary="false">Microsoft</owner>
// <owner current="false" primary="false">Microsoft</owner>
//------------------------------------------------------------------------------

namespace System.Data
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.Serialization;

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [Serializable]
    public class InvalidExpressionException : DataException
    {
        protected InvalidExpressionException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public InvalidExpressionException()
            : base() { }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public InvalidExpressionException(string s)
            : base(s) { }

        public InvalidExpressionException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [Serializable]
    public class EvaluateException : InvalidExpressionException
    {
        protected EvaluateException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public EvaluateException()
            : base() { }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public EvaluateException(string s)
            : base(s) { }

        public EvaluateException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [Serializable]
    public class SyntaxErrorException : InvalidExpressionException
    {
        protected SyntaxErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public SyntaxErrorException()
            : base() { }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public SyntaxErrorException(string s)
            : base(s) { }

        public SyntaxErrorException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    internal sealed class ExprException
    {
        private ExprException()
        { /* prevent utility class from being insantiated*/
        }

        private static OverflowException _Overflow(string error)
        {
            OverflowException e = new OverflowException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static InvalidExpressionException _Expr(string error)
        {
            InvalidExpressionException e = new InvalidExpressionException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static SyntaxErrorException _Syntax(string error)
        {
            SyntaxErrorException e = new SyntaxErrorException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static EvaluateException _Eval(string error)
        {
            EvaluateException e = new EvaluateException(error);
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        private static EvaluateException _Eval(string error, Exception innerException)
        {
            EvaluateException e = new EvaluateException(
                error /*, innerException*/
            ); //
            ExceptionBuilder.TraceExceptionAsReturnValue(e);
            return e;
        }

        public static Exception InvokeArgument()
        {
            return ExceptionBuilder._Argument(Res.GetString(Res.Expr_InvokeArgument));
        }

        public static Exception NYI(string moreinfo)
        {
            string err = Res.GetString(Res.Expr_NYI, moreinfo);
            Debug.Fail(err);
            return _Expr(err);
        }

        public static Exception MissingOperand(OperatorInfo before)
        {
            return _Syntax(Res.GetString(Res.Expr_MissingOperand, Operators.ToString(before.op)));
        }

        public static Exception MissingOperator(string token)
        {
            return _Syntax(Res.GetString(Res.Expr_MissingOperand, token));
        }

        public static Exception TypeMismatch(string expr)
        {
            return _Eval(Res.GetString(Res.Expr_TypeMismatch, expr));
        }

        public static Exception FunctionArgumentOutOfRange(string arg, string func)
        {
            return ExceptionBuilder._ArgumentOutOfRange(
                arg,
                Res.GetString(Res.Expr_ArgumentOutofRange, func)
            );
        }

        public static Exception ExpressionTooComplex()
        {
            return _Eval(Res.GetString(Res.Expr_ExpressionTooComplex));
        }

        public static Exception UnboundName(string name)
        {
            return _Eval(Res.GetString(Res.Expr_UnboundName, name));
        }

        public static Exception InvalidString(string str)
        {
            return _Syntax(Res.GetString(Res.Expr_InvalidString, str));
        }

        public static Exception UndefinedFunction(string name)
        {
            return _Eval(Res.GetString(Res.Expr_UndefinedFunction, name));
        }

        public static Exception SyntaxError()
        {
            return _Syntax(Res.GetString(Res.Expr_Syntax));
        }

        public static Exception FunctionArgumentCount(string name)
        {
            return _Eval(Res.GetString(Res.Expr_FunctionArgumentCount, name));
        }

        public static Exception MissingRightParen()
        {
            return _Syntax(Res.GetString(Res.Expr_MissingRightParen));
        }

        public static Exception UnknownToken(string token, int position)
        {
            return _Syntax(
                Res.GetString(
                    Res.Expr_UnknownToken,
                    token,
                    position.ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        public static Exception UnknownToken(Tokens tokExpected, Tokens tokCurr, int position)
        {
            return _Syntax(
                Res.GetString(
                    Res.Expr_UnknownToken1,
                    tokExpected.ToString(),
                    tokCurr.ToString(),
                    position.ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        public static Exception DatatypeConvertion(Type type1, Type type2)
        {
            return _Eval(
                Res.GetString(Res.Expr_DatatypeConvertion, type1.ToString(), type2.ToString())
            );
        }

        public static Exception DatavalueConvertion(
            object value,
            Type type,
            Exception innerException
        )
        {
            return _Eval(
                Res.GetString(Res.Expr_DatavalueConvertion, value.ToString(), type.ToString()),
                innerException
            );
        }

        public static Exception InvalidName(string name)
        {
            return _Syntax(Res.GetString(Res.Expr_InvalidName, name));
        }

        public static Exception InvalidDate(string date)
        {
            return _Syntax(Res.GetString(Res.Expr_InvalidDate, date));
        }

        public static Exception NonConstantArgument()
        {
            return _Eval(Res.GetString(Res.Expr_NonConstantArgument));
        }

        public static Exception InvalidPattern(string pat)
        {
            return _Eval(Res.GetString(Res.Expr_InvalidPattern, pat));
        }

        public static Exception InWithoutParentheses()
        {
            return _Syntax(Res.GetString(Res.Expr_InWithoutParentheses));
        }

        public static Exception InWithoutList()
        {
            return _Syntax(Res.GetString(Res.Expr_InWithoutList));
        }

        public static Exception InvalidIsSyntax()
        {
            return _Syntax(Res.GetString(Res.Expr_IsSyntax));
        }

        public static Exception Overflow(Type type)
        {
            return _Overflow(Res.GetString(Res.Expr_Overflow, type.Name));
        }

        public static Exception ArgumentType(string function, int arg, Type type)
        {
            return _Eval(
                Res.GetString(
                    Res.Expr_ArgumentType,
                    function,
                    arg.ToString(CultureInfo.InvariantCulture),
                    type.ToString()
                )
            );
        }

        public static Exception ArgumentTypeInteger(string function, int arg)
        {
            return _Eval(
                Res.GetString(
                    Res.Expr_ArgumentTypeInteger,
                    function,
                    arg.ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        public static Exception TypeMismatchInBinop(int op, Type type1, Type type2)
        {
            return _Eval(
                Res.GetString(
                    Res.Expr_TypeMismatchInBinop,
                    Operators.ToString(op),
                    type1.ToString(),
                    type2.ToString()
                )
            );
        }

        public static Exception AmbiguousBinop(int op, Type type1, Type type2)
        {
            return _Eval(
                Res.GetString(
                    Res.Expr_AmbiguousBinop,
                    Operators.ToString(op),
                    type1.ToString(),
                    type2.ToString()
                )
            );
        }

        public static Exception UnsupportedOperator(int op)
        {
            return _Eval(Res.GetString(Res.Expr_UnsupportedOperator, Operators.ToString(op)));
        }

        public static Exception InvalidNameBracketing(string name)
        {
            return _Syntax(Res.GetString(Res.Expr_InvalidNameBracketing, name));
        }

        public static Exception MissingOperandBefore(string op)
        {
            return _Syntax(Res.GetString(Res.Expr_MissingOperandBefore, op));
        }

        public static Exception TooManyRightParentheses()
        {
            return _Syntax(Res.GetString(Res.Expr_TooManyRightParentheses));
        }

        public static Exception UnresolvedRelation(string name, string expr)
        {
            return _Eval(Res.GetString(Res.Expr_UnresolvedRelation, name, expr));
        }

        internal static EvaluateException BindFailure(string relationName)
        {
            return _Eval(Res.GetString(Res.Expr_BindFailure, relationName));
        }

        public static Exception AggregateArgument()
        {
            return _Syntax(Res.GetString(Res.Expr_AggregateArgument));
        }

        public static Exception AggregateUnbound(string expr)
        {
            return _Eval(Res.GetString(Res.Expr_AggregateUnbound, expr));
        }

        public static Exception EvalNoContext()
        {
            return _Eval(Res.GetString(Res.Expr_EvalNoContext));
        }

        public static Exception ExpressionUnbound(string expr)
        {
            return _Eval(Res.GetString(Res.Expr_ExpressionUnbound, expr));
        }

        public static Exception ComputeNotAggregate(string expr)
        {
            return _Eval(Res.GetString(Res.Expr_ComputeNotAggregate, expr));
        }

        public static Exception FilterConvertion(string expr)
        {
            return _Eval(Res.GetString(Res.Expr_FilterConvertion, expr));
        }

        public static Exception LookupArgument()
        {
            return _Syntax(Res.GetString(Res.Expr_LookupArgument));
        }

        public static Exception InvalidType(string typeName)
        {
            return _Eval(Res.GetString(Res.Expr_InvalidType, typeName));
        }

        public static Exception InvalidHoursArgument()
        {
            return _Eval(Res.GetString(Res.Expr_InvalidHoursArgument));
        }

        public static Exception InvalidMinutesArgument()
        {
            return _Eval(Res.GetString(Res.Expr_InvalidMinutesArgument));
        }

        public static Exception InvalidTimeZoneRange()
        {
            return _Eval(Res.GetString(Res.Expr_InvalidTimeZoneRange));
        }

        public static Exception MismatchKindandTimeSpan()
        {
            return _Eval(Res.GetString(Res.Expr_MismatchKindandTimeSpan));
        }

        public static Exception UnsupportedDataType(Type type)
        {
            return ExceptionBuilder._Argument(
                Res.GetString(Res.Expr_UnsupportedType, type.FullName)
            );
        }
    }
}
