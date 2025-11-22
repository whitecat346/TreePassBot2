namespace TreePassBot2.PluginSdk.Interfaces;

public interface ICommandContext
{
    ulong SenderId { get; }
    ulong GroupId { get; }
    string RawMessage { get; }
    string[] Args { get; }

    Task ReplyAsync(string msg);
}