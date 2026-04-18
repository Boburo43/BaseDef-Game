using System.Resources;
using UnityEngine;

public class DevCommands : MonoBehaviour
{
    private void Start()
    {
        var console = GetComponent<DevConsoleUI>();

        if (console == null)
        {
            Debug.LogError("[DevCommands] DevConsoleUI not found.");
            return;
        }

        console.Register("add", args =>
        {
            if (args.Length < 2)
                return "Usage: add <type> <amount> (types: Wood, Stone)";

            if (!int.TryParse(args[1], out int amount))
                return "Amount must be a number.";

            if (!System.Enum.TryParse(args[0], true, out ResourceType type))
                return $"Unknown type '{args[0]}'. Valid types: Wood, Stone";

            ResourseManager.Instance.Add(type, amount);

            return $"+{amount} {type}";
        });
    }
}