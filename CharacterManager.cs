namespace CharacterConsole;

public class CharacterManager
{
    private readonly IInput _input;
    private readonly IOutput _output;
    private readonly string _filePath = "input.csv";

    private string[] lines;

    public CharacterManager(IInput input, IOutput output)
    {
        _input = input;
        _output = output;
    }

    List<Character> Characters = new List<Character>();

    public void Run()
    {
        _output.WriteLine("Welcome to Character Management");

        lines = File.ReadAllLines(_filePath);

        while (true)
        {
            _output.WriteLine("Menu:");
            _output.WriteLine("1. Display Characters");
            _output.WriteLine("2. Add Character");
            _output.WriteLine("3. Level Up Character");
            _output.WriteLine("4. Exit");
            _output.Write("Enter your choice: ");
            var choice = _input.ReadLine();

            switch (choice)
            {
                case "1":
                    DisplayCharacters();
                    break;
                case "2":
                    AddCharacter();
                    break;
                case "3":
                    LevelUpCharacter();
                    break;
                case "4":
                    return;
                default:
                    _output.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    public void DisplayCharacters()
    {
        // Skip the header row
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];

            string name;
            int commaIndex;

            // Check if the name is quoted
            if (line.StartsWith("\""))
            {
                // TODO: Find the closing quote and the comma right after it
                
                commaIndex = line.IndexOf(',');
                name = line.Substring(0, commaIndex);
                int pos = name.Length + 1;

                var line2 = line.Substring(pos);

                int commaIndex2 = line2.IndexOf(',');

                int nameEndsIndex = pos + commaIndex2;

                // TODO: Remove quotes from the name if present and parse the name
                name = line.Substring(0,nameEndsIndex);
                line = line.Substring(nameEndsIndex);
                name = name.Replace("\"","");
            }
            else
            {
                // TODO: Name is not quoted, so store the name up to the first comma
                commaIndex = line.IndexOf(',');
                name = line.Substring(0,commaIndex);
                line = line.Substring(commaIndex);
            }

            // TODO: Parse characterClass, level, hitPoints, and equipment

            string[] otherFields = line.Split(','); 
               
            var heroClass = otherFields[1];
            var heroLevel = otherFields[2];
            var hitPoints = otherFields[3];
            
            // TODO: Parse equipment noting that it contains multiple items separated by '|'
            var heroEquipmentString = otherFields[4];                
            string[] heroEquipmentArray = heroEquipmentString.Split('|');

            // Display character information
            Console.WriteLine($"Name: {name}; Class: {heroClass}; Level: {heroLevel}; HP: {hitPoints}; Equipment: {string.Join(", ", heroEquipmentArray)}");

            Characters.Add(new Character()
            {
                CharacterName = name,
                CharacterClass = heroClass,
                CharacterLevel = Convert.ToInt16(heroLevel),
                CharacterHitPoints = Convert.ToInt16(hitPoints),
                CharacterEquipment = heroEquipmentArray
            });
        }
    }

    public void AddCharacter()
    {
        // TODO: Implement logic to add a new character
        
        // Prompt for character details (name, class, level, hit points, equipment)
        // DO NOT just ask the user to enter a new line of CSV data or enter the pipe-separated equipment string
        Console.Write("Enter the name for your new character: ");
        string newCharacter = Console.ReadLine();
        Console.Write($"Enter your character's class: ");
        string newClass = Console.ReadLine();
        Console.WriteLine("Select 3 tools from the menu below: ");

        string[] equipmentOptions = {"Armor","Book","Cloak","Dagger","Horse","Lockpick","Mace","Potion","Robe","Shield","Staff","Sword"};
        int countEquipChoicesLeft = 3;
        List<string> choicesList = new List<string>();

        while (countEquipChoicesLeft > 0)
        {
            
            Console.Write("1. Armor\n2. Book\n3. Cloak\n4. Dagger\n5. Horse\n6. Lockpick\n7. Mace\n8. Potion\n9. Robe\n10. Shield\n11. Staff\n12. Sword\n");

            int choice = Convert.ToInt16(Console.ReadLine());
            int mappedToIndexChoice = choice - 1;
            string choiceName = equipmentOptions[mappedToIndexChoice];

            choicesList.Add(choiceName);

            countEquipChoicesLeft -= 1;
        }

        string[] choicesArray = choicesList.ToArray();
        string choicesString = string.Join(", ", choicesArray);

        Console.WriteLine($"You've chosen the following equipment: {choicesString}");

        // Append the new character to the lines array
        if (newCharacter.Contains(","))
        {
            newCharacter = $"\"{newCharacter}\"";
        }

        string pipeDelimitedChoicesString = string.Join("|", choicesArray);
        string lineToAppend = $"{newCharacter},{newClass},{1},{10},{pipeDelimitedChoicesString}"; // automatically level 1 and 10 hit points
        Console.WriteLine(lineToAppend);
        lines = lines.Append(lineToAppend).ToArray();

        Characters.Add(new Character()
            {
                CharacterName = newCharacter,
                CharacterClass = newClass,
                CharacterLevel = 1,
                CharacterHitPoints = 10,
                CharacterEquipment = choicesArray
            });
    }

    public void LevelUpCharacter()
    {
        Console.Write("Enter the name of the character to level up: ");
        string nameToLevelUp = Console.ReadLine();

        // Loop through characters to find the one to level up
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];

            // TODO: Check if the name matches the one to level up
            // Do not worry about case sensitivity at this point
            if (line.Contains(nameToLevelUp))
            {

                // TODO: Split the rest of the fields locating the level field
                string[] fields = line.Split(",");

                string heroClass = fields[^4];
                int level = Convert.ToInt16(fields[^3]);
                int hitPoints = Convert.ToInt16(fields[^2]);
                string equipment = fields[^1];

                string name;
                int commaIndex;

                if (line.StartsWith("\""))
                {
                    // TODO: Find the closing quote and the comma right after it
                    
                    commaIndex = line.IndexOf(',');
                    name = line.Substring(0, commaIndex);
                    int pos = name.Length + 1;

                    var line2 = line.Substring(pos);

                    int commaIndex2 = line2.IndexOf(',');

                    int nameEndsIndex = pos + commaIndex2;

                    // TODO: Remove quotes from the name if present and parse the name
                    // name = ...
                    name = line.Substring(0,nameEndsIndex);
                    line = line.Substring(nameEndsIndex);
                    name = name.Replace("\"","");
                }
                else
                {
                    // TODO: Name is not quoted, so store the name up to the first comma
                    commaIndex = line.IndexOf(',');
                    name = line.Substring(0,commaIndex);
                    line = line.Substring(commaIndex);
                }

                // TODO: Level up the character
                level++;
                Console.WriteLine($"Character {name} leveled up to level {level}!");

                // TODO: Update the line with the new level
                if (name.Contains(","))
                {
                    name = $"\"{name}\"";
                }
                lines[i] = $"{name},{heroClass},{level},{hitPoints},{equipment}";

                break;
            }
        }
    }
}

public class Character
{
    public string CharacterName {get;set;}
    public string CharacterClass {get;set;}
    public int CharacterLevel {get;set;}
    public int CharacterHitPoints {get;set;}
    public string[] CharacterEquipment {get;set;}

}