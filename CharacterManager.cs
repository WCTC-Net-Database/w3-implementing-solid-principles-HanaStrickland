namespace CharacterConsole;

public class CharacterManager
{
    private readonly IInput _input;
    private readonly IOutput _output;
    private readonly string _filePath = "input.csv";

    private string[] lines;

    private CharacterReader characterReader;
    private List<Character> Characters;
    private EquipmentManager equipmentManager;
    private CharcaterClassManager characterClassManager;

    public CharacterManager(IInput input, IOutput output)
    {
        _input = input;
        _output = output;

        characterReader = new CharacterReader();
        Characters = characterReader.CharactersList ?? new List<Character>();
        equipmentManager = new EquipmentManager();
        characterClassManager = new CharcaterClassManager();
    }

    public void Run()
    {
        _output.WriteLine("Welcome to Character Management");

        lines = File.ReadAllLines(_filePath);
        characterReader.CharacterLines = lines;
        characterReader.LoadCharacters();

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
        characterReader.DisplayCharacters();
    }

    public void AddCharacter()
    {
        // TODO: Implement logic to add a new character
        
        // Prompt for character details (name, class, level, hit points, equipment)
        // DO NOT just ask the user to enter a new line of CSV data or enter the pipe-separated equipment string
        Console.Write("Enter the name for your new character: ");
        string newCharacter = Console.ReadLine();

        Console.WriteLine($"Pick you character's class from the menu below: ");
        characterClassManager.DisplayCharacterClassMenu();
        Console.Write("Enter Your Choice: ");
        int newClassIndex = Convert.ToInt16(Console.ReadLine()) - 1;

        string[] characterClassOptions = characterClassManager.CharacterClassOptions;

        string newClass = characterClassOptions[newClassIndex];
        
        _output.WriteLine("Select 3 tools from the menu below: ");

        string[] equipmentOptions = equipmentManager.EquipmentOptions;
        int countEquipChoicesLeft = 3;
        List<string> choicesList = new List<string>();

        while (countEquipChoicesLeft > 0)
        {
            equipmentManager.DisplayEquipmentMenu();
            
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

        characterReader.AddCharacter(newCharacter, newClass, choicesArray);
    }

    public void LevelUpCharacter()
    {
        _output.WriteLine("Select the character to level up: ");

        characterReader.DisplayCharacterNamesMenu();
        List<string> characterNamesList = characterReader.CharacterNamesList;
        
        Console.Write("Enter Your Choice: ");
        int indexOfNameToLevelUp = Convert.ToInt16(Console.ReadLine()) - 1;
        
        string nameToLevelUp = characterNamesList[indexOfNameToLevelUp];
        
        _output.WriteLine($"You've Chosen to Level Up {nameToLevelUp}");

        var foundCharacter = characterReader.FindCharacter(nameToLevelUp);

        if (foundCharacter != null)
        {
            foundCharacter.CharacterLevel++;
        }
        else
        {
            _output.WriteLine("Character not found.");
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

public class EquipmentManager
{
    public string[] EquipmentOptions {get;set;} = {"Armor","Book","Cloak","Dagger","Horse","Lockpick","Mace","Potion","Robe","Shield","Staff","Sword"};

    
    public void DisplayEquipmentMenu()
    {
        for (int i = 0; i < EquipmentOptions.Length; i++)
            {
                Console.WriteLine($"{i+1}: {EquipmentOptions[i]}");
            }
    }
}

public class CharcaterClassManager
{
    public string[] CharacterClassOptions {get;set;} = {"Cleric", "Fighter", "Rogue", "Wizard"};

    public void DisplayCharacterClassMenu()
    {
        for (int i = 0; i < CharacterClassOptions.Length; i++)
        {
            Console.WriteLine($"{i+1}: {CharacterClassOptions[i]}");
        }
    }
}

public class CharacterReader
{
    public string[] CharacterLines {get;set;}
    public List<Character> CharactersList {get;set;} = new List<Character>();
    public List<string> CharacterNamesList {get;set;} // For menu of characters

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

    public void LoadCharacters()
    {
        for (int i = 1; i < CharacterLines.Length; i++)
        {
            string line = CharacterLines[i];

            string characterName = GetName(line);

            var (characterClass, characterLevel, characterHitPoints, characterEquipment) = GetCharacterTraits(line);

            CharactersList.Add(new Character()
            {
                CharacterName = characterName,
                CharacterClass = characterClass,
                CharacterLevel = characterLevel,
                CharacterHitPoints = characterHitPoints,
                CharacterEquipment = characterEquipment
            });
        }

    }

    public void DisplayCharacterNamesMenu()
    {
        CharacterNamesList = new List<string>();

        foreach (var character in CharactersList)

        {
            CharacterNamesList.Add(character.CharacterName);
        }

        for (int i = 0; i < CharacterNamesList.Count; i++)
        {
            Console.WriteLine($"{i+1}: {CharacterNamesList[i]}");
        }
    }

    public Character FindCharacter(string characterSearch)
    {
        var foundCharacter = CharactersList.Where(c => c.CharacterName == characterSearch).FirstOrDefault();
        return foundCharacter;
    }

    public void DisplayCharacters()
    {
        foreach (var character in CharactersList)
            {
                Console.WriteLine($"Name: {character.CharacterName}; Class: {character.CharacterClass}; Level: {character.CharacterLevel}; Hit Points: {character.CharacterHitPoints};  Equipment: {string.Join(", ", character.CharacterEquipment)}");
            }
    }

    public void AddCharacter(string newCharacter, string newClass, string[] choicesArray)
    {
        CharactersList.Add(new Character()
            {
                CharacterName = newCharacter,
                CharacterClass = newClass,
                CharacterLevel = 1,
                CharacterHitPoints = 10,
                CharacterEquipment = choicesArray
            });

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