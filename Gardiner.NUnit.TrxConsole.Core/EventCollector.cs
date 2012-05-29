// ****************************************************************
// Copyright 2007, Charlie Poole
// This is free software licensed under the NUnit license. You may
// obtain a copy of the license at http://nunit.org
// ****************************************************************

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.ConsoleRunner;
using NUnit.Core;
using NUnit.Util;

namespace Gardiner.NUnit.TrxConsole.Core
{
    /// <summary>
    /// Summary description for EventCollector.
    /// </summary>
    /// <remarks>
    /// This is the NUnit EventCollector, with just some methods made virtual and 
    /// some fields made accessible to inheriting classes
    /// </remarks>
    public class EventCollector : MarshalByRefObject, EventListener
    {
        private readonly TextWriter errorWriter;
        private readonly ConsoleOptions options;
        private readonly TextWriter outWriter;

        private readonly bool progress;

        private readonly ArrayList unhandledExceptions = new ArrayList();
        protected string currentTestName;
        private int failureCount;
        private int level;
        private StringCollection messages;
        private int testIgnoreCount;
        private int testRunCount;



        public EventCollector( ConsoleOptions options, TextWriter outWriter, TextWriter errorWriter )
        {
            level = 0;
            this.options = options;
            this.outWriter = outWriter;
            this.errorWriter = errorWriter;
            currentTestName = string.Empty;
            progress = !options.xmlConsole && !options.labels && !options.nodots;

            AppDomain.CurrentDomain.UnhandledException +=
                OnUnhandledException;
        }

        public bool HasExceptions
        {
            get { return unhandledExceptions.Count > 0; }
        }

        public void RunStarted( string name, int testCount )
        {
        }

        public void RunFinished( TestResult result )
        {
        }

        public void RunFinished( Exception exception )
        {
        }

        public void TestFinished( TestResult testResult )
        {
            switch ( testResult.ResultState )
            {
                case ResultState.Error:
                case ResultState.Failure:
                case ResultState.Cancelled:
                    testRunCount++;
                    failureCount++;

                    if ( progress )
                        Console.Write( "F" );

                    messages.Add( string.Format( "{0}) {1} :", failureCount, testResult.Test.TestName.FullName ) );
                    messages.Add( testResult.Message.Trim( Environment.NewLine.ToCharArray() ) );

                    string stackTrace = StackTraceFilter.Filter( testResult.StackTrace );
                    if ( stackTrace != null && stackTrace != string.Empty )
                    {
                        string[] trace = stackTrace.Split( Environment.NewLine.ToCharArray() );
                        foreach ( string s in trace )
                        {
                            if ( s != string.Empty )
                            {
                                string link = Regex.Replace( s.Trim(), @".* in (.*):line (.*)", "$1($2)" );
                                messages.Add( string.Format( "at\n{0}", link ) );
                            }
                        }
                    }
                    break;

                case ResultState.Inconclusive:
                case ResultState.Success:
                    testRunCount++;
                    break;

                case ResultState.Ignored:
                case ResultState.Skipped:
                case ResultState.NotRunnable:
                    testIgnoreCount++;

                    if ( progress )
                        Console.Write( "N" );
                    break;
            }

            currentTestName = string.Empty;
        }

        public virtual void TestStarted( TestName testName )
        {
            currentTestName = testName.FullName;

            if ( options.labels )
                outWriter.WriteLine( "***** {0}", currentTestName );

            if ( progress )
                Console.Write( "." );
        }

        public void SuiteStarted( TestName testName )
        {
            if ( level++ == 0 )
            {
                messages = new StringCollection();
                testRunCount = 0;
                testIgnoreCount = 0;
                failureCount = 0;
                Trace.WriteLine( "################################ UNIT TESTS ################################" );
                Trace.WriteLine( "Running tests in '" + testName.FullName + "'..." );
            }
        }

        public void SuiteFinished( TestResult suiteResult )
        {
            if ( --level == 0 )
            {
                Trace.WriteLine( "############################################################################" );

                if ( messages.Count == 0 )
                {
                    Trace.WriteLine( "##############                 S U C C E S S               #################" );
                }
                else
                {
                    Trace.WriteLine( "##############                F A I L U R E S              #################" );

                    foreach ( string s in messages )
                    {
                        Trace.WriteLine( s );
                    }
                }

                Trace.WriteLine( "############################################################################" );
                Trace.WriteLine( "Executed tests       : " + testRunCount );
                Trace.WriteLine( "Ignored tests        : " + testIgnoreCount );
                Trace.WriteLine( "Failed tests         : " + failureCount );
                Trace.WriteLine( "Unhandled exceptions : " + unhandledExceptions.Count );
                Trace.WriteLine( "Total time           : " + suiteResult.Time + " seconds" );
                Trace.WriteLine( "############################################################################" );
            }
        }


        public void UnhandledException( Exception exception )
        {
            // If we do labels, we already have a newline
            unhandledExceptions.Add( currentTestName + " : " + exception );
            //if (!options.labels) outWriter.WriteLine();
            string msg = string.Format( "##### Unhandled Exception while running {0}", currentTestName );
            //outWriter.WriteLine(msg);
            //outWriter.WriteLine(exception.ToString());

            Trace.WriteLine( msg );
            Trace.WriteLine( exception.ToString() );
        }

        public virtual void TestOutput( TestOutput output )
        {
            switch ( output.Type )
            {
                case TestOutputType.Out:
                    outWriter.Write( output.Text );
                    break;
                case TestOutputType.Error:
                    errorWriter.Write( output.Text );
                    break;
            }
        }
        
        public void WriteExceptions()
        {
            Console.WriteLine();
            Console.WriteLine( "Unhandled exceptions:" );
            int index = 1;
            foreach ( string msg in unhandledExceptions )
                Console.WriteLine( "{0}) {1}", index++, msg );
        }

        private void OnUnhandledException( object sender, UnhandledExceptionEventArgs e )
        {
            if ( e.ExceptionObject.GetType() != typeof (ThreadAbortException) )
            {
                UnhandledException( (Exception) e.ExceptionObject );
            }
        }


        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}