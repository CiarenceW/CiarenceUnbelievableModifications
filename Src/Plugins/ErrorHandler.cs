using System;
using System.Text;
using System.Threading.Tasks;

namespace CiarenceUnbelievableModifications
{
	public static class ErrorHandler
	{
		public static bool RessourcesFailed
		{
			get;
			internal set;
		}

		public delegate void voidDelegate();

		public static void TryExecute(voidDelegate test)
		{

		}

		public static bool TryExecute<T>(Func<T> test, out T result)
		{
			result = default;

			var ranFine = true;

			try
			{
				result = test.Invoke();
			}
			catch (Exception ex)
			{
				MainPlugin.Logger.LogError(ex);
				MainPlugin.Logger.LogMessage($"Failed to execute {test.Method.Name}");
				ranFine = false;
			}

			return ranFine;
		}
	}
}
