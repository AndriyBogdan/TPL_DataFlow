using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GermesBot.Adapter.Parallel
{
	class ParallelWorkerWrapper<T_in, T_out>
	{
		/// <summary>
		/// Instance of class that represent parallel logic
		/// </summary>
		ParallelWorker pWorker;
		/// <summary>
		/// Thread-safe collection for result data with parallel-type
		/// </summary>
		BlockingCollection<IParallelData> resultContainer;
		/// <summary>
		/// Collection for result data with concrete type
		/// </summary>
		List<T_out> unwrappedItemsCollection;

		Func<T_in, T_out> MethodWithInOutData;

		MethodInfo mInfo;
		object dynamicInstance;
		bool setCallbackState = false;
		bool receivedState = false;

		#region Constructors
		public ParallelWorkerWrapper()
		{

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="numberOfThreads">Set the number of parallel threads for processing data</param>
		private ParallelWorkerWrapper(int numberOfThreads)
		{
			pWorker = new ParallelWorker(TransformMethod, ActionMethod);
			pWorker.DataWasReceived += PWorker_DataWasReceived;
			resultContainer = new BlockingCollection<IParallelData>();

			pWorker.Init(numberOfThreads);
		}
		public ParallelWorkerWrapper(int numberOfThreads, Func<T_in, T_out> transformBlockWorker) : this(numberOfThreads)
		{
			this.MethodWithInOutData = transformBlockWorker;
		}

		#endregion

		public List<T_out> Run(List<T_in> items)
		{
			var wrappedItemsCollection = new List<DataIn<T_in>>();
			foreach (var item in items)
			{
				wrappedItemsCollection.Add(new DataIn<T_in>(item));
			}

			IEnumerable<IParallelData> data = wrappedItemsCollection;
			pWorker.Run(data);

			WaitForReceived();

			return unwrappedItemsCollection;
		}
		private void WaitForReceived()
		{
			while (true)
			{
				if (receivedState) return;
				else
				{
					Task.Delay(100);
				}
			}
		}
		private void PWorker_DataWasReceived()
		{
			unwrappedItemsCollection = new List<T_out>();
			var res = resultContainer.ToArray();

			for (int i = 0; i < res.Length; i++)
			{
				unwrappedItemsCollection.Add((T_out)res[i].Data);
			}

			//if you want to call some method from another instance you must to implement IParallelResponse interface in it
			//because InvokeCallback call instance method which declared in IParallelResponse interface

			//InvokeCallback(unwrappedItemsCollection);
		}
		private IParallelData TransformMethod(IParallelData data)
		{
			IParallelData transformedData = new DataOut<T_out>();

			transformedData.Data = this.MethodWithInOutData.Invoke((T_in)data.Data);

			return transformedData;
		}
		private void ActionMethod(IParallelData data)
		{
			resultContainer.Add(data);
		}
		public void SetDynamicCallback(ref object instance, string methodName, Type returnValueType)
		{
			if (instance == null) return;	

			Type tInstance = instance.GetType();
			dynamicInstance = Convert.ChangeType(instance, tInstance);
			mInfo = tInstance.GetMethod(methodName, new Type[] { returnValueType });

			if (mInfo != null) setCallbackState = true;
		}
		public void InvokeCallback(object data)
		{
			if (!setCallbackState) return;
			mInfo.Invoke(dynamicInstance, new object[] { data });
		}
	}
}
