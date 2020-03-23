using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace ConsoleApp1
{
	public class DataIn : IParallelData
	{
		public int Hash => this.GetHashCode();
		public Type CurrentType => this.GetType(); 
		public string Data { get; set; }
		public DataIn()
		{
			Data = "--------------------------------------------------------------------------------------------------------";
		}
		public void SomeMethod()
		{
			Console.WriteLine("from DataIn");
		}
	}
	public class DataOut : IParallelData
	{
		public int Hash => this.GetHashCode();

		public Type CurrentType => this.GetType();
		public string Data { get; set; }
		public DataOut(string tData)
		{
			Data = tData;
		}

		public void SomeMethod() 
		{
			Console.WriteLine("from DataOut");
		}
	}
	public class Worker
	{
		public static int counter = 0;	

		ParallelWorker pWorker;

		public List<IParallelData> parallelDatas;

		public Worker()
		{
			parallelDatas = new List<IParallelData>();
			
			pWorker = new ParallelWorker(TransformMethod, ActionMethod);
			pWorker.Init(10);

			List<IParallelData> dt = new List<IParallelData>();

			IParallelData tmpData = null;

			for (int i = 0; i < 20; i++)
			{
				tmpData = new DataIn();
				dt.Add(tmpData);
			}

			pWorker.Run(dt);
		}

		public IParallelData TransformMethod(IParallelData data)
		{
			IParallelData transformedData = null;
			string d = data.Data as String;
			string transformed = string.Empty;

			if (string.IsNullOrEmpty(d)) return null;

			foreach (var c in d)
			{
				transformed += c + counter;
			}

			transformedData = new DataOut(transformed);

			Interlocked.Increment(ref counter);

			return transformedData;
		}
		public void ActionMethod(IParallelData data)
		{
			parallelDatas.Add(data);
		}
	}
	class Program
	{
		static void Main(string[] args)
		{
			Worker worker = new Worker();

			foreach (var item in worker.parallelDatas)
			{
				Console.WriteLine(item);
			}

			Console.ReadKey();
		}
	}
}
