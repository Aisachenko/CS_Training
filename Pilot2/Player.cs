namespace Task1;

public class Player
{
    public string Name;
    public int Score; //максимальное количество слов в победной игре

    public Player(string name, int score = 0)
    {
        Name = name;
        Score = score;
    }

    public Player()
    {
        Name = "";
        Score = 0;
    }
}