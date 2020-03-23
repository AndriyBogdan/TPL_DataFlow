using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
	public interface IParallelData
	{
		public int Hash { get; }
		public Type CurrentType { get; }
		public string Data { get; set; }
	}
}
