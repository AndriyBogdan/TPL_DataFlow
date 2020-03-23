using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
	public sealed class ParallelWorker : AbstractParallelWorker<IParallelData, IParallelData>
	{
		public ParallelWorker() : base()
		{
			

		}
		public ParallelWorker(Func<IParallelData, IParallelData> TransformMethod, Action<IParallelData> ActionMethod) : base(TransformMethod, ActionMethod)
		{
			
		}
		public override void Init(int maxThreads) => base.Init(maxThreads);
		public override void Run(IEnumerable<IParallelData> dataCollection) => base.Run(dataCollection);
	}
}
