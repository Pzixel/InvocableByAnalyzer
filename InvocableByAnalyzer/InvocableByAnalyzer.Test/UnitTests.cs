using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace InvocableByAnalyzer.Test
{
    [TestClass]
    [SuppressMessage("ReSharper", "UseStringInterpolation")]
    public class UnitTest : CodeFixVerifier
    {
        //No diagnostics expected to show up
        [TestMethod]
        public void TestMethod1()
        {
            VerifyCSharpDiagnostic(@"");
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void TestMethod2()
        {
            const string test = @"
    using System;

    public class InvocableByAttribute : Attribute
    {
        public Type[] AllowedTypes { get; }

        public InvocableByAttribute(params Type[] allowedTypes)
        {
            AllowedTypes = allowedTypes;
        }
    }

    class A
    {
        [InvocableBy(typeof(B))]
        public A()
        {

        }
    }

    class B
    {
        public B()
        {
            var foo = new A();
        }
    }

    class C
    {
        public C()
        {
            var foo = new A();
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = InvocableByDiagnosticAnalyzer.DiagnosticId,
                Message = string.Format("Class '{0}' is not allowed to call this member", "C"),
                Severity = DiagnosticSeverity.Error,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 35, 23)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new InvocableByDiagnosticAnalyzer();
        }
    }
}