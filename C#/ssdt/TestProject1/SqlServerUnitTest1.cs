using Microsoft.Data.Tools.Schema.Sql.UnitTesting;
using Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace TestProject1
{
    [TestClass()]
    public class SqlServerUnitTest1 : SqlDatabaseTestClass
    {

        public SqlServerUnitTest1()
        {
            InitializeComponent();
        }

        [TestInitialize()]
        public void TestInitialize()
        {
            base.InitializeTest();
        }
        [TestCleanup()]
        public void TestCleanup()
        {
            base.CleanupTest();
        }

        #region Designer support code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction dbo_GetFooBarInfoTest_TestAction;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SqlServerUnitTest1));
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction dbo_GetFooBarInfoTest_PretestAction;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction dbo_GetFooBarInfoTest_PosttestAction;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.RowCountCondition rowCountCondition1;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition scalarValueCondition1;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition scalarValueCondition2;
            Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition scalarValueCondition3;
            this.dbo_GetFooBarInfoTestData = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestActions();
            dbo_GetFooBarInfoTest_TestAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            dbo_GetFooBarInfoTest_PretestAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            dbo_GetFooBarInfoTest_PosttestAction = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.SqlDatabaseTestAction();
            rowCountCondition1 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.RowCountCondition();
            scalarValueCondition1 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            scalarValueCondition2 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            scalarValueCondition3 = new Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions.ScalarValueCondition();
            // 
            // dbo_GetFooBarInfoTest_TestAction
            // 
            dbo_GetFooBarInfoTest_TestAction.Conditions.Add(rowCountCondition1);
            dbo_GetFooBarInfoTest_TestAction.Conditions.Add(scalarValueCondition1);
            dbo_GetFooBarInfoTest_TestAction.Conditions.Add(scalarValueCondition2);
            dbo_GetFooBarInfoTest_TestAction.Conditions.Add(scalarValueCondition3);
            resources.ApplyResources(dbo_GetFooBarInfoTest_TestAction, "dbo_GetFooBarInfoTest_TestAction");
            // 
            // dbo_GetFooBarInfoTestData
            // 
            this.dbo_GetFooBarInfoTestData.PosttestAction = dbo_GetFooBarInfoTest_PosttestAction;
            this.dbo_GetFooBarInfoTestData.PretestAction = dbo_GetFooBarInfoTest_PretestAction;
            this.dbo_GetFooBarInfoTestData.TestAction = dbo_GetFooBarInfoTest_TestAction;
            // 
            // dbo_GetFooBarInfoTest_PretestAction
            // 
            resources.ApplyResources(dbo_GetFooBarInfoTest_PretestAction, "dbo_GetFooBarInfoTest_PretestAction");
            // 
            // dbo_GetFooBarInfoTest_PosttestAction
            // 
            resources.ApplyResources(dbo_GetFooBarInfoTest_PosttestAction, "dbo_GetFooBarInfoTest_PosttestAction");
            // 
            // rowCountCondition1
            // 
            rowCountCondition1.Enabled = true;
            rowCountCondition1.Name = "rowCountCondition1";
            rowCountCondition1.ResultSet = 1;
            rowCountCondition1.RowCount = 1;
            // 
            // scalarValueCondition1
            // 
            scalarValueCondition1.ColumnNumber = 1;
            scalarValueCondition1.Enabled = true;
            scalarValueCondition1.ExpectedValue = "foo";
            scalarValueCondition1.Name = "scalarValueCondition1";
            scalarValueCondition1.NullExpected = false;
            scalarValueCondition1.ResultSet = 1;
            scalarValueCondition1.RowNumber = 1;
            // 
            // scalarValueCondition2
            // 
            scalarValueCondition2.ColumnNumber = 2;
            scalarValueCondition2.Enabled = true;
            scalarValueCondition2.ExpectedValue = "bar";
            scalarValueCondition2.Name = "scalarValueCondition2";
            scalarValueCondition2.NullExpected = false;
            scalarValueCondition2.ResultSet = 1;
            scalarValueCondition2.RowNumber = 1;
            // 
            // scalarValueCondition3
            // 
            scalarValueCondition3.ColumnNumber = 2;
            scalarValueCondition3.Enabled = true;
            scalarValueCondition3.ExpectedValue = "foobar";
            scalarValueCondition3.Name = "scalarValueCondition3";
            scalarValueCondition3.NullExpected = false;
            scalarValueCondition3.ResultSet = 1;
            scalarValueCondition3.RowNumber = 1;
        }

        #endregion


        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        #endregion

        [TestMethod()]
        public void dbo_GetFooBarInfoTest()
        {
            SqlDatabaseTestActions testActions = this.dbo_GetFooBarInfoTestData;
            // 執行測試前指令碼
            // 
            System.Diagnostics.Trace.WriteLineIf((testActions.PretestAction != null), "正在執行 測試前 指令碼...");
            SqlExecutionResult[] pretestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PretestAction);
            try
            {
                // 執行測試指令碼
                // 
                System.Diagnostics.Trace.WriteLineIf((testActions.TestAction != null), "正在執行 測試 指令碼...");
                SqlExecutionResult[] testResults = TestService.Execute(this.ExecutionContext, this.PrivilegedContext, testActions.TestAction);
            }
            finally
            {
                // 執行測試後指令碼
                // 
                System.Diagnostics.Trace.WriteLineIf((testActions.PosttestAction != null), "正在執行 測試後 指令碼...");
                SqlExecutionResult[] posttestResults = TestService.Execute(this.PrivilegedContext, this.PrivilegedContext, testActions.PosttestAction);
            }
        }
        private SqlDatabaseTestActions dbo_GetFooBarInfoTestData;
    }
}
