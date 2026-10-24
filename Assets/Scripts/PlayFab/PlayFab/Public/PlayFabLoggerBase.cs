using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using UnityEngine;

namespace PlayFab.Public
{
	public abstract class PlayFabLoggerBase : IPlayFabLogger
	{
		[CompilerGenerated]
		private sealed class _003CRegisterLogger_003Ed__23 : IEnumerator<object>, IEnumerator, IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public PlayFabLoggerBase _003C_003E4__this;

			object IEnumerator<object>.Current
			{
				[DebuggerHidden]
				get
				{
					return null;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return null;
				}
			}

			[DebuggerHidden]
			public _003CRegisterLogger_003Ed__23(int _003C_003E1__state)
			{
			}

			[DebuggerHidden]
			void IDisposable.Dispose()
			{
			}

			private bool MoveNext()
			{
				return false;
			}

			bool IEnumerator.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				return this.MoveNext();
			}

			[DebuggerHidden]
			void IEnumerator.Reset()
			{
			}
		}

		private static readonly StringBuilder Sb;

		private readonly Queue<string> LogMessageQueue;

		private const int LOG_CACHE_INTERVAL_MS = 10000;

		private Thread _writeLogThread;

		private readonly object _threadLock;

		private static readonly TimeSpan _threadKillTimeout;

		private DateTime _threadKillTime;

		private bool _isApplicationPlaying;

		private int _pendingLogsCount;

		public IPAddress ip { get; set; }

		public int port { get; set; }

		public string url { get; set; }

		public virtual void OnEnable()
		{
		}

		[IteratorStateMachine(typeof(_003CRegisterLogger_003Ed__23))]
		private IEnumerator RegisterLogger()
		{
			return null;
		}

		public virtual void OnDisable()
		{
		}

		public virtual void OnDestroy()
		{
		}

		protected abstract void BeginUploadLog();

		protected abstract void UploadLog(string message);

		protected abstract void EndUploadLog();

		private void HandleUnityLog(string message, string stacktrace, LogType type)
		{
		}

		private void ActivateThreadWorker()
		{
		}

		private void WriteLogThreadWorker()
		{
		}
	}
}
