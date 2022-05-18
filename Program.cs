// See https://aka.ms/new-console-template for more information
int counter = 1;
while (true)
{
    double answerBytesLength = 1 + Math.Floor(Math.Log2(counter) / 8);
    counter += 1;
    Console.WriteLine(answerBytesLength);
}