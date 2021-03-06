﻿//   --------------------------------------------------------------------------------------------------------------------
//   <copyright file="GotoDeclarationCommand.cs" company="(c) Greg Munn">
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Core;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using MonoDevelop.Projects;
using MonoDevelop.Ide.TypeSystem;
using MonoDevelop.Refactoring;

namespace BananaTools.Navigation.Commands
{
	/// <summary>
	/// Searches for the declaration of a type in all open solutions
	/// </summary>
	public sealed class GotoDeclarationCommandHandler : CommandHandler
	{
		protected override void Run (object dataItem)
		{
			var doc = IdeApp.Workbench.ActiveDocument;
			if (doc == null || doc.FileName == FilePath.Null)
				return;

			NavigationMarkers.DropMarkerAtCaret ();

			ResolveResult resolveResoult;
			object item = CurrentRefactoryOperationsHandler.GetItem (doc, out resolveResoult);
			var entity = item as INamedElement;
			if (entity != null) {
				// determine the list of solutions and projects outside of the background thread
				var solutionsToSearch = GetSolutionsToSearch (entity);
				var solutions = GetSolutionProjects (solutionsToSearch.ToList ());
				var conf = IdeApp.Workspace.ActiveConfiguration;

				Task.Factory.StartNew (() => {
					entity = CheckIfDefinedInOtherOpenSolution (entity, conf,  solutions);
					Xwt.Application.Invoke (() => IdeApp.ProjectOperations.JumpToDeclaration (entity));
				});
			} else {
				var v = item as IVariable;
				if (v != null)
					IdeApp.ProjectOperations.JumpToDeclaration (v);
			}
		}

		/// <summary>
		/// Determines if the given entity has been resolved to a source file
		/// </summary>
		static bool EntityResolvesToSourceFile (INamedElement entity) 
		{
			var ex = entity as IEntity;
			return ex != null && !ex.Region.IsEmpty;
		}

		/// <summary>
		/// Gets an array of opened solutions to search, except for the current active solution.
		/// Returns an empty set if the entity already resolves to a source file.
		/// </summary>
		static Solution[] GetSolutionsToSearch (INamedElement entity)
		{
			if (EntityResolvesToSourceFile (entity))
				return new Solution[0];

			// build up an array of solutions to search in case the entity is not in the current solution
			if (IdeApp.Workspace != null) {
				// do we have workspace? check the workspace for solution items
				var currentSolution = IdeApp.ProjectOperations.CurrentSelectedSolution;

				var workspace = IdeApp.Workspace.Items.OfType<Workspace> ().FirstOrDefault ();
				if (workspace != null)
					return workspace.Items.OfType<Solution> ().Where(s => s != currentSolution).ToArray ();
				else
					return IdeApp.Workspace.Items.OfType<Solution> ().Where(s => s != currentSolution).ToArray ();
			}

			return new Solution[0];
		}

		static Dictionary<Solution, ReadOnlyCollection<Project>> GetSolutionProjects (List<Solution> solutions) 
		{
			var dict = new Dictionary<Solution, ReadOnlyCollection<Project>> ();
			foreach (var solution in solutions) {
				dict.Add (solution, solution.GetAllProjects ());
			}
			return dict;
		}

		/// <summary>
		/// Gets a set of assemblies from the given solutions that match the assembly name. These assemblies are possible candidate assembles
		/// that the type we are looking for could be defined in.
		/// </summary>
		static HashSet<IAssembly> GetCandidateAssemblies (ConfigurationSelector conf, Dictionary<Solution, ReadOnlyCollection<Project>> solutionsToSearch, string assemblyName)
		{
			var assemblies = new HashSet<IAssembly> ();

			foreach (var solution in solutionsToSearch) {
				foreach (var project in solution.Value) {

					var outputFiles = project.GetOutputFiles (conf);

					if (outputFiles.Any (f => ((string)f.FullPath).Equals (assemblyName, System.StringComparison.OrdinalIgnoreCase))) {
						var comp = TypeSystemService.GetCompilation (project);
						if (comp == null)
							continue;

						assemblies.Add (comp.MainAssembly);
					}
				}
			}

			return assemblies;
		}

		/// <summary>
		/// Checks if the INamedElement is actually defined in another open solution and returns a INamedElement from that solution instead.
		/// Provides support for going to the source code definition of an element instead of the Assembly Browser.
		/// Returns the original entity if no suitable open solution was found
		/// </summary>
		static INamedElement CheckIfDefinedInOtherOpenSolution (INamedElement entity, ConfigurationSelector conf, Dictionary<Solution, ReadOnlyCollection<Project>> solutionsToSearch)
		{
			var ex = entity as IEntity;

			if (ex != null) {
				// if there are solutions to search it means that the entity we were looking for was not resolved to a source file
				// we will look in other open solutions
				var monitor = IdeApp.Workbench.ProgressMonitors.GetSearchProgressMonitor (true, true);
				using (monitor) {
					monitor.BeginTask (GettextCatalog.GetString ("Searching ..."), 1); 

					var assemblies = GetCandidateAssemblies (conf, solutionsToSearch, ex.ParentAssembly.UnresolvedAssembly.Location);

					foreach (var assembly in assemblies) {
						if (ex.DeclaringTypeDefinition != null) {
							var foundType = assembly.GetTypeDefinition (ex.DeclaringTypeDefinition.FullTypeName);
							if (foundType != null) {
								var foundMember = foundType.Members.FirstOrDefault (x => x.ReflectionName == entity.ReflectionName);

								if (foundMember != null) {
									return foundMember;
								}

								return foundType;
							}
						} else {
							var typeDef = ex as ITypeDefinition;
							if (typeDef != null) {
								var foundType = assembly.GetTypeDefinition (typeDef.FullTypeName);
								if (foundType != null)
									return foundType;
							}
						}
					}

					monitor.EndTask ();
				}
			}

			return entity;
		}
	}
}