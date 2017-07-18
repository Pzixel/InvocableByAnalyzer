﻿using System.Diagnostics.CodeAnalysis;
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
    using InvocableByAnalyzer;

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
                Id = "InvocableByAnalyzer",
                Message = string.Format("Class '{0}' is not allowed to call this member", "C"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 11, 15)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new InvocableByAnalyzer();
        }
    }
}