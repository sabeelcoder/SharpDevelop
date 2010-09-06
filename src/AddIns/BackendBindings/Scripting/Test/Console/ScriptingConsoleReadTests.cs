﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Threading;
using System.Windows.Input;

using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	/// <summary>
	/// Tests the ScriptingConsole ReadLine method.
	/// </summary>
	[TestFixture]
	public class ScriptingConsoleReadTests : ScriptingConsoleTestsBase
	{
		int initialAutoIndentSize = 4;
		string readLine;
		int autoIndentSize;
		bool raiseKeyPressEvent;
		bool raiseDialogKeyPressEvent;
		
		[TestFixtureSetUp]
		public void Init()
		{
			base.CreateConsole();
			
			autoIndentSize = initialAutoIndentSize;
			Thread thread = new Thread(ReadLineFromConsoleOnDifferentThread);
			thread.Start();
						
			int sleepInterval = 10;
			int maxWait = 2000;
			int currentWait = 0;
			while ((MockConsoleTextEditor.Text.Length < autoIndentSize) && (currentWait < maxWait)) {
				Thread.Sleep(sleepInterval);
				currentWait += sleepInterval;
			}
			
			raiseKeyPressEvent = MockConsoleTextEditor.RaisePreviewKeyDownEvent(Key.A);
			raiseDialogKeyPressEvent = MockConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			currentWait = 0;
			while ((MockConsoleTextEditor.Text.Length < autoIndentSize + 2) && (currentWait < maxWait)) {
				Thread.Sleep(10);
				currentWait += sleepInterval;				
			}
			thread.Join(2000);
		}
		
		[TestFixtureTearDown]
		public void TearDown()
		{
			TestableScriptingConsole.Dispose();
		}

		[Test]
		public void ReadLineFromConsole()
		{
			string expectedString = String.Empty.PadLeft(initialAutoIndentSize) + "A";
			Assert.AreEqual(expectedString, readLine);
		}
		
		[Test]
		public void ReadLineWithNonZeroAutoIndentSizeWritesSpacesToTextEditor()
		{
			string expectedString = String.Empty.PadLeft(initialAutoIndentSize) + "A\r\n";
			Assert.AreEqual(expectedString, MockConsoleTextEditor.Text);
		}
		
		[Test]
		public void DocumentInsertCalledWhenAutoIndentIsNonZero()
		{
			Assert.IsTrue(MockConsoleTextEditor.IsWriteCalled);
		}
		
		[Test]
		public void NoTextWrittenWhenAutoIndentSizeIsZero()
		{
			TestableScriptingConsole console = new TestableScriptingConsole();
			MockConsoleTextEditor textEditor = console.MockConsoleTextEditor;
			textEditor.RaisePreviewKeyDownEvent(Key.A);
			textEditor.RaisePreviewKeyDownEventForDialogKey(Key.Enter);
			
			textEditor.IsWriteCalled = false;
			console.ReadLine(0);
			Assert.IsFalse(textEditor.IsWriteCalled);
		}
		
		/// <summary>
		/// Should return false for any character that should be handled by the text editor itself.
		/// </summary>
		[Test]
		public void RaiseKeyPressEventReturnedFalse()
		{
			Assert.IsFalse(raiseKeyPressEvent);
		}
		
		/// <summary>
		/// Should return false for any character that should be handled by the text editor itself.
		/// </summary>
		[Test]
		public void RaiseDialogKeyPressEventReturnedFalse()
		{
			Assert.IsFalse(raiseDialogKeyPressEvent);
		}
		
		void ReadLineFromConsoleOnDifferentThread()
		{
			System.Console.WriteLine("Reading on different thread");
			readLine = TestableScriptingConsole.ReadLine(autoIndentSize);
			System.Console.WriteLine("Finished reading on different thread");
		}
	}
}