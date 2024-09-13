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
            _output.WriteLine("4. Find Character");
            _output.WriteLine("5. Exit");
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
                    FindACharacter();
                    break;
                case "5":
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
        
        Console.Write("Enter Your Choice: ");
        string choice = Console.ReadLine();
                
        var foundCharacter = characterReader.FindCharacter(choice);

        if (foundCharacter != null)
        {
             _output.WriteLine($"You've Chosen to Level Up {foundCharacter.CharacterName}");
            foundCharacter.CharacterLevel++;
        }
        else
        {
            _output.WriteLine("Character not found.");
        }
        
    }

    public void FindACharacter()
    {
        characterReader.DisplayCharacterNamesMenu();
        
        Console.Write("Enter Your Choice: ");
        string choice = Console.ReadLine();
                
        var foundCharacter = characterReader.FindCharacter(choice);

        if (foundCharacter != null)
        {
            foundCharacter.DisplayCharacterInformation();
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

    public void DisplayCharacterInformation()
    {
        Console.WriteLine($"Name: {CharacterName}\nClass: {CharacterClass}\nLevel: {CharacterLevel}\nHit Points: {CharacterHitPoints}\nEquipment: {string.Join("|", CharacterEquipment)}");
    }

}