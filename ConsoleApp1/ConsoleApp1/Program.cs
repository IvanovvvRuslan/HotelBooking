using ConsoleApp1;


var counter = new Counter();
var lockObject = new object();

var thread = new Thread(DoSomeCalculations);
thread.Start();

for (var i = 0; i < 10_000; i++)
{
    lock (lockObject)
    {
        counter.Value++;
    }
}
Console.WriteLine("Counter: " + counter.Value);

void DoSomeCalculations()
{
    for (var i = 0; i < 10_000; i++)
    {
        lock (lockObject)
        {
            counter.Value++;
        }
    }
    Console.WriteLine("Counter: " + counter.Value);
}