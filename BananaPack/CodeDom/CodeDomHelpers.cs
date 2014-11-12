//   --------------------------------------------------------------------------------------------------------------------
//   <copyright file="CodeDomHelpers.cs" company="(c) Greg Munn">
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
using System.Linq;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using MonoDevelop.Ide.Gui;
using MonoDevelop.Ide.Gui.Content;

namespace BananaTools.CodeDom
{
	public static class CodeDomHelpers
	{
		public static ResolveResult GetResolveResultAtCaret(Document document)
		{
			var textEditorResolver = document.GetContent<ITextEditorResolver>();
			if (textEditorResolver != null)
			{
				return textEditorResolver.GetLanguageItem(document.Editor.Caret.Offset);
			}

			return null;
		}

		public static object GetEntityAtCaret(Document document, bool handleConstructor)
		{
			var resolveResult = CodeDomHelpers.GetResolveResultAtCaret(document);
			if (resolveResult == null)
			{
				return null;
			}

			if (resolveResult is LocalResolveResult) 
			{
				return ((LocalResolveResult)resolveResult).Variable;
			}

			if (resolveResult is MemberResolveResult)
			{
				var x = ((MemberResolveResult)resolveResult);

				if (x.Member is IMethod)
				{
					// TODO: this is needed when we are in a loop that matches the type that we are looking for
					if (((IMethod)x.Member).IsConstructor && handleConstructor)
					{
						return x.Type;
					}
				}

				return x.Member;
			}

			if (resolveResult is MethodGroupResolveResult) 
			{
				var mg = ((MethodGroupResolveResult)resolveResult);
				var method = mg.Methods.FirstOrDefault();

				if (method == null && mg.GetExtensionMethods().Any()) 
				{
					method = mg.GetExtensionMethods().First().FirstOrDefault ();
				}

				return method;
			}

			if (resolveResult is TypeResolveResult)
			{
				return resolveResult.Type;
			}

			if (resolveResult is NamespaceResolveResult)
			{
				return ((NamespaceResolveResult)resolveResult).Namespace;
			}                    

			return null;
		}

		public static bool AreEqualSearchTargets(IVariable t1, IVariable t2)
		{
			if (t1 != null && t2 != null)
			{
				return t1.Name == t2.Name && t1.Type == t2.Type;
			}

			return false;
		}

		public static bool AreEqualSearchTargets(IEntity t1, IEntity t2)
		{
			if (t1 != null && t2 != null)
			{
				return t1 == t2;
			}

			return false;
		}

		public static bool AreEqualSearchTargets(object t1, object t2)
		{
			return AreEqualSearchTargets(t1 as IVariable, t2 as IVariable) || AreEqualSearchTargets(t1 as IEntity, t2 as IEntity);
		}
	}
}

