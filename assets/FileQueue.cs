using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System;

namespace ChickenIngot.Utility
{
	public enum FileOption
	{
		Read,
		Write,
		RemoveFile,
		RemoveDirectory
	}

	public enum FileError
	{
		Success,
		FileNotFound,
		DirectoryNotFound,
		WriteError,
		ReadError,
		RemoveError
	}

	public class FileTask
	{
		public string filePath;
		public byte[] data;
		public FileOption option;
		public FileError error;
		public bool done;

		public FileTask(string filePath, byte[] data, FileOption option)
		{
			this.filePath = filePath;
			this.data = data;
			this.option = option;
		}

		public FileTask(string filePath, FileOption option)
		{
			this.filePath = filePath;
			this.option = option;
		}

		public override string ToString()
		{
			string message = "";

			switch (option)
			{
				case FileOption.Read:
					message = "[Read] ";
					break;

				case FileOption.Write:
					message = "[Write] ";
					break;
			}

			switch (error)
			{
				case FileError.Success:
					message += "Success : " + filePath;
					break;

				case FileError.FileNotFound:
					message += "FileNotFound : " + filePath;
					break;

				case FileError.ReadError:
					message += "ReadError : " + filePath;
					break;

				case FileError.WriteError:
					message += "WriteError : " + filePath;
					break;
			}

			return message;
		}

		public static bool CheckAllDone(FileTask[] ioStates)
		{
			for (int i = 0; i < ioStates.Length; ++i)
			{
				if (!ioStates[i].done) return false;
			}

			return true;
		}
	}

	/// <summary>
	/// 파일 입출력을 수행하고 다양한 정보를 담은 구조체를 반환한다. 특히 비동기 입출력시 호출 순서대로 수행될 수 있도록 해 준다.
	/// 예컨대, 동일한 디렉토리에 접근하는 연산과 제거하는 연산이 서로 다른 곳에서 동시에 호출되는 것과 같은 동기화 문제를 예방할 수 있다.
	/// </summary>
	public static class FileQueue
	{
		private static bool _processOn = true;
		private static Thread _thread = new Thread(Process);
		private static ManualResetEvent _manualEvent = new ManualResetEvent(false);
		private static Queue<FileTask> _queue = new Queue<FileTask>();

		public static int Count { get { return _queue.Count; } }

		static FileQueue()
		{
			// 게임이 종료돼도 작업이 끝날 때까지 스레드를 종료하지 않는다.
			_thread.IsBackground = false;
			_thread.Start();
		}

		private static void Process()
		{
			while (_processOn)
			{
				_manualEvent.WaitOne();

				lock (_queue)
				{
					while (_queue.Count > 0)
					{
						FileTask task = _queue.Dequeue();

						switch (task.option)
						{
							case FileOption.Read:
								Read(task);
								break;

							case FileOption.Write:
								Write(task);
								break;

							case FileOption.RemoveFile:
								RemoveFile(task);
								break;

							case FileOption.RemoveDirectory:
								RemoveDirectory(task);
								break;
						}
					}

					_manualEvent.Reset();
				}
			}
		}

		private static void Read(FileTask task)
		{
			try
			{
				task.data = File.ReadAllBytes(task.filePath);
			}
			catch (FileNotFoundException)
			{
				task.error = FileError.FileNotFound;
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				task.error = FileError.ReadError;
			}

			task.done = true;
		}

		private static void Write(FileTask task)
		{
			try
			{
				string directory = Path.GetDirectoryName(task.filePath);
				DirectoryInfo di = new DirectoryInfo(directory);
				if (!di.Exists) di.Create();

				File.WriteAllBytes(task.filePath, task.data);
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				task.error = FileError.WriteError;
			}

			task.done = true;
		}

		private static void RemoveFile(FileTask task)
		{
			try
			{
				File.Delete(task.filePath);
			}
			catch (DirectoryNotFoundException)
			{
				task.error = FileError.DirectoryNotFound;
			}
			catch (FileNotFoundException)
			{
				task.error = FileError.FileNotFound;
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				task.error = FileError.RemoveError;
			}

			task.done = true;
		}

		private static void RemoveDirectory(FileTask task)
		{
			try
			{
				Directory.Delete(task.filePath, true);
			}
			catch (DirectoryNotFoundException)
			{
				task.error = FileError.DirectoryNotFound;
			}
			catch (Exception e)
			{
				Debug.LogError(e);
				task.error = FileError.RemoveError;
			}

			task.done = true;
		}

		public static void Stop()
		{
			_processOn = false;
		}

		public static FileTask EnqueueWrite(string filePath, byte[] data)
		{
			lock (_queue)
			{
				FileTask task = new FileTask(filePath, data, FileOption.Write);
				_queue.Enqueue(task);
				_manualEvent.Set();

				return task;
			}
		}

		public static FileTask EnqueueRead(string filePath)
		{
			lock (_queue)
			{
				FileTask task = new FileTask(filePath, FileOption.Read);
				_queue.Enqueue(task);
				_manualEvent.Set();

				return task;
			}
		}

		public static FileTask EnqueueRemoveFile(string filePath)
		{
			lock (_queue)
			{
				FileTask task = new FileTask(filePath, FileOption.RemoveFile);
				_queue.Enqueue(task);
				_manualEvent.Set();

				return task;
			}
		}

		public static FileTask EnqueueRemoveDirectory(string directory)
		{
			lock (_queue)
			{
				FileTask task = new FileTask(directory, FileOption.RemoveDirectory);
				_queue.Enqueue(task);
				_manualEvent.Set();

				return task;
			}
		}
	}
}