using System;

using CalculatorInterface;

namespace Server
{
	public class Calculator: MarshalByRefObject, ICalculator
	{
		public Calculator()
		{
            Console.WriteLine("Calculator constructor called");
        }

		#region ICalculator Members

		public int Add(int a, int b)
		{
            Console.WriteLine("Calculator service: Add({0}, {1}).", a, b);
			return (a + b);
		}

		#endregion
	}
}
