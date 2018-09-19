﻿using System.Threading.Tasks;
using Agoda.Analyzers.AgodaCustom;
using Agoda.Analyzers.Test.Helpers;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace Agoda.Analyzers.Test.AgodaCustom
{
    internal class AG0025UnitTests : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer DiagnosticAnalyzer => new AG0025PreventUseOfTaskContinue();
        
        protected override string DiagnosticId => AG0025PreventUseOfTaskContinue.DIAGNOSTIC_ID;
        
        [Test]
        public async Task AG0025_WhenInvokeStart_ShouldNotShowWarning()
        {
            var code = @"
using System.Threading.Tasks;

class TestClass 
{
    public void TestMethod1() 
    {
        new Task(() => {}).Start();
    }
}
";
            
            await VerifyDiagnosticResults(code);
        }

        [Test]
        public async Task AG0025_WhenInvokeCustomTaskContinue_ShouldNotShowWarning()
        {
            var code = @"
class Task 
{
    public void Continue() {}
}

class TestClass 
{
    public void TestMethod1() 
    {
        new Task().Continue();
    }
}
";

            await VerifyDiagnosticResults(code);
        }

        [Test]
        public async Task AG0025_WhenInvokeContinueWith_ShouldShowWarning()
        {
            var code = @"
using System.Threading.Tasks;

class TestClass 
{
    public void TestMethod1() 
    {
        new Task(() => {}).ContinueWith(null);
    }
}
";

            await VerifyDiagnosticResults(code, new DiagnosticLocation(8, 9));
        }

        [Test]
        public async Task AG0025_WhenInvokeContinueGeneric_ShouldShowWarning()
        {
            var code = @"
using System.Threading.Tasks;

class TestClass 
{
    public void TestMethod1() 
    {
        var task = Task.CompletedTask;
        task.ContinueWith<string>(null);
    }
}
";
            
            await VerifyDiagnosticResults(code, new DiagnosticLocation(9, 9));
        }
    }
}