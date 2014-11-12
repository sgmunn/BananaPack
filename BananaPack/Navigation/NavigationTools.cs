//   --------------------------------------------------------------------------------------------------------------------
//   <copyright file="NavigationTools.cs" company="(c) Greg Munn">
//     (c) 2014 (c) Greg Munn  
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//     documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
//     the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
//     to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
//     the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
//     THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
//     CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
//     IN THE SOFTWARE.
//   </copyright>
//   --------------------------------------------------------------------------------------------------------------------
using System;
//using ICSharpCode.NRefactory.Semantics;
//using MonoDevelop.Ide.Gui.Content;
using MonoDevelop.Ide.Gui;
using ICSharpCode.NRefactory.TypeSystem;
//using ICSharpCode.NRefactory.CSharp.Resolver;
//using System.Linq;
using MonoDevelop.Core;
using MonoDevelop.Ide.FindInFiles;
using System.Threading;
using Mono.TextEditor;
using MonoDevelop.Ide;
using BananaTools.CodeDom;
using BananaTools.Ide;
using System.Linq;
using System.IO;

namespace BananaTools.Navigation
{
	public static class NavigationTools
	{
		//		public static Caret GetCaretInActiveDocument()
		//		{
		//			var editor = IdeApp.Workbench.ActiveDocument.GetContent<ITextEditorDataProvider>().GetTextEditorData();
		//			return editor.Caret;
		//		}
//
//		public static void GotoToNextReferenceAtCaret(Document document)
//		{
//			var searchTarget = CodeDomHelpers.GetEntityAtCaret(document, true);
//			if (searchTarget != null)
//			{
//				var nextItem = ReferenceNavigationLoop.GetNextSearchTarget(searchTarget);
//				if (!nextItem.Equals(NavigationItem.Empty))
//				{
//					IdeApp.Workbench.OpenDocument(nextItem.FileName, nextItem.Line, nextItem.Col);
//				}
//				else if (searchTarget is IVariable || searchTarget is IEntity)
//				{
//					FindReferencesForNavigation(searchTarget, true);
//				}
//			}
//		}
//
//		public static void GotoToPreviousReferenceAtCaret(Document document)
//		{
//			var searchTarget = CodeDomHelpers.GetEntityAtCaret(document, true);
//			if (searchTarget != null)
//			{
//				var nextItem = ReferenceNavigationLoop.GetPreviousSearchTarget(searchTarget);
//				if (!nextItem.Equals(NavigationItem.Empty))
//				{
//					IdeApp.Workbench.OpenDocument(nextItem.FileName, nextItem.Line, nextItem.Col);
//				}
//				else if (searchTarget is IVariable || searchTarget is IEntity)
//				{
//					FindReferencesForNavigation(searchTarget, false);
//				}
//			}
//		}
//
//		private static void FindReferencesForNavigation(object searchTarget, bool findNext)
//		{
//			var navLoop = ReferenceNavigationLoop.New(searchTarget);
//			var monitor = IdeApp.Workbench.ProgressMonitors.GetSearchProgressMonitor(true, false);
//			var solution = IdeApp.ProjectOperations.CurrentSelectedSolution;
//
//			// find out where we are so that we can make an educated guess for which result to navigate to next
//			var currentCaret = NavigationTools.GetCaretInActiveDocument();
//			var fileName = IdeApp.Workbench.ActiveDocument.FileName;
//
//			ThreadPool.QueueUserWorkItem(delegate {
//				try 
//				{
//					bool droppedMarker = false;
//
//					foreach (var mref in ReferenceFinder.FindReferences(solution, searchTarget, true, ReferenceFinder.RefactoryScope.Unknown, monitor)) 
//					{
//						var navItem = new NavigationItem(mref.FileName, mref.Region.BeginLine, mref.Region.BeginColumn);
//						navLoop.Items.Add(navItem);
//
//						if (mref.FileName == fileName)
//						{
//							// TODO: need to check column as well as line number
//							bool candidateForNavigation = findNext ? currentCaret.Line < mref.Region.BeginLine : currentCaret.Line > mref.Region.BeginLine;
//
//							if (!droppedMarker && candidateForNavigation)
//							{
//								droppedMarker = true;
//
//								DispatchService.GuiDispatch(() => {
//									NavigationTools.DropMarkerAtCaret();
//									IdeApp.Workbench.OpenDocument(mref.FileName, mref.Region.BeginLine, mref.Region.BeginColumn);
//									navLoop.MakeCurrent(navItem);
//								});
//							}
//						}
//
//						monitor.ReportResult(mref);
//					}
//
//					if (!droppedMarker && navLoop.Items.Count > 0)
//					{
//						var navItem = findNext ? navLoop.Next() : navLoop.Prev();
//
//						DispatchService.GuiDispatch(() => {
//							NavigationTools.DropMarkerAtCaret();
//							IdeApp.Workbench.OpenDocument(navItem.FileName, navItem.Line, navItem.Col);
//						});
//					}
//				} 
//				catch(Exception ex) 
//				{
//					if (monitor != null)
//					{
//						monitor.ReportError("Error finding references", ex);
//					}
//					else
//					{
//						LoggingService.LogError("Error finding references", ex);
//					}
//				} 
//				finally 
//				{
//					if (monitor != null)
//					{
//						monitor.Dispose();
//					}
//				}
//			});
//		}
	}
}

