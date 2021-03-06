﻿using CSharpEssentials.UseNameOf;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using RoslynNUnitLight;

namespace CSharpEssentials.Tests.UseNameOf
{
    [TestFixture]
    public class UseNameOfAnalyzerTests : AnalyzerTestFixture
    {
        protected override string LanguageName => LanguageNames.CSharp;
        protected override DiagnosticAnalyzer CreateAnalyzer() => new UseNameOfAnalyzer();

        [Test]
        public void NoDiagnosticWhenThereAreNoArguments()
        {
            const string code = @"
using System;
class C
{
    void M()
    {
        throw new ArgumentNullException(""x"");
    }
}";

            NoDiagnostic(code, DiagnosticIds.UseNameOf);
        }

        [Test]
        public void NoDiagnosticWhenArgumentDoesntMatchAnyParameters()
        {
            const string code = @"
using System;
class C
{
    void M(int y, int z)
    {
        throw new ArgumentNullException(""x"");
    }
}";

            NoDiagnostic(code, DiagnosticIds.UseNameOf);
        }

        [Test]
        public void DiagnosticWhenArgumentMatchesParameter()
        {
            const string code = @"
using System;
class C
{
    void M(int x)
    {
        throw new ArgumentNullException([|""x""|]);
    }
}";

            HasDiagnostic(code, DiagnosticIds.UseNameOf);
        }

        [Test]
        public void NoDiagnosticWhenArgumentPassedToParameterThatIsntParamName()
        {
            const string code = @"
using System;
class C
{
    void M(int x)
    {
        throw new ArgumentException([|""x""|]);
    }
}";

            NoDiagnostic(code, DiagnosticIds.UseNameOf);
        }

        [Test]
        public void DiagnosticWhenArgumentPassedToNamedParameter()
        {
            const string code = @"
using System;
class C
{
    void M(int x)
    {
        throw new ArgumentException(paramName: [|""x""|], message: ""Hello"");
    }
}";

            HasDiagnostic(code, DiagnosticIds.UseNameOf);
        }

        [Test]
        public void DiagnosticWhenArgumentMatchesLambdaParameter()
        {
            const string code = @"
using System;
class C
{
    void M(int x)
    {
        Action<int> = y =>
        {
            throw new ArgumentException(paramName: [|""y""|], message: ""Hello"");
        };
    }
}";

            HasDiagnostic(code, DiagnosticIds.UseNameOf);
        }
    }
}
