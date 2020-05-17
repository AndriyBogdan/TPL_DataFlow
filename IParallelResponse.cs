using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GermesBot.Adapter.Parallel
{
	interface IParallelResponse<T>
	{
		void SetResult(T data);
	}
}
