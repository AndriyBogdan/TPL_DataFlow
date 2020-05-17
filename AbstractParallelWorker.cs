using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks.Dataflow;

namespace GermesBot.Adapter.Parallel
{
	public abstract class AbstractParallelWorker<T_in, T_out>
	{
		public BufferBlock<T_in> bufferBlock;
		public TransformBlock<T_in, T_out> updateBlock;
		public ActionBlock<T_out> actionBlock;

		public ExecutionDataflowBlockOptions updateBlockOptions;
		public ExecutionDataflowBlockOptions actionBlockOptions;

		public Func<T_in, T_out> TransformMethod;
		public Action<T_out> ActionMethod;

		public event Action DataWasReceived;

		private bool initState;
		public AbstractParallelWorker()
		{
			initState = false;
		}

		public AbstractParallelWorker(Func<T_in, T_out> TransformMethod, Action<T_out> ActionMethod) : this()
		{
			this.TransformMethod = TransformMethod;
			this.ActionMethod = ActionMethod;
		}

		public virtual void Init(int maxThreads)
		{
			bufferBlock = new BufferBlock<T_in>();
			updateBlockOptions = new ExecutionDataflowBlockOptions
			{
				MaxDegreeOfParallelism = maxThreads,
				BoundedCapacity = 100
			};
			updateBlock = new TransformBlock<T_in, T_out>((dataIn) => TransformMethod(dataIn), updateBlockOptions);
			actionBlockOptions = new ExecutionDataflowBlockOptions
			{
				BoundedCapacity = 100,
				SingleProducerConstrained = true
			};
			actionBlock = new ActionBlock<T_out>(transformedData => ActionMethod(transformedData), actionBlockOptions);
			bufferBlock.LinkTo(updateBlock);
			bufferBlock.Completion.ContinueWith(task => updateBlock.Complete());
			updateBlock.LinkTo(actionBlock);
			updateBlock.Completion.ContinueWith(task => actionBlock.Complete());

			initState = true;
		}

		public virtual void Run(IEnumerable<T_in> dataCollection)
		{
			if(!initState) return;
			
			foreach (var item in dataCollection)
			{
				bufferBlock.Post(item);
			}

			bufferBlock.Complete();

			actionBlock.Completion.Wait();

			DataWasReceived?.Invoke();
		}
	}
}
