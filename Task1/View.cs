namespace Task1
{

    public class View
    {
        public void DisplayMessage(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public string GetInput()
        {
            return Console.ReadLine().ToLower();
        }
    }
}