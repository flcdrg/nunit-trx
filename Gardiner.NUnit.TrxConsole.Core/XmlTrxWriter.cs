using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using NUnit.Core;
using NUnit.Util;

namespace Gardiner.NUnit.TrxConsole.Core
{
    //
    // TRX Schema file - "C:\Program Files (x86)\Microsoft Visual Studio 10.0\Xml\Schemas\vstst.xsd"
    public class XmlTrxWriter
    {
        private readonly MemoryStream _memoryStream;
        private const string TestListId = "8c84fa94-04c1-424b-9868-57a2d4851a1d";
        private readonly Dictionary<string, TestData> _tests = new Dictionary<string, TestData>();
        private readonly TextWriter _writer;
        private readonly XmlWriter _xmlWriter;
        private DateTimeOffset _current;
        private string _storage = "";
        private Assembly _assembly;
        private Type[] _types;
        private Type _currentType;

        public XmlTrxWriter( string fileName )
        {
            _xmlWriter = new XmlTextWriter( new StreamWriter( fileName, false, Encoding.UTF8 ) );
        }

        public XmlTrxWriter( TextWriter writer )
        {
            var settings = new XmlWriterSettings {Indent = true, NewLineHandling = NewLineHandling.Replace};

            _writer = writer;
            _memoryStream = new MemoryStream();
            _xmlWriter = XmlWriter.Create( new StreamWriter( _memoryStream, Encoding.UTF8 ), settings );
        }

        private void TerminateXmlFile()
        {
            _xmlWriter.WriteEndElement(); // test-results
            _xmlWriter.WriteEndDocument();
            _xmlWriter.Flush();


            if ( _memoryStream != null && _writer != null )
            {
                _memoryStream.Position = 0;
                using ( var rdr = new StreamReader( _memoryStream ) )
                {
                    _writer.Write( rdr.ReadToEnd() );
                }
            }

            _xmlWriter.Close();
        }

        /*
        <?xml version="1.0" encoding="UTF-8"?>
        <TestRun id="66d6df1f-bd39-4dc9-ba31-9a125552c83d" name="David@CLARABEL 2012-05-11 07:56:56" runUser="CLARABEL\David" xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010">
          <TestSettings name="Local" id="ab5080a1-7318-41b4-9135-a1072add74a5">
            <Description>These are default test settings for a local test run.</Description>
            <Deployment enabled="false" runDeploymentRoot="David_CLARABEL 2012-05-11 07_56_56" />
            <Execution>
              <TestTypeSpecific />
              <AgentRule name="Execution Agents">
              </AgentRule>
            </Execution>
          </TestSettings>
          <Times creation="2012-05-11T07:56:56.0131205+09:30" queuing="2012-05-11T07:56:56.0881248+09:30" start="2012-05-11T07:56:56.1041257+09:30" finish="2012-05-11T07:56:56.5211496+09:30" />
          <ResultSummary outcome="Completed">
            <Counters total="1" executed="1" passed="1" error="0" failed="0" timeout="0" aborted="0" inconclusive="0" passedButRunAborted="0" notRunnable="0" notExecuted="0" disconnected="0" warning="0" completed="0" inProgress="0" pending="0" />
          </ResultSummary>
          <TestDefinitions>
            <UnitTest name="TestMethod1" storage="c:\dev\nunitsandbox\mstestsampletest\bin\debug\mstestsampletest.dll" id="675fc3a8-3d46-3d34-b659-7069842f8bfb">
              <Execution id="a9449235-dee2-46c2-80e7-2f32627cdf16" />
              <TestMethod codeBase="C:/dev/NUnitSandbox/MsTestSampleTest/bin/Debug/MsTestSampleTest.DLL" adapterTypeName="Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestAdapter, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.Adapter, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" className="MsTestSampleTest.UnitTest1, MsTestSampleTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" name="TestMethod1" />
            </UnitTest>
          </TestDefinitions>
          <TestLists>
            <TestList name="Results Not in a List" id="8c84fa94-04c1-424b-9868-57a2d4851a1d" />
            <TestList name="All Loaded Results" id="19431567-8539-422a-85d7-44ee4e166bda" />
          </TestLists>
          <TestEntries>
            <TestEntry testId="675fc3a8-3d46-3d34-b659-7069842f8bfb" executionId="a9449235-dee2-46c2-80e7-2f32627cdf16" testListId="8c84fa94-04c1-424b-9868-57a2d4851a1d" />
          </TestEntries>
          <Results>
            <UnitTestResult executionId="a9449235-dee2-46c2-80e7-2f32627cdf16" testId="675fc3a8-3d46-3d34-b659-7069842f8bfb" testName="TestMethod1" computerName="CLARABEL" duration="00:00:00.0564593" startTime="2012-05-11T07:56:56.1061258+09:30" endTime="2012-05-11T07:56:56.4961481+09:30" testType="13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b" outcome="Passed" testListId="8c84fa94-04c1-424b-9868-57a2d4851a1d" relativeResultsDirectory="a9449235-dee2-46c2-80e7-2f32627cdf16">
              <Output>
                <StdOut>Console</StdOut>
                <DebugTrace>Trace
        Debug
        </DebugTrace>
                <TextMessages>
                  <Message>TestContext</Message>
                </TextMessages>
              </Output>
            </UnitTestResult>
          </Results>
        </TestRun>
        */

        private Dictionary<string, Output> _testOutput;
        private bool _hasErrorMessage;
        private bool _hasStackTrace;
        private bool _hasOutput;
        private Output _output;
        private string _message;
        private string _stackTrace;

        public void SaveTestResult( TestResult result, Dictionary<string, Output> testOutput )
        {
            // http://blogs.msdn.com/b/dhopton/archive/2008/06/12/helpful-internals-of-trx-and-vsmdi-files.aspx
            var summary = new ResultSummarizer( result );

            _testOutput = testOutput;


            _xmlWriter.WriteStartDocument();

            _xmlWriter.WriteStartElement( "TestRun", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010" );

            _xmlWriter.WriteAttributeString( "id", Guid.NewGuid().ToString( "D" ) );
            _xmlWriter.WriteAttributeString( "runUser",
                                             string.Format( "{0}\\{1}", Environment.UserDomainName, Environment.UserName ) );
            _xmlWriter.WriteAttributeString( "name",
                                             string.Format( "{0}@{1} {2}", Environment.UserName, Environment.MachineName,
                                                            DateTime.Now ) );

            _xmlWriter.WriteStartElement( "TestSettings" );
            _xmlWriter.WriteAttributeString( "name", "Local" );
            _xmlWriter.WriteAttributeString( "id", Guid.NewGuid().ToString( "D" ) );

            _xmlWriter.WriteElementString( "Description", "test setting description" );

            _xmlWriter.WriteStartElement( "Execution" );
            _xmlWriter.WriteElementString( "TestTypeSpecific", "" );

            _xmlWriter.WriteStartElement( "AgentRule" );
            _xmlWriter.WriteAttributeString( "name", "Execution Agents" );
            _xmlWriter.WriteEndElement(); // AgentRule

            _xmlWriter.WriteEndElement(); // Execution
            _xmlWriter.WriteEndElement(); // TestSettings

            TimeSpan duration = TimeSpan.FromSeconds( result.Time );
            _current = DateTimeOffset.UtcNow.Add( duration );

            _xmlWriter.WriteStartElement( "Times" );
            _xmlWriter.WriteAttributeString( "creation", _current.LocalDateTime.ToString( "O" ) );
            _xmlWriter.WriteAttributeString( "queuing", _current.LocalDateTime.ToString( "O" ) );
            _xmlWriter.WriteAttributeString( "start", _current.LocalDateTime.ToString( "O" ) );


            _xmlWriter.WriteAttributeString( "finish", ( _current + duration ).LocalDateTime.ToString( "O" ) );
            _xmlWriter.WriteEndElement();

            _xmlWriter.WriteStartElement( "ResultSummary" );
            WriteOutcomeAttribute( result );

            // <Counters total="1" executed="1" passed="1" error="0" failed="0" timeout="0" aborted="0" inconclusive="0" 
            // passedButRunAborted="0" notRunnable="0" notExecuted="0" disconnected="0" warning="0" completed="0" inProgress="0" pending="0" />
            _xmlWriter.WriteStartElement( "Counters" );

            _xmlWriter.WriteAttributeString( "total", summary.ResultCount.ToString( CultureInfo.InvariantCulture ) );
            _xmlWriter.WriteAttributeString( "executed", summary.TestsRun.ToString( CultureInfo.InvariantCulture ) );
            _xmlWriter.WriteAttributeString( "passed", summary.Passed.ToString( CultureInfo.InvariantCulture ) );
            _xmlWriter.WriteAttributeString( "error", summary.Errors.ToString( CultureInfo.InvariantCulture ) );
            _xmlWriter.WriteAttributeString( "failed", summary.Failures.ToString( CultureInfo.InvariantCulture ) );
            _xmlWriter.WriteAttributeString( "timeout", "0" );
            _xmlWriter.WriteAttributeString( "aborted", "0" );
            _xmlWriter.WriteAttributeString( "inconclusive",
                                             summary.Inconclusive.ToString( CultureInfo.InvariantCulture ) );
            _xmlWriter.WriteAttributeString( "passedButRunAborted", "0" );
            _xmlWriter.WriteAttributeString( "notRunnable", summary.NotRunnable.ToString( CultureInfo.InvariantCulture ) );
            _xmlWriter.WriteAttributeString( "notExecuted", summary.TestsNotRun.ToString( CultureInfo.InvariantCulture ) );
            _xmlWriter.WriteAttributeString( "disconnected", "0" );
            _xmlWriter.WriteAttributeString( "warning", "0" );
            _xmlWriter.WriteAttributeString( "completed", "0" );
            _xmlWriter.WriteAttributeString( "inProgress", "0" );
            _xmlWriter.WriteAttributeString( "pending", "0" );
/*
            _xmlWriter.WriteAttributeString( "ignored", summary.Ignored.ToString( CultureInfo.InvariantCulture ) );
            _xmlWriter.WriteAttributeString( "skipped", summary.Skipped.ToString( CultureInfo.InvariantCulture ) );
*/
            _xmlWriter.WriteEndElement(); // Counters

            _xmlWriter.WriteEndElement(); // ResultSummary

            _xmlWriter.WriteStartElement( "TestDefinitions" );


            WriteUnitTest( result );

            _xmlWriter.WriteEndElement(); // TestDefinitions

            _xmlWriter.WriteStartElement( "TestLists" );
            //     <TestList name="Results Not in a List" id="8c84fa94-04c1-424b-9868-57a2d4851a1d" />
/*
            _xmlWriter.WriteStartElement( "TestList" );
            _xmlWriter.WriteAttributeString( "name", "Results Not in a List" );
            _xmlWriter.WriteAttributeString( "id", "8c84fa94-04c1-424b-9868-57a2d4851a1d" ); // this is a magic guid VS expects to exist
            _xmlWriter.WriteEndElement(); // TestList
*/

            //     <TestList name="All Loaded Results" id="19431567-8539-422a-85d7-44ee4e166bda" />
            _xmlWriter.WriteStartElement( "TestList" );
            _xmlWriter.WriteAttributeString( "name", "All Loaded Results" );
            _xmlWriter.WriteAttributeString( "id", "19431567-8539-422a-85d7-44ee4e166bda" );
                // this is a magic guid VS expects to exist
            _xmlWriter.WriteEndElement(); // TestList

            _xmlWriter.WriteEndElement(); // TestLists

            _xmlWriter.WriteStartElement( "TestEntries" );
            WriteTestEntry( result );

            _xmlWriter.WriteEndElement(); // TestEntries

            _xmlWriter.WriteStartElement( "Results" );
            WriteResult( result );
            _xmlWriter.WriteEndElement(); // Results

/*
            result.

            DateTime now = DateTime.Now;
            _xmlWriter.WriteAttributeString( "date", XmlConvert.ToString( now, "yyyy-MM-dd" ) );
            _xmlWriter.WriteAttributeString( "time", XmlConvert.ToString( now, "HH:mm:ss" ) );
*/
            TerminateXmlFile();
        }

        private void WriteTestEntry( TestResult result )
        {
            if ( result.HasResults )
            {
                foreach ( TestResult childResult in result.Results )
                {
                    WriteTestEntry( childResult );
                }
            }

            if ( !result.Test.IsSuite && !result.HasResults )
            {
                //string testId = Guid.NewGuid().ToString();

                var testData = _tests[ result.FullName ];
                //testData.TestId = testId;

                // <TestEntry testId="3b9c939b-7610-7a8b-d417-986f31f5d827" executionId="375aceff-59a4-4a04-895f-80e037859456" testListId="8c84fa94-04c1-424b-9868-57a2d4851a1d" />
                _xmlWriter.WriteStartElement( "TestEntry" );
                _xmlWriter.WriteAttributeString( "testListId", TestListId );
                _xmlWriter.WriteAttributeString( "testId", testData.TestId );
                _xmlWriter.WriteAttributeString( "executionId", testData.TestId );
                _xmlWriter.WriteEndElement(); // TestEntry
            }
        }

        private void WriteResult( TestResult result )
        {
            // if failure in setup/cleanup then error will be higher up
            if ( result.FailureSite != FailureSite.Child && result.FailureSite != FailureSite.Parent )
            {
                _hasErrorMessage = !string.IsNullOrEmpty( result.Message );
                _hasStackTrace = !string.IsNullOrEmpty( result.StackTrace );
                _hasOutput = _testOutput.ContainsKey( result.FullName );

                if ( _hasOutput )
                    _output = _testOutput[ result.FullName ];

                if (_hasErrorMessage)
                    _message = result.Message;

                if ( _hasStackTrace )
                    _stackTrace = result.StackTrace;
            }

            if ( result.HasResults )
            {
                foreach ( TestResult childResult in result.Results )
                {
                    WriteResult( childResult );
                }
            }

            // 2012-05-11T07:56:56.1061258+09:30
            // 2012-05-14T20:57:07.1170928+09:30 

            //    <UnitTestResult executionId="a9449235-dee2-46c2-80e7-2f32627cdf16" testId="675fc3a8-3d46-3d34-b659-7069842f8bfb" 
            //            testName="TestMethod1" computerName="CLARABEL" duration="00:00:00.0564593" startTime="2012-05-11T07:56:56.1061258+09:30" 
            //            endTime="2012-05-11T07:56:56.4961481+09:30" testType="13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b" outcome="Passed" 
            //            testListId="8c84fa94-04c1-424b-9868-57a2d4851a1d" relativeResultsDirectory="a9449235-dee2-46c2-80e7-2f32627cdf16">
            //        <Output>
            //        <StdOut>Console</StdOut>
            //        <DebugTrace>Trace
            //Debug
            //</DebugTrace>
            //        <TextMessages>
            //            <Message>TestContext</Message>
            //        </TextMessages>
            //        </Output>
            //    </UnitTestResult>

            //     <UnitTestResult executionId="f49a51fa-b18d-47f4-b2ed-ce35804e9005" testId="3b9c939b-7610-7a8b-d417-986f31f5d827" 
            // testName="WithAssert" computerName="CLARABEL" duration="00:00:00.0001652" startTime="2012-05-25T23:13:06.1590260+09:30" 
            // endTime="2012-05-25T23:13:06.1620261+09:30" testType="13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b" outcome="Passed" 
            // testListId="8c84fa94-04c1-424b-9868-57a2d4851a1d" relativeResultsDirectory="f49a51fa-b18d-47f4-b2ed-ce35804e9005">

            if ( !result.Test.IsSuite && !result.HasResults )
            {
                TimeSpan duration = TimeSpan.FromSeconds( result.Time );

                var testData = _tests[ result.FullName ];
                _xmlWriter.WriteStartElement( "UnitTestResult" );

                _xmlWriter.WriteAttributeString( "executionId", testData.TestId );
                _xmlWriter.WriteAttributeString( "testId", testData.TestId );
                _xmlWriter.WriteAttributeString( "testName", result.Name );
                _xmlWriter.WriteAttributeString( "computerName", Environment.MachineName );
                _xmlWriter.WriteAttributeString( "duration", duration.ToString( "G", CultureInfo.InvariantCulture ) );

                _xmlWriter.WriteAttributeString( "startTime", _current.LocalDateTime.ToString( "O" ) );
                _current += duration;
                _xmlWriter.WriteAttributeString( "endTime", _current.LocalDateTime.ToString( "O" ) );
                _xmlWriter.WriteAttributeString( "testType", "13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b" );

                WriteOutcomeAttribute( result );
                _xmlWriter.WriteAttributeString( "testListId", TestListId );

                _xmlWriter.WriteAttributeString( "relativeResultsDirectory", testData.ExecutionId );

//      <Output>
//        <ErrorInfo>
//          <Message>Assert.IsTrue failed. Surprisingly, this test failed</Message>
//          <StackTrace>   at MsTestSampleTest.UnitTest1.AssertFails() in C:\dev\Gardiner.NUnit.TrxConsole\TestSamples\Gardiner.NUnit.MsTestSampleTest\UnitTest1.cs:line 61
//</StackTrace>
//        </ErrorInfo>
//      </Output>


                if (result.ResultState != ResultState.Skipped && result.ResultState != ResultState.Ignored && (_hasOutput || _hasErrorMessage || _hasStackTrace))
                {
                    _xmlWriter.WriteStartElement( "Output" );

                    if ( _hasOutput )
                    {
                        string text = _output.Out;

                        if (!string.IsNullOrEmpty(text))
                        {
                            _xmlWriter.WriteStartElement( "StdOut" );
                            _xmlWriter.WriteString( text );
                            _xmlWriter.WriteEndElement(); // stdout
                        }

                        text = _output.Trace;

                        if ( !string.IsNullOrEmpty( text ) )
                        {
                            _xmlWriter.WriteStartElement( "DebugTrace" );
                            _xmlWriter.WriteString( text );
                            _xmlWriter.WriteEndElement(); // DebugTrace
                        }

                    }

                    if ( _hasErrorMessage || _hasStackTrace )
                    {
                        _xmlWriter.WriteStartElement( "ErrorInfo" );

                        if ( _hasErrorMessage )
                            _xmlWriter.WriteElementString( "Message", _message );

                        if (_hasStackTrace)
                        {
                            _xmlWriter.WriteElementString( "StackTrace", _stackTrace );
                        }

                        _xmlWriter.WriteEndElement(); // errorinfo
                    }
                    _xmlWriter.WriteEndElement(); // output
                    
                }
                _xmlWriter.WriteEndElement(); // UnitTestResult
            }
        }

        private void WriteOutcomeAttribute( TestResult result )
        {
//<xs:simpleType name="TestOutcome">
            //  <xs:restriction base="xs:string">
            //    <xs:enumeration value="Error"/>
            //    <xs:enumeration value="Failed"/>
            //    <xs:enumeration value="Timeout"/>
            //    <xs:enumeration value="Aborted"/>
            //    <xs:enumeration value="Inconclusive"/>
            //    <xs:enumeration value="PassedButRunAborted"/>
            //    <xs:enumeration value="NotRunnable"/>
            //    <xs:enumeration value="NotExecuted"/>
            //    <xs:enumeration value="Disconnected"/>
            //    <xs:enumeration value="Warning"/>
            //    <xs:enumeration value="Passed"/>
            //    <xs:enumeration value="Completed"/>
            //    <xs:enumeration value="InProgress"/>
            //    <xs:enumeration value="Pending"/>
            //  </xs:restriction>
            //</xs:simpleType>

            string outcome;
            switch ( result.ResultState )
            {
                case ResultState.Cancelled:
                    outcome = "Aborted";
                    break;

                case ResultState.Ignored:
                case ResultState.Skipped:
                    outcome = "NotExecuted";
                    break;

                case ResultState.Error:
                case ResultState.Failure:
                    outcome = "Failed";
                    break;

                case ResultState.Inconclusive:
                    outcome = "Inconclusive";
                    break;

                case ResultState.NotRunnable:
                    outcome = "NotRunnable";
                    break;

                case ResultState.Success:
                    outcome = "Passed";
                    break;
                default:
                    outcome = string.Empty;
                    break;
            }
            _xmlWriter.WriteAttributeString( "outcome", outcome );
        }

        private void WriteUnitTest( TestResult result )
        {
            //<UnitTest name="TestMethod1" storage="c:\dev\nunitsandbox\mstestsampletest\bin\debug\mstestsampletest.dll" id="675fc3a8-3d46-3d34-b659-7069842f8bfb">
            //    <Execution id="a9449235-dee2-46c2-80e7-2f32627cdf16" />
            //    <TestMethod codeBase="C:/dev/NUnitSandbox/MsTestSampleTest/bin/Debug/MsTestSampleTest.DLL" 
            //        adapterTypeName="Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestAdapter, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.Adapter, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" 
            //        className="MsTestSampleTest.UnitTest1, MsTestSampleTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null" name="TestMethod1" />
            //</UnitTest>

            if ( result.Test.IsSuite && result.Test.TestType == "Assembly" )
            {
                _assembly = Assembly.LoadFrom( result.Test.TestName.FullName );

                _storage = Path.GetFileName( _assembly.Location );

                try
                {
                    _types = _assembly.GetTypes();
                }
                catch ( ReflectionTypeLoadException ex )
                {
                    Console.Error.WriteLine( "Error loading types from test assembly.\n{0}", ex );

                    foreach ( var loaderException in ex.LoaderExceptions )
                    {
                        Console.Error.WriteLine("\t{0}", loaderException);
                    }
                }
            }

            if ( result.Test.TestType == "TestFixture" )
            {
                _currentType = _types.First(
                    x => x.FullName == result.FullName
                    );
            }

            if ( !result.Test.IsSuite )
            {
                _xmlWriter.WriteStartElement( "UnitTest" );
                _xmlWriter.WriteAttributeString( "name", result.Name );
                _xmlWriter.WriteAttributeString( "storage", _storage );
                var executionId = Guid.NewGuid().ToString();
                _xmlWriter.WriteAttributeString( "id", executionId );

                //<TestCategory>
                //  <TestCategoryItem TestCategory="ACategory" />
                //</TestCategory>
                if (result.Test.Categories.Count > 0)
                {
                    _xmlWriter.WriteStartElement( "TestCategory" );
                    foreach ( var category in result.Test.Categories )
                    {
                        _xmlWriter.WriteStartElement( "TestCategoryItem" );
                        _xmlWriter.WriteAttributeString( "TestCategory", category.ToString() );
                        _xmlWriter.WriteEndElement();
                    }
                    _xmlWriter.WriteEndElement();
                    
                }
                _xmlWriter.WriteStartElement( "Execution" );
                _xmlWriter.WriteAttributeString( "id", executionId );
                _xmlWriter.WriteEndElement(); // Execution

                string name = result.FullName;

                _tests.Add( name, new TestData( executionId ) );

                // <TestMethod codeBase="C:/dev/Gardiner.NUnit.TrxConsole/TestSamples/Gardiner.NUnit.MsTestSampleTest/bin/Debug/MsTestSampleTest.DLL" 
                //  adapterTypeName="Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestAdapter, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.Adapter, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
                //  className="MsTestSampleTest.UnitTest1, MsTestSampleTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
                //  name="TestMethod1" />

                
                _xmlWriter.WriteStartElement( "TestMethod" );
                var codeBase = new Uri( _assembly.CodeBase );
                _xmlWriter.WriteAttributeString( "codeBase", codeBase.AbsolutePath );
                _xmlWriter.WriteAttributeString( "adapterTypeName",
                                                 "Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestAdapter, Microsoft.VisualStudio.QualityTools.Tips.UnitTest.Adapter, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" );


                _xmlWriter.WriteAttributeString( "className", _currentType.AssemblyQualifiedName );
                                                 //string.Format( "{0}, {1}", result.FullName, "NUnitSandbox.Class1" ) );
                _xmlWriter.WriteAttributeString( "name", result.Name );

                _xmlWriter.WriteEndElement(); // TestMethod

                _xmlWriter.WriteEndElement(); // UnitTest
            }
            if ( result.HasResults )
            {
                foreach ( TestResult childResult in result.Results )
                {
                    WriteUnitTest( childResult );
                }
            }
        }
    }
}