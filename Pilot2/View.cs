namespace Task1
{

    public class View
    {
        public void DisplayMessage(string message, params object[] args)
        {
            Console.WriteLine(message, args);
        }

        public string GetInput(out bool isCommand)
        {
            string input = Console.ReadLine().ToLower();
            isCommand = input.StartsWith("/");
            return input;
        }
        
        public (bool, string) GetTimedInput(out bool isCommand)
        {
            string input = "";
            Thread inputThread = new Thread(() =>
            {
                input = Console.ReadLine().ToLower();
            });
            inputThread.Start();
            
            isCommand = input.StartsWith("/");
            if (isCommand)
            {
                inputThread.Interrupt();
                return (false, input);
            }

            if (inputThread.Join(10000))
                return (false, input);
            
            inputThread.Interrupt();
            return (true, input);
        }
    }
}