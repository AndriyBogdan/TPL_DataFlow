using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GermesBot.Adapter.Parallel
{
	public class DataIn<T> : IParallelData
	{
		public int Hash => this.GetHashCode();
		public Type CurrentType => this.GetType();
		public object Data { get; set; }

		public static implicit operator T(DataIn<T> d) => (T)d.Data;
		public static explicit operator DataIn<T>(T data) => new DataIn<T>(data);

		public DataIn(T tData)
		{
			Data = tData;
		}
		public DataIn()
		{
			
		}
	}
}
