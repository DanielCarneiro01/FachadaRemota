//
// This file exposes the interfaces for remoted objects
// Remoting clients see only these interfaces so there is no need to copy the implementation 
// assembly to the client machine.
//

namespace CalculatorInterface
{
    public interface ICalculator
	{
		int Add (int a, int b);
	}
}
