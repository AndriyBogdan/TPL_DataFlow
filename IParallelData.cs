using System;
using System.Collections.Generic;
using System.Text;

namespace GermesBot.Adapter.Parallel
{
	public interface IParallelData
	{
		int Hash { get; }
		Type CurrentType { get; }
		object Data { get; set; }
	}
}
