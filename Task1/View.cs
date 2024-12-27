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
        
        public (bool, string) GetTimedInput()
        {
            string input = "";
            Thread inputThread = new Thread(() =>
            {
                input = Console.ReadLine().ToLower();
            });

            inputThread.Start();

            if (inputThread.Join(10000))
                return (false, input);
            
            inputThread.Interrupt();
            return (true, input);
        }
      
    }
}