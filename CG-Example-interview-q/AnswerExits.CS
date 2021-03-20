using System;
using System.Linq;
					
public class Answer
{
	public static bool Exists(int[] ints, int k)
	{
		if (ints.Contains(k))return true;
		else return false;
	}

}

public class Program
{
	public static void Main()
	{
		int[] ints = {-9,14,37,102};
		Console.WriteLine(Answer.Exists(ints,102));
		Console.WriteLine(Answer.Exists(ints,36));
	}
}