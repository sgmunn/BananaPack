//   --------------------------------------------------------------------------------------------------------------------
//   <copyright file="DropNavigationMarkerCommandHandler.cs" company="(c) Greg Munn">
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
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using Mono.TextEditor;

namespace BananaTools.Navigation.Commands
{
	/// <summary>
	/// Drops a navigation marker at the location of the caret in the active document.
	/// </summary>
	public sealed class DropNavigationMarkerCommandHandler : CommandHandler
	{
		protected override void Run()
		{
			NavigationMarkers.DropMarkerAtCaret();
		}

		protected override void Update(CommandInfo info)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			info.Enabled = doc != null && doc.GetContent<ITextEditorDataProvider>() != null;
		}
	}
}