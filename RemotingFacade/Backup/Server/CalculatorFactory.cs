using System;

using CalculatorInterface;
using RemotingFacade;

namespace Server
{
    public class CalculatorFactory : MarshalByRefObject, IRemotedFactory<ICalculator>
	{
        public CalculatorFactory()
		{
            Console.WriteLine("CalculatorFactory constructor called");
        }

        #region IRemotedFactory<ICalculator> Members

        public ICalculator Create()
        {
            return new Calculator();
        }

        #endregion
    }
}
