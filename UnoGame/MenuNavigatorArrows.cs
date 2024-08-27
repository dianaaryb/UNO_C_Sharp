namespace UnoGame;

public class MenuNavigatorArrows
{
        public int CurrentIndexArrows = 0;
        public readonly List<string> Options;
        public readonly List<string> Commands;
        public int StartLine;

        public MenuNavigatorArrows(List<string> options, List<string> commands, int startLine)
        {
            Options = options;
            Commands = commands;
            StartLine = startLine;
        }

        public string NavigateAndSelect()
        {
            Console.CursorVisible = false;
            DisplayMenu();
            ConsoleKey key;
            do
            {
                key = Console.ReadKey(true).Key;
                HandleKeyPress(key);
            } while (key != ConsoleKey.Enter);

            return Commands[CurrentIndexArrows];
        }

        public void DisplayMenu()
        {
            Console.SetCursorPosition(0, StartLine);
            for (int i = 0; i < Options.Count; i++)
            {
                if (i == CurrentIndexArrows)
                {
                    Console.Write("\x1B[4m");
                    Console.WriteLine(Options[i]);
                    Console.Write("\x1B[0m");
                    // Console.BackgroundColor = ConsoleColor.Blue; 
                    // Console.WriteLine(_options[i]);
                    // Console.ResetColor();
                }
                else
                {
                    Console.WriteLine(Options[i]);
                }
            }
        }

        public void HandleKeyPress(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    CurrentIndexArrows = (CurrentIndexArrows - 1 + Options.Count) % Options.Count;
                    DisplayMenu();
                    break;
                case ConsoleKey.DownArrow:
                    CurrentIndexArrows = (CurrentIndexArrows + 1) % Options.Count;
                    DisplayMenu();
                    break;
            }
        }
    }