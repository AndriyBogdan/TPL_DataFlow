using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GermesBot.Adapter.Parallel
{
	public class DataOut<T> : IParallelData
	{
		public int Hash => this.GetHashCode();
		public Type CurrentType => this.GetType();
		public object Data { get; set; }

		public static implicit operator T(DataOut<T> d) => (T)d.Data;
		public static explicit operator DataOut<T>(T data) => new DataOut<T>(data);

		public DataOut(T tData)
		{
			Data = tData;
		}
		public DataOut()
		{

		}
	}
}
