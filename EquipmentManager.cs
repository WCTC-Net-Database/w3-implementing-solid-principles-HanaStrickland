namespace CharacterConsole;

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