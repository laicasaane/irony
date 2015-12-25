#region License

/* **********************************************************************************
 * Copyright (c) Roman Ivantsov
 * This source code is subject to terms and conditions of the MIT License
 * for Irony. A copy of the license can be found in the License.txt file
 * at the root of this distribution.
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/

#endregion License

using System;
using System.Reflection;
using System.Windows.Forms;
using Irony.Parsing;

namespace Irony.GrammarExplorer
{
	public partial class fmSelectGrammars : Form
	{
		public fmSelectGrammars()
		{
			this.InitializeComponent();
		}

		public static GrammarItemList SelectGrammars(string assemblyPath, GrammarItemList loadedGrammars)
		{
			var fromGrammars = LoadGrammars(assemblyPath);
			if (fromGrammars == null)
				return null;

			// Fill the listbox and show the form
			var form = new fmSelectGrammars();
			var listbox = form.lstGrammars;
			listbox.Sorted = false;

			foreach (GrammarItem item in fromGrammars)
			{
				listbox.Items.Add(item);
				if (!ContainsGrammar(loadedGrammars, item))
					listbox.SetItemChecked(listbox.Items.Count - 1, true);
			}

			listbox.Sorted = true;

			if (form.ShowDialog() != DialogResult.OK)
				return null;

			var result = new GrammarItemList();
			for (int i = 0; i < listbox.Items.Count; i++)
			{
				if (listbox.GetItemChecked(i))
				{
					var item = listbox.Items[i] as GrammarItem;
					item.loading = false;
					result.Add(item);
				}
			}

			return result;
		}

		private static bool ContainsGrammar(GrammarItemList items, GrammarItem item)
		{
			foreach (var listItem in items)
			{
				if (listItem.TypeName == item.TypeName && listItem.Location == item.Location)
					return true;
			}

			return false;
		}

		private static GrammarItemList LoadGrammars(string assemblyPath)
		{
			Assembly asm = null;

			try
			{
				asm = GrammarLoader.LoadAssembly(assemblyPath);
			}
			catch (Exception ex)
			{
				MessageBox.Show("Failed to load assembly: " + ex.Message);
				return null;
			}

			var types = asm.GetTypes();
			var grammars = new GrammarItemList();

			foreach (Type t in types)
			{
				if (t.IsAbstract)
					continue;

				if (!t.IsSubclassOf(typeof(Grammar)))
					continue;

				grammars.Add(new GrammarItem(t, assemblyPath));
			}

			if (grammars.Count == 0)
			{
				MessageBox.Show("No classes derived from Irony.Grammar were found in the assembly.");
				return null;
			}

			return grammars;
		}

		private void btnCheckUncheck_Click(object sender, EventArgs e)
		{
			var check = sender == btnCheckAll;

			for (int i = 0; i < lstGrammars.Items.Count; i++)
			{
				lstGrammars.SetItemChecked(i, check);
			}
		}
	}
}
