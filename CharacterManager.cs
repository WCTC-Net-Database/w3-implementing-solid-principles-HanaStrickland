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
        CharacterReader characterReader = new CharacterReader();
        characterReader.CharacterLines = lines;
        characterReader.LoadCharacters(Characters);

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
                    CharacterWriter characterWriter = new CharacterWriter();
                    characterWriter.CharacterWriterList = Characters;
                    characterWriter.WriteOutCharacters();
                    return;
                default:
                    _output.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    public void DisplayCharacters()
    {
        foreach (var character in Characters)
            {
                _output.WriteLine($"Name: {character.CharacterName}; Class: {character.CharacterClass}; Level: {character.CharacterLevel}; Hit Points: {character.CharacterHitPoints};  Equipment: {string.Join(", ", character.CharacterEquipment)}");
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
        _output.WriteLine("Select 3 tools from the menu below: ");

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

        _output.WriteLine($"You've chosen the following equipment: {choicesString}");

        // Append the new character to the lines array

        string pipeDelimitedChoicesString = string.Join("|", choicesArray);
        string lineToAppend = $"{newCharacter},{newClass},{1},{10},{pipeDelimitedChoicesString}"; // automatically level 1 and 10 hit points
        _output.WriteLine(lineToAppend);
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
        _output.WriteLine("Select the character to level up: ");

        CharacterNamesMenu characterNamesMenu = new CharacterNamesMenu();
        characterNamesMenu.GetMenutOfCharacters(Characters);

        List<string> characterNamesList = characterNamesMenu.CharacterNamesList;
        
        Console.Write("Enter Your Choice: ");
        int indexOfNameToLevelUp = Convert.ToInt16(Console.ReadLine()) - 1;
        
        string nameToLevelUp = characterNamesList[indexOfNameToLevelUp];
        
        _output.WriteLine($"You've Chosen to Level Up {nameToLevelUp}");

        var foundCharacter = Characters.Where(c => c.CharacterName == nameToLevelUp).FirstOrDefault();

        foundCharacter.CharacterLevel++;
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

public class CharacterReader
{
    public string[] CharacterLines {get;set;}

    public string GetName(string line)
    {
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
            name = name.Replace("\"","");
        }
        else
        {
            // TODO: Name is not quoted, so store the name up to the first comma
            commaIndex = line.IndexOf(',');
            name = line.Substring(0,commaIndex);
        }
        return name;
    }
    
        public (string, int, int, string[]) GetCharacterTraits(string line)
    {   
        string[] fields = line.Split(",");

        string heroClass = fields[^4];
        int level = Convert.ToInt16(fields[^3]);
        int hitPoints = Convert.ToInt16(fields[^2]);
        string heroEquipmentString = fields[^1];
        
        // TODO: Parse equipment noting that it contains multiple items separated by '|'
        string[] heroEquipmentArray = heroEquipmentString.Split('|');

        return (heroClass, level, hitPoints, heroEquipmentArray);

    }

    public void LoadCharacters(List<Character> Characters)
    {
        for (int i = 1; i < CharacterLines.Length; i++)
        {
            string line = CharacterLines[i];

            string characterName = GetName(line);

            var (characterClass, characterLevel, characterHitPoints, characterEquipment) = GetCharacterTraits(line);

            Characters.Add(new Character()
            {
                CharacterName = characterName,
                CharacterClass = characterClass,
                CharacterLevel = characterLevel,
                CharacterHitPoints = characterHitPoints,
                CharacterEquipment = characterEquipment
            });
        }

    }
}

public class CharacterWriter
{
    public List<Character> CharacterWriterList {get;set;}
    private List<string> OutputList = new List<string>();

    public void WriteOutCharacters()
    {
        foreach(var character in CharacterWriterList)
        {
            string nameString;

            if (character.CharacterName.Contains(","))
            {
                nameString = $"\"{character.CharacterName}\"";
            }
            else
            {
                nameString = character.CharacterName;
            }

            string pipeDelimitedChoicesString = string.Join("|", character.CharacterEquipment);

            string lineToAdd = $"{nameString},{character.CharacterClass},{character.CharacterLevel},{character.CharacterHitPoints},{pipeDelimitedChoicesString}";
            
            OutputList.Add(lineToAdd);
        }
        using (StreamWriter outputFile = new StreamWriter("WriteLines.txt"))
            {
                foreach (string line in OutputList)
                    outputFile.WriteLine(line);
            }
    }
}

public class CharacterNamesMenu
{
    public List<string> CharacterNamesList {get;set;}

    public void GetMenutOfCharacters(List<Character> Characters)
    {
        CharacterNamesList = new List<string>();

        foreach (var character in Characters)

        {
            CharacterNamesList.Add(character.CharacterName);
        }

        for (int i = 0; i < CharacterNamesList.Count; i++)
        {
            Console.WriteLine($"{i+1}: {CharacterNamesList[i]}");
        }
    }
}